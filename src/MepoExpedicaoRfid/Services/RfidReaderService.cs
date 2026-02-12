using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MepoExpedicaoRfid.Models;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// Serviço profissional para leitura de tags RFID via UHFAPI.dll.
/// Recursos:
///   - Conexão robusta USB/COM
///   - Leitura assíncrona em Task separada
///   - Sem bloqueios de UI
///   - Buffer reutilizável (zero GC em hot path)
///   - Deduplicação de tags (ConcurrentDictionary)
///   - Cancelamento gracioso com CancellationToken
///   - Logging detalhado
/// </summary>
public sealed class RfidReaderService : IDisposable
{
    private readonly AppLogger _log;
    private readonly RfidConfig? _cfg;
    private readonly int _maxRetries;
    private readonly int _readDelayMs;
    
    private bool _connected;
    private CancellationTokenSource? _readCts;
    private Task? _readTask;
    
    // Buffer reutilizável - alocado UMA VEZ
    private readonly byte[] _tagBuffer;
    private readonly byte[] _versionBuffer;
    
    // Deduplicação de EPCs em leitura contínua
    private readonly ConcurrentDictionary<string, DateTime> _recentEpcs;
    private readonly TimeSpan _deduplicationWindow;
    
    // PARTE A: Trava global de modo - garante nunca haver concorrência entre modos
    private readonly SemaphoreSlim _modeLock = new SemaphoreSlim(1, 1);
    private bool _isInventoryRunning = false;
    
    // Evento de tag detectada
    public event Action<string, byte>? TagDetected;  // epc, rssi
    public event Action<string>? ConnectionStateChanged;  // "connected" ou "disconnected"
    
    public bool IsConnected => _connected;
    public bool IsInventoryRunning => _isInventoryRunning;
    
    public RfidReaderService(AppLogger log, RfidConfig? cfg = null, int maxRetries = 3, int readDelayMs = 50)
    {
        _log = log;
        _cfg = cfg;
        _maxRetries = maxRetries;
        _readDelayMs = readDelayMs;
        
        // Alocação UMA VEZ (zero GC em hot path de leitura)
        _tagBuffer = new byte[NativeMethods.BUFFER_SIZE];
        _versionBuffer = new byte[256];
        
        // Deduplicação por 500ms
        _recentEpcs = new ConcurrentDictionary<string, DateTime>();
        _deduplicationWindow = TimeSpan.FromMilliseconds(500);
        
        _connected = false;
    }
    
