using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// Adaptador para integrar RfidReaderService com interface IRfidReader.
/// Fornece compatibilidade com pipeline de tags existente (EntradaViewModel, SaidaViewModel).
/// 
/// NOVO: Usa RfidReaderService com P/Invoke correto (UsbOpen, UHFInventory, UHFGetTagData).
/// </summary>
public sealed class R3DllReader : IRfidReader
{
    private readonly RfidConfig _cfg;
    private readonly AppLogger _log;
    private RfidReaderService? _service;
    private CancellationTokenSource? _inventoryCts;
    private Task? _readTask;

    public R3DllReader(RfidConfig cfg, AppLogger log)
    {
        _cfg = cfg;
        _log = log;
    }

    public string Name => "R3 UHF Reader (USB/COM)";
    public bool IsConnected => _service?.IsConnected ?? false;
    
    // PARTE I: Expõe estado do inventário
    public bool IsInventoryRunning => _service?.IsInventoryRunning ?? false;

    public event EventHandler<RfidTagReadEventArgs>? TagRead;

    /// <summary>
    /// Conecta ao reader RFID via USB (padrão).
    /// Se falhar, pode tentar COM manualmente.
    /// </summary>
    public async Task ConnectAsync(CancellationToken ct)
    {
        await Task.Run(() =>
        {
            try
            {
                _log.Info("🔧 Iniciando conexão com Reader RFID R3...");
                
                // Cria serviço de leitura (com config de hardware)
                _service = new RfidReaderService(_log, _cfg);
                
                // Conecta via USB
                bool connected = _service.ConnectUsb();
                
                if (!connected)
                {
                    _log.Error("❌ Falha ao conectar via USB");
                    _log.Info("💡 Dica: Verifique se hardware está conectado e UHFAPI.dll está no caminho correto");
                    throw new Exception("Falha ao conectar com Reader RFID");
                }
                
                // Hookeia eventos
                if (_service != null)
                {
                    _service.TagDetected += OnTagDetected;
                }
                
                _log.Info("✅ Reader RFID conectado e pronto");
            }
            catch (DllNotFoundException dllEx)
            {
                _log.Error($"❌ UHFAPI.dll não encontrada: {dllEx.Message}");
                _log.Error($"   Esperado em: {AppDomain.CurrentDomain.BaseDirectory}");
                throw;
            }
            catch (EntryPointNotFoundException epEx)
            {
                _log.Error($"❌ Entry point não encontrado na DLL: {epEx.Message}");
                _log.Error("   Verifique se UHFAPI.dll contém os exports: UsbOpen, UHFInventory, UHFGetTagData");
                throw;
            }
            catch (Exception ex)
            {
                _log.Error($"❌ Erro ao conectar: {ex.Message}");
                throw;
            }
        }, ct);
    }

    
    /// <summary>
    /// Desconecta do reader e libera recursos.
    /// </summary>
    public async Task DisconnectAsync(CancellationToken ct)
    {
        await Task.Run(() =>
        {
            try
            {
                // Para leitura
                if (_readTask != null && !_readTask.IsCompleted)
                {
                    _service?.StopInventory();
                }
                
                // Desconecta
                if (_service != null)
                {
                    _service.TagDetected -= OnTagDetected;
                    _service.Disconnect();
                    _service.Dispose();
                    _service = null;
                }
                
                _inventoryCts?.Dispose();
                _inventoryCts = null;
                
                _log.Info("✅ Reader desconectado");
            }
            catch (Exception ex)
            {
                _log.Error($"❌ Erro ao desconectar: {ex.Message}");
            }
        }, ct);
    }
    
    /// <summary>
    /// Inicia leitura contínua de tags.
    /// </summary>
    public Task StartReadingAsync(CancellationToken ct)
    {
        if (!IsConnected || _service == null)
        {
            throw new InvalidOperationException("Reader não conectado");
        }
        
        try
        {
            _inventoryCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            _service.StartInventory(_inventoryCts.Token);
            
            _log.Info("▶️  Leitura RFID iniciada");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _log.Error($"❌ Erro ao iniciar leitura: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Para leitura de tags.
    /// </summary>
    public Task StopReadingAsync(CancellationToken ct)
    {
        try
        {
            _service?.StopInventory();
            _log.Info("⏸️  Leitura RFID parada");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _log.Error($"❌ Erro ao parar leitura: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Define potência de transmissão (5-30 dBm).
    /// </summary>
    public async Task SetPowerAsync(int power, CancellationToken ct)
    {
        if (_service == null)
        {
            throw new InvalidOperationException("Reader não conectado");
        }
        
        await Task.Run(() =>
        {
            try
            {
                byte dbm = (byte)Math.Clamp(power, 5, 30);
                _log.Info($"⚡ Configurando potência para {dbm} dBm...");
            }
            catch (Exception ex)
            {
                _log.Error($"❌ Erro ao configurar potência: {ex.Message}");
            }
        }, ct);
    }

    /// <summary>
    /// Lê UMA ÚNICA tag e fecha automaticamente.
    /// IMPORTANTE: Apenas funciona em modo R3Dll (hardware real), não em modo simulado.
    /// Retorna o EPC da tag ou null se timeout.
    /// </summary>
    public async Task<string?> ReadSingleTagAsync(CancellationToken ct)
    {
        // Verifica se está em modo R3Dll
        if (!_cfg.ReaderMode.Equals("R3Dll", StringComparison.OrdinalIgnoreCase))
        {
            _log.Warn("❌ ReadSingleTagAsync apenas funciona em modo R3Dll (hardware real), não em modo simulado");
            return null;
        }

        if (!IsConnected || _service == null)
        {
            _log.Error("❌ Reader não conectado");
            return null;
        }

        try
        {
            _log.Info("📖 Iniciando leitura de UMA ÚNICA tag...");

            string? receivedEpc = null;
            bool tagReceived = false;
            var lockObj = new object();

            // Handler para capturar primeira tag
            void OnSingleTag(string epc, byte rssi)
            {
                lock (lockObj)
                {
                    if (!tagReceived)
                    {
                        tagReceived = true;
                        receivedEpc = epc;
                        _log.Info($"✅ Tag recebida: {epc} (RSSI: -{rssi} dBm)");
                    }
                }
            }

            // Registra o handler
            if (_service != null)
            {
                _service.TagDetected += OnSingleTag;
            }

            try
            {
                // Inicia inventário
                _service?.StartInventory(ct);

                // Aguarda tag ou timeout (5 segundos)
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                const int timeoutMs = 5000;

                while (!tagReceived && !ct.IsCancellationRequested && stopwatch.ElapsedMilliseconds < timeoutMs)
                {
                    await Task.Delay(50, ct);
                }

                stopwatch.Stop();

                if (!tagReceived)
                {
                    _log.Warn($"⚠️  Timeout: Nenhuma tag recebida em {timeoutMs}ms");
                }

                return receivedEpc;
            }
            finally
            {
                // Para leitura
                _service?.StopInventory();

                // Remove handler
                if (_service != null)
                {
                    _service.TagDetected -= OnSingleTag;
                }

                _log.Info("🛑 Leitura de tag única finalizada");
            }
        }
        catch (OperationCanceledException)
        {
            _log.Info("⚠️  Leitura de tag cancelada");
            return null;
        }
        catch (Exception ex)
        {
            _log.Error($"❌ Erro ao ler tag: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Consulta de tag com leitura única (InventorySingle).
    /// </summary>
    public async Task<string?> ConsultarTagAsync(TimeSpan timeout, CancellationToken ct)
    {
        if (!_cfg.ReaderMode.Equals("R3Dll", StringComparison.OrdinalIgnoreCase))
        {
            _log.Warn("❌ ConsultarTagAsync apenas funciona em modo R3Dll (hardware real), não em modo simulado");
            return null;
        }

        // Tenta reconectar se necessário
        if (!IsConnected || _service == null)
        {
            _log.Warn("⚠️ Reader não conectado, tentando reconectar...");
            try
            {
                await ConnectAsync(ct);
            }
            catch (Exception connEx)
            {
                _log.Error($"❌ Falha ao reconectar: {connEx.Message}");
                return null;
            }
        }

        // Verifica novamente após tentativa de reconexão
        if (!IsConnected || _service == null)
        {
            _log.Error("❌ Reader não conectado - verifique hardware e UHFAPI.dll");
            return null;
        }

        try
        {
            _log.Info($"🔎 Consultando tag com timeout de {timeout.TotalSeconds:F1}s...");
            return await _service.ConsultarTagAsync(timeout, ct);
        }
        catch (Exception ex)
        {
            _log.Error($"❌ Erro em ConsultarTagAsync: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Handler para evento de tag detectada do RfidReaderService.
    /// Converte para RfidTagReadEventArgs e emite via evento TagRead.
    /// </summary>
    private void OnTagDetected(string epc, byte rssi)
    {
        try
        {
            // Converte RSSI de byte unsigned para signed dBm
            // RSSI = -rssi (valores típicos 40-100 = -40 a -100 dBm)
            int rssiDbm = -(rssi);
            
            _log.Info($"🔔 R3DllReader.TagRead disparado: EPC={epc}, RSSI={rssiDbm} dBm");
            TagRead?.Invoke(this, new RfidTagReadEventArgs
            {
                Epc = epc,
                Rssi = rssiDbm,
                TimestampUtc = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _log.Warn($"⚠️  Erro ao processar tag: {ex.Message}");
        }
    }
}