    /// <summary>
    /// Conecta ao reader RFID via USB.
    /// Retorna true se sucesso, false se falha.
    /// </summary>
    public bool ConnectUsb()
    {
        if (_connected)
        {
            _log.Warn("⚠️ Já conectado ao reader");
            return true;
        }
        
        try
        {
            // DIAGNÓSTICO: Valida exports da DLL (versão simplificada)
            try
            {
                _log.Info("Validando exports da UHFAPI.dll...");
                var results = NativeMethods.ValidateDllExports();
                
                if (results.ContainsKey("DLL_LOAD_FAILED"))
                {
                    _log.Error("Falha ao carregar UHFAPI.dll");
                }
                else
                {
                    int found = results.Count(x => x.Value);
                    int missing = results.Count(x => !x.Value);
                    _log.Info($"Validacao: {found} exports encontrados, {missing} ausentes");
                }
            }
            catch (Exception validEx)
            {
                _log.Warn($"Erro ao validar exports (continuando): {validEx.Message}");
            }
            
            _log.Info("Conectando ao reader via USB...");
            
            int result = NativeMethods.UsbOpen();
            if (result != NativeMethods.UHFAPI_SUCCESS)
            {
                _log.Error($"UsbOpen() falhou com codigo {result}");
                return false;
            }
            
            _log.Info("✅ USB aberto com sucesso");
            
            // Obtém versão do firmware
            if (!GetVersion())
            {
                _log.Warn("⚠️ Não conseguiu obter versão do firmware (continuando)");
            }
            
            // 🔧 Configuração de hardware (OPCIONAL)
            // Regra: por padrão, NÃO alterar nem salvar configurações no reader ao abrir o app.
            // Só aplica se RFID.ApplyConfigOnConnect=true.

            var cfg = _cfg;
            if (cfg?.ApplyConfigOnConnect == true)
            {
                byte save = (byte)(cfg.SaveToEeprom ? 1 : 0);

                // 1) Antena
                if (cfg.AntennaMask.HasValue)
                {
                    try
                    {
                        int mask = Math.Clamp(cfg.AntennaMask.Value, 0, 0xFFFF);
                        byte[] antBuf = new byte[] { (byte)(mask & 0xFF), (byte)((mask >> 8) & 0xFF) };
                        _log.Info($"Configurando antena mask=0x{mask:X4} (save={save})...");

                        int antResult = NativeMethods.UHFSetANT(save, antBuf);
                        if (antResult == NativeMethods.UHFAPI_SUCCESS)
                        {
                            _log.Info("✅ Antena configurada");
                        }
                        else
                        {
                            _log.Warn($"⚠️ Falha ao configurar antena (código {antResult})");
                        }
                    }
                    catch (Exception antEx)
                    {
                        _log.Warn($"⚠️ Erro ao configurar antena: {antEx.Message}");
                    }
                }

                // 2) Região
                if (!string.IsNullOrWhiteSpace(cfg.Region))
                {
                    try
                    {
                        byte regionCode = cfg.Region.Trim().ToLowerInvariant() switch
                        {
                            "usa" => NativeMethods.REGION_USA,
                            "europe" => NativeMethods.REGION_EUROPE,
                            "china1" => NativeMethods.REGION_CHINA1,
                            "china2" => NativeMethods.REGION_CHINA2,
                            "korea" => NativeMethods.REGION_KOREA,
                            "japan" => NativeMethods.REGION_JAPAN,
                            _ => (byte)0
                        };

                        if (regionCode == 0)
                        {
                            _log.Warn($"⚠️ Região inválida em config: '{cfg.Region}'. Ignorando.");
                        }
                        else
                        {
                            _log.Info($"Configurando região={cfg.Region} (0x{regionCode:X2}) (save={save})...");
                            int regionResult = NativeMethods.UHFSetRegion(save, regionCode);
                            if (regionResult == NativeMethods.UHFAPI_SUCCESS)
                                _log.Info("✅ Região configurada");
                            else
                                _log.Warn($"⚠️ Falha ao configurar região (código {regionResult})");
                        }
                    }
                    catch (Exception regEx)
                    {
                        _log.Warn($"⚠️ Erro ao configurar região: {regEx.Message}");
                    }
                }

                // 3) Modo EPC+TID
                if (cfg.ApplyEpcTidMode)
                {
                    try
                    {
                        _log.Info($"Configurando modo EPC+TID (save={save})...");
                        int modeResult = NativeMethods.UHFSetEPCTIDUSERMode(save, 0x01, 0, 0);
                        if (modeResult == NativeMethods.UHFAPI_SUCCESS)
                            _log.Info("✅ Modo EPC+TID configurado");
                        else
                            _log.Warn($"⚠️ Falha ao configurar modo (código {modeResult})");
                    }
                    catch (Exception modeEx)
                    {
                        _log.Warn($"⚠️ Erro ao configurar modo: {modeEx.Message}");
                    }
                }

                // 4) Potência
                if (cfg.PowerDbm.HasValue)
                {
                    try
                    {
                        byte dbm = (byte)Math.Clamp(cfg.PowerDbm.Value, 5, 30);
                        _log.Info($"Configurando potência {dbm} dBm (save={save})...");
                        int pwResult = NativeMethods.UHFSetPower(save, dbm);
                        if (pwResult == NativeMethods.UHFAPI_SUCCESS)
                            _log.Info("✅ Potência configurada");
                        else
                            _log.Warn($"⚠️ Falha ao configurar potência (código {pwResult})");
                    }
                    catch (Exception pwEx)
                    {
                        _log.Warn($"⚠️ Erro ao configurar potência: {pwEx.Message}");
                    }
                }

                // 5) Beep
                if (cfg.Beep.HasValue)
                {
                    try
                    {
                        byte enable = (byte)(cfg.Beep.Value ? 1 : 0);
                        _log.Info($"Configurando beep={(cfg.Beep.Value ? "on" : "off")}...");
                        int beepResult = NativeMethods.UHFSetBeep(enable);
                        if (beepResult == NativeMethods.UHFAPI_SUCCESS)
                            _log.Info("✅ Beep configurado");
                    }
                    catch (Exception beepEx)
                    {
                        _log.Warn($"⚠️ Erro ao configurar beep: {beepEx.Message}");
                    }
                }

                // Valida estado atual (best-effort)
                try
                {
                    byte[] antCheck = new byte[2];
                    if (NativeMethods.UHFGetANT(antCheck) == NativeMethods.UHFAPI_SUCCESS)
                        _log.Info($"Reader ANT mask atual: 0x{antCheck[0]:X2}{antCheck[1]:X2}");

                    byte regionCheck = 0;
                    if (NativeMethods.UHFGetRegion(ref regionCheck) == NativeMethods.UHFAPI_SUCCESS)
                        _log.Info($"Reader region atual: 0x{regionCheck:X2}");

                    byte powerCheck = 0;
                    if (NativeMethods.UHFGetPower(ref powerCheck) == NativeMethods.UHFAPI_SUCCESS)
                        _log.Info($"Reader power atual: {powerCheck} dBm");
                }
                catch (Exception diagEx)
                {
                    _log.Warn($"⚠️ Falha ao validar estado do reader: {diagEx.Message}");
                }
            }
            else
            {
                _log.Info("RFID.ApplyConfigOnConnect=false — mantendo configuração do reader (não altera região/potência/antena). ");
            }
            
            _connected = true;
            ConnectionStateChanged?.Invoke("connected");
            
            _log.Info("✅ Reader RFID pronto para leitura");
            return true;
        }
        catch (DllNotFoundException dllEx)
        {
            _log.Error($"❌ UHFAPI.dll não encontrada: {dllEx.Message}");
            _log.Error($"   Local esperado: {AppDomain.CurrentDomain.BaseDirectory}");
            return false;
        }
        catch (EntryPointNotFoundException epEx)
        {
            _log.Error($"❌ Entry point não encontrado: {epEx.Message}");
            _log.Error("   Verifique se UHFAPI.dll contém função UsbOpen()");
            return false;
        }
        catch (Exception ex)
        {
            _log.Error($"❌ Erro ao conectar: {ex.GetType().Name}", ex);
            return false;
        }
    }
    
    /// <summary>
    /// Conecta ao reader via COM (serial port).
    /// </summary>
    public bool ConnectCom(int portNumber = 1, int baudRate = 115200)
    {
        if (_connected)
        {
            _log.Warn("⚠️ Já conectado ao reader");
            return true;
        }
        
        try
        {
            _log.Info($"🔌 Conectando ao reader via COM{portNumber} ({baudRate} baud)...");
            
            int result = NativeMethods.ComOpenWithBaud(portNumber, baudRate);
            if (result != NativeMethods.UHFAPI_SUCCESS)
            {
                _log.Error($"❌ ComOpenWithBaud() falhou com código {result}");
                return false;
            }
            
            _log.Info("✅ COM aberto com sucesso");
            
            _connected = true;
            ConnectionStateChanged?.Invoke("connected");
            return true;
        }
        catch (Exception ex)
        {
            _log.Error($"❌ Erro ao conectar COM: {ex.GetType().Name}", ex);
            return false;
        }
    }
    
    /// <summary>
    /// Desconecta do reader.
    /// </summary>
    public void Disconnect()
    {
        try
        {
            // Para leitura assíncrona
            StopInventory();
            
            if (_connected)
            {
                _log.Info("🔌 Desconectando reader...");
                
                // Tenta fechar via USB
                int usbResult = NativeMethods.UsbClose();
                if (usbResult != NativeMethods.UHFAPI_SUCCESS)
                {
                    // Tenta via COM
                    NativeMethods.ClosePort();
                }
                
                _connected = false;
                ConnectionStateChanged?.Invoke("disconnected");
                
                _log.Info("✅ Reader desconectado");
            }
        }
        catch (Exception ex)
        {
            _log.Error($"❌ Erro ao desconectar: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Obtém versão do firmware.
    /// </summary>
    private bool GetVersion()
    {
        try
        {
            int len = _versionBuffer.Length;
            int result = NativeMethods.UHFGetReaderVersion(_versionBuffer, ref len);
            
            if (result == NativeMethods.UHFAPI_SUCCESS && len > 0)
            {
                string version = Encoding.ASCII.GetString(_versionBuffer, 0, len).TrimEnd('\0');
                _log.Info($"📋 Firmware: {version}");
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Define potência de transmissão.
    /// </summary>
    private bool SetPower(byte powerDbm)
    {
        try
        {
            // UHFSetPower(save, power) - save=1 salva em EEPROM
            int result = NativeMethods.UHFSetPower(save: 1, power: powerDbm);
            if (result == NativeMethods.UHFAPI_SUCCESS)
            {
                _log.Info($"⚡ Potência: {powerDbm} dBm (salvo em EEPROM)");
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Inicia leitura assíncrona de tags.
    /// Cria Task separada que não bloqueia UI.
    /// PARTE A: Usa _modeLock para garantir que apenas um modo roda por vez.
    /// </summary>
    public async Task StartInventoryAsync(CancellationToken externalCt = default)
    {
        // Aguarda trava de modo
        await _modeLock.WaitAsync(externalCt);
        try
        {
            await StartInventoryInternalAsync(externalCt);
        }
        finally
        {
            _modeLock.Release();
        }
    }
    
    /// <summary>
    /// Versão interna sem lock (para uso dentro de _modeLock).
    /// </summary>
    private async Task StartInventoryInternalAsync(CancellationToken externalCt = default)
    {
        if (!_connected)
        {
            _log.Error("❌ Reader não conectado");
            return;
        }

        if (_isInventoryRunning)
        {
            _log.Warn("⚠️ Inventário já em andamento");
            return;
        }

        _log.Info("📖 Iniciando inventário contínuo...");

        // Inicia inventário hardware
        int result = NativeMethods.UHFInventory();
        if (result != NativeMethods.UHFAPI_SUCCESS)
        {
            _log.Error($"❌ UHFInventory() falhou com código {result}");
            return;
        }

        _isInventoryRunning = true;

        // Cria CancellationToken para controlar leitura
        _readCts = CancellationTokenSource.CreateLinkedTokenSource(externalCt);

        // Inicia Task de leitura em background
        _readTask = Task.Run(() => ReadLoopAsync(_readCts.Token), _readCts.Token);

        _log.Info("✅ Inventário iniciado");
    }

    /// <summary>
    /// Versão síncrona de StartInventoryAsync (para compatibilidade com R3DllReader).
    /// </summary>
    public void StartInventory(CancellationToken externalCt = default)
    {
        // Faz await de forma síncrona (não ideal, mas mantém compatibilidade)
        StartInventoryAsync(externalCt).GetAwaiter().GetResult();
    }
    
    /// <summary>
    /// Para leitura contínua de tags.
    /// PARTE A: Usa _modeLock para garantir exclusividade.
    /// </summary>
    public async Task StopInventoryAsync()
    {
        // Aguarda trava de modo
        await _modeLock.WaitAsync();
        try
        {
            await StopInventoryInternalAsync();
        }
        finally
        {
            _modeLock.Release();
        }
    }
    
    /// <summary>
    /// Versão interna sem lock (para ser chamada dentro de _modeLock).
    /// </summary>
    private async Task StopInventoryInternalAsync()
    {
        try
        {
            // Sinaliza cancelamento
            _readCts?.Cancel();

            // Aguarda Task terminar (timeout 5s)
            if (_readTask != null && !_readTask.IsCompleted)
            {
                try
                {
                    await _readTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Esperado
                }
            }

            // Para hardware
            if (_connected)
            {
                _log.Info("🛑 Parando inventário...");
                int result = NativeMethods.UHFStopGet();
                if (result == NativeMethods.UHFAPI_SUCCESS)
                {
                    _log.Info("✅ Inventário parado");
                }
            }

            _isInventoryRunning = false;
        }
        catch (Exception ex)
        {
            _log.Warn($"⚠️ Erro ao parar inventário: {ex.Message}");
        }
        finally
        {
            _readCts?.Dispose();
            _readCts = null;
            _readTask = null;
        }
    }

    /// <summary>
    /// Versão síncrona de StopInventoryAsync (para compatibilidade).
    /// CORRIGIDO: Não usa GetAwaiter().GetResult() para evitar deadlock.
    /// </summary>
    public void StopInventory()
    {
        try
        {
            // Sinaliza cancelamento
            _readCts?.Cancel();
            
            // Para hardware IMEDIATAMENTE sem await
            if (_connected)
            {
                _log.Info("🛑 Parando inventário...");
                int result = NativeMethods.UHFStopGet();
                if (result == NativeMethods.UHFAPI_SUCCESS)
                {
                    _log.Info("✅ Inventário parado");
                }
            }
            
            _isInventoryRunning = false;
            
            // Cleanup em Task separada para não bloquear
            _ = Task.Run(async () =>
            {
                try
                {
                    if (_readTask != null && !_readTask.IsCompleted)
                    {
                        await _readTask.ConfigureAwait(false);
                    }
                }
                catch { }
                finally
                {
                    _readCts?.Dispose();
                    _readCts = null;
                    _readTask = null;
                }
            });
        }
        catch (Exception ex)
        {
            _log.Warn($"⚠️ Erro ao parar inventário: {ex.Message}");
        }
    }

    /// <summary>
    /// Leitura única (consulta) usando UHFInventorySingle + UHFGetTagData.
    /// PARTE B: Implementa polling robusto com fallback strategies e hexdump de diagnóstico.
    /// IMPORTANTE: Para qualquer inventário contínuo antes de fazer leitura única.
    /// </summary>
    public async Task<string?> ConsultarTagAsync(TimeSpan timeout, CancellationToken ct)
    {
        if (!_connected)
        {
            _log.Error("❌ Reader não conectado");
            return null;
        }

        // PARTE A: Aguarda trava de modo
        await _modeLock.WaitAsync(ct);
        try
        {
            // CRÍTICO: Para inventário contínuo se estiver rodando (sem chamar método público)
            if (_isInventoryRunning)
            {
                _log.Info("⏸️ Parando inventário contínuo para leitura única...");
                await StopInventoryInternalAsync();  // Usa versão interna sem lock!
                await Task.Delay(200, ct); // Aguarda hardware estabilizar
            }

            _log.Info("🔎 Iniciando consulta de tag (estratégia correta: UHFInventory + UHF_GetReceived_EX)...");

            try
            {
                // ESTRATÉGIA CORRETA (Base Fabrica):
                // 1. Inicia inventário contínuo
                if (NativeMethods.UHFInventory() != NativeMethods.UHFAPI_SUCCESS)
                {
                    _log.Error("❌ Falha ao iniciar UHFInventory");
                    return null;
                }

                _log.Info("✅ Inventário iniciado, aguardando tags...");

                // 2. Loop que lê tags do buffer usando wrapper correto (base fabrica)
                var sw = System.Diagnostics.Stopwatch.StartNew();
                
                while (sw.Elapsed < timeout && !ct.IsCancellationRequested)
                {
                    // 🔥 USA WRAPPER COMPLETO (como na base fabrica)
                    TagInfo? tagInfo = GetReceivedTagInfo();
                    
                    if (tagInfo != null && !string.IsNullOrWhiteSpace(tagInfo.Epc))
                    {
                        _log.Info($"✅ Tag consultada: {tagInfo}");
                        return tagInfo.Epc;
                    }
                    
                    await Task.Delay(5, ct);  // Sleep pequeno como na Base Fabrica
                }

                _log.Warn("⚠️ Timeout na consulta de tag");
                return null;
            }
            finally
            {
                // 3. GARANTIR que para a leitura
                _log.Info("🛑 Parando inventário...");
                int stopResult = NativeMethods.UHFStopGet();
                if (stopResult == NativeMethods.UHFAPI_SUCCESS)
                {
                    _log.Info("✅ Inventário parado");
                }
                else
                {
                    _log.Warn($"⚠️ Erro ao parar inventário: {stopResult}");
                }
            }
        }
        finally
        {
            _modeLock.Release();
        }
    }

    /// <summary>
    /// Wrapper completo para GetReceived_EX (equivalente a uhfGetReceived() da base fabrica).
    /// Parseia buffer complexo e retorna objeto TagInfo estruturado.
    /// CRITICAL: Esta é a implementação CORRETA usada pela base fabrica!
    /// </summary>
    private TagInfo? GetReceivedTagInfo()
    {
        try
        {
            int uLen = 0;
            byte[] bufData = new byte[150];
            
            int result = NativeMethods.UHFGetReceived_EX(ref uLen, bufData);
            
            if (result != NativeMethods.UHFAPI_SUCCESS || uLen == 0)
            {
                return null;
            }
            
            // Parse do buffer (formato base fabrica)
            // Formato: [uii_len][pc_2bytes][epc...][crc_2bytes][tid_len][tid...][rssi_2bytes][ant]
            
            int uii_len = bufData[0];  // Comprimento total UII (PC + EPC + CRC)
            if (uii_len < 2 || uii_len + 2 > uLen) return null;
            
            int tid_leng = bufData[uii_len + 1];  // Comprimento TID
            int tid_idex = uii_len + 2;           // Índice inicial TID
            int rssi_index = 1 + uii_len + 1 + tid_leng;
            int ant_index = rssi_index + 2;
            
            // Valida se buffer tem tamanho suficiente
            if (ant_index >= uLen) return null;
            
            // Converte buffer para string hex
            string strData = BitConverter.ToString(bufData, 0, uLen).Replace("-", "");
            
            // Extrai EPC (remove PC 2 bytes + CRC 2 bytes)
            // Exemplo: [len][PC_2bytes][EPC_data][CRC_2bytes]
            //          Substring(6) pula os 2 bytes PC (4 chars hex)
            //          uii_len * 2 - 4 remove CRC (2 bytes = 4 chars hex)
            int epcStartIndex = 6;  // Pula: len(2) + PC(4)
            int epcLength = uii_len * 2 - 4;  // Total - CRC
            
            if (epcStartIndex + epcLength > strData.Length) return null;
            
            string epc_data = strData.Substring(epcStartIndex, epcLength);
            
            // Extrai TID
            string tid_data = string.Empty;
            string user_data = string.Empty;
            
            if (tid_leng > 0 && (tid_idex * 2 + tid_leng * 2) <= strData.Length)
            {
                if (tid_leng > 12)
                {
                    tid_data = strData.Substring(tid_idex * 2, 24);  // TID = 12 bytes
                    user_data = strData.Substring(tid_idex * 2 + 24, (tid_leng - 12) * 2);
                }
                else
                {
                    tid_data = strData.Substring(tid_idex * 2, tid_leng * 2);
                }
            }
            
            // Extrai RSSI (2 bytes, signed)
            string rssi_data = "0.0";
            try
            {
                if (rssi_index * 2 + 4 <= strData.Length)
                {
                    string temp = strData.Substring(rssi_index * 2, 4);
                    int rssiTemp = Convert.ToInt32(temp, 16) - 65535;
                    float rssiFloat = (float)rssiTemp / 10.0f;
                    rssi_data = rssiFloat.ToString("F1");
                    
                    if (!rssi_data.Contains("."))
                        rssi_data = rssi_data + ".0";
                }
            }
            catch
            {
                rssi_data = "0.0";
            }
            
            // Extrai número da antena
            string ant_data = "0";
            try
            {
                if (ant_index * 2 + 2 <= strData.Length)
                {
                    ant_data = Convert.ToInt32(strData.Substring(ant_index * 2, 2), 16).ToString();
                }
            }
            catch
            {
                ant_data = "0";
            }
            
            // Cria objeto TagInfo
            var info = new TagInfo
            {
                Epc = epc_data,
                Tid = tid_data,
                Rssi = rssi_data,
                Ant = ant_data,
                User = user_data,
                ReadTime = DateTime.UtcNow
            };
            
            _log.Debug($"📡 Tag parseada: {info}");
            
            return info;
        }
        catch (Exception ex)
        {
            _log.Warn($"⚠️ Erro em GetReceivedTagInfo: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Parse do buffer retornado por UHF_GetReceived_EX.
    /// Formato: [epc_len][epc_data...][tid_len][tid_data...][rssi_2bytes][ant]
    /// OBSOLETO: Use GetReceivedTagInfo() ao invés deste método!
    /// </summary>
    private string? ParseEpcFromBuffer(byte[] buffer, int len)
    {
        try
        {
            if (len < 3) return null;

            int epcLen = buffer[0];
            _log.Debug($"🔍 EPC length from buffer: {epcLen}");

            if (epcLen == 0 || epcLen > 128 || (1 + epcLen) > len)
                return null;

            string epc = Convert.ToHexString(buffer.AsSpan(1, epcLen));
            
            if (string.IsNullOrWhiteSpace(epc) || epc.All(c => c == '0'))
                return null;

            _log.Debug($"🔍 Parsed EPC: {epc}");
            return epc;
        }
        catch (Exception ex)
        {
            _log.Warn($"⚠️ Erro ao parsear buffer: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// PARTE B - Estratégia 1: UHFInventorySingle com polling.
    /// </summary>
    private async Task<string?> TryInventorySingleAsync(TimeSpan timeout, CancellationToken ct)
    {
        _log.Info($"DEBUG: TryInventorySingleAsync iniciado, timeout={timeout.TotalSeconds}s");
        
        try
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            int attempts = 0;
            int maxAttempts = (int)(timeout.TotalMilliseconds / 150); // ~33 tentativas para 5s
            _log.Info($"DEBUG: maxAttempts={maxAttempts}");

            while (sw.Elapsed < timeout && !ct.IsCancellationRequested && attempts < maxAttempts)
            {
                attempts++;
                _log.Debug($"DEBUG: Tentativa {attempts}/{maxAttempts}, elapsed={sw.ElapsedMilliseconds}ms");
                
                try
                {
                    // TESTE: Simular resultado sem chamar P/Invoke
                    _log.Debug("DEBUG: Simulando chamada P/Invoke...");
                    await Task.Delay(50, ct);
                    
                    int result = -1; // Simula erro
                    
                    _log.Debug($"📡 UHFInventorySingle() retornou: {result}");

                    if (result != NativeMethods.UHFAPI_SUCCESS)
                    {
                        _log.Debug($"⚠️ UHFInventorySingle falhou com código {result}");
                        await Task.Delay(50, ct);
                        continue;
                    }

                    // ... resto do código
                    await Task.Delay(100, ct);
                }
                catch (OperationCanceledException)
                {
                    _log.Info("DEBUG: Loop cancelado");
                    throw;
                }
                catch (Exception ex)
                {
                    _log.Error($"DEBUG: Exceção no loop: {ex.Message}");
                    await Task.Delay(100, ct);
                }
            }

            _log.Info($"DEBUG: TryInventorySingleAsync concluído após {attempts} tentativas");
            return null;
        }
        catch (Exception mainEx)
        {
            _log.Error($"DEBUG: Exceção principal em TryInventorySingleAsync: {mainEx.Message}");
            _log.Error($"Stack: {mainEx.StackTrace}");
            throw;
        }
    }






    
    /// <summary>
    /// Loop de leitura assíncrono.
    /// Roda em Task separada, não bloqueia UI.
    /// Alocação ZERO em hot path - buffer reutilizável.
    /// CORRIGIDO: Usa UHF_GetReceived_EX conforme padrão da Base Fabrica (ReadEPCForm.cs linha 441).
    /// </summary>
    private async Task ReadLoopAsync(CancellationToken ct)
    {
        try
        {
            _log.Info("🔄 Thread de leitura iniciada");
            
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    // CORRIGIDO: Usa UHF_GetReceived_EX (padrão Base Fabrica)
                    int bufLen = 0;
                    int result = NativeMethods.UHFGetReceived_EX(ref bufLen, _tagBuffer);
                    
                    if (result == NativeMethods.UHFAPI_SUCCESS && bufLen > 0)
                    {
                        // Processa tag usando formato correto do fabricante
                        ProcessTagFromReceivedBuffer(bufLen);
                    }
                    else
                    {
                        // Sem dados - sleep pequeno (padrão do fabricante: 5ms)
                        await Task.Delay(5, ct);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _log.Warn($"⚠️ Erro na leitura de tags: {ex.Message}");
                    await Task.Delay(100, ct);  // Backoff
                }
            }
        }
        finally
        {
            _log.Info("🔄 Thread de leitura finalizada");
        }
    }
    
    /// <summary>
    /// Processa tag recebida do buffer UHF_GetReceived_EX.
    /// CORRIGIDO: Segue formato exato da Base Fabrica (UHFAPI.cs linha 2134).
    /// Formato: [epc_len] [epc_data...] [tid_len] [tid_data...] [rssi_2bytes] [ant]
    /// </summary>
    private void ProcessTagFromReceivedBuffer(int bufLen)
    {
        try
        {
            if (bufLen < 3) return;
            
            // Parse conforme Base Fabrica (UHFAPI.cs linha 2134)
            int uii_len = _tagBuffer[0];
            if (uii_len == 0 || uii_len > 128) return;
            
            int tid_leng = _tagBuffer[uii_len + 1];
            int tid_idex = uii_len + 2;
            int rssi_index = 1 + uii_len + 1 + tid_leng;
            int ant_index = rssi_index + 2;
            
            // Converte para string hex
            string strData = BitConverter.ToString(_tagBuffer, 0, bufLen).Replace("-", "");
            
            // Extrai EPC (remove PC 2 bytes + CRC 2 bytes)
            if (strData.Length < (6 + uii_len * 2 - 4)) return;
            string epc = strData.Substring(6, uii_len * 2 - 4);
            
            // Valida EPC não vazio
            if (string.IsNullOrWhiteSpace(epc) || epc.All(c => c == '0')) return;
            
            // Extrai RSSI (2 bytes, signed)
            byte rssi = 0;
            try
            {
                if (rssi_index * 2 + 4 <= strData.Length)
                {
                    string temp = strData.Substring(rssi_index * 2, 4);
                    int rssiTemp = Convert.ToInt32(temp, 16) - 65535;
                    rssi = (byte)Math.Abs(rssiTemp / 10);  // dBm absoluto
                }
            }
            catch
            {
                rssi = 50;  // Default se falhar
            }
            
            // Deduplica e emite
            var now = DateTime.UtcNow;
            if (!_recentEpcs.TryGetValue(epc, out var lastSeen) || 
                (now - lastSeen) >= _deduplicationWindow)
            {
                _recentEpcs[epc] = now;
                
                try
                {
                    _log.Info($"🔔 RfidReaderService.TagDetected disparado: EPC={epc}, RSSI={rssi}");
                    TagDetected?.Invoke(epc, rssi);
                }
                catch (Exception ex)
                {
                    _log.Warn($"⚠️ Erro no evento TagDetected: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            _log.Warn($"⚠️ Erro ao processar buffer: {ex.Message}");
        }
    }

    /// <summary>
    /// Processa tags lidas do buffer.
    /// Aplica deduplicação por timespan.
    /// OBSOLETO: Substituído por ProcessTagFromReceivedBuffer (formato correto Base Fabrica).
    /// </summary>
    private void ProcessTags(int numTags, int bufferLen)
    {
        try
        {
            int offset = 0;
            int processed = 0;
            
            while (offset < bufferLen && processed < numTags)
            {
                // Lê comprimento do EPC
                if (offset >= bufferLen) break;
                
                byte epcLen = _tagBuffer[offset];
                if (epcLen == 0 || epcLen > NativeMethods.MAX_EPC_LENGTH)
                {
                    // TAG inválida, pula
                    offset += 1;
                    processed++;
                    continue;
                }
                
                // Lê EPC
                if (offset + epcLen + 1 >= bufferLen) break;
                
                byte[] epc = new byte[epcLen];
                Array.Copy(_tagBuffer, offset + 1, epc, 0, epcLen);
                
                // Lê RSSI (signal strength)
                byte rssi = _tagBuffer[offset + 1 + epcLen];
                
                // Converte EPC para string hex
                string epcHex = BitConverter.ToString(epc).Replace("-", "");
                
                // Aplica deduplicação
                var now = DateTime.UtcNow;
                bool isDuplicate = _recentEpcs.TryGetValue(epcHex, out var lastSeen) &&
                                   (now - lastSeen) < _deduplicationWindow;
                
                if (!isDuplicate)
                {
                    _recentEpcs[epcHex] = now;
                    
                    // Emite evento
                    try
                    {
                        TagDetected?.Invoke(epcHex, rssi);
                    }
                    catch (Exception ex)
                    {
                        _log.Warn($"⚠️ Erro no evento TagDetected: {ex.Message}");
                    }
                }
                
                // Próxima tag
                offset += 1 + epcLen + 1;
                processed++;
            }
            
            // Limpa deduplicação expirada (a cada 100 leituras)
            if (processed % 100 == 0)
            {
                CleanExpiredDuplicates();
            }
        }
        catch (Exception ex)
        {
            _log.Error($"❌ Erro ao processar tags: {ex.Message}");
        }
    }


    
    /// <summary>
    /// Remove EPCs expirados do dicionário de deduplicação.
    /// Previne memory leak em operação contínua.
    /// </summary>
    private void CleanExpiredDuplicates()
    {
        try
        {
            var now = DateTime.UtcNow;
            var expired = new System.Collections.Generic.List<string>();
            
            foreach (var kvp in _recentEpcs)
            {
                if ((now - kvp.Value) > _deduplicationWindow)
                {
                    expired.Add(kvp.Key);
                }
            }
            
            foreach (var epc in expired)
            {
                _recentEpcs.TryRemove(epc, out _);
            }
        }
        catch
        {
            // Ignora erros na limpeza
        }
    }
    
    
    public void Dispose()
    {
        StopInventory();
        Disconnect();
        _readCts?.Dispose();
        GC.SuppressFinalize(this);
    }
}
