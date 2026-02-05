using System.Threading;

namespace MepoExpedicaoRfid.Services;

public sealed class SimulatedRfidReader : IRfidReader
{
    private readonly RfidConfig _cfg;
    private readonly AppLogger _log;
    private CancellationTokenSource? _loopCts;
    private readonly Random _rng = new();
    private bool _inventoryRunning = false;

    public SimulatedRfidReader(RfidConfig cfg, AppLogger log)
    {
        _cfg = cfg;
        _log = log;
    }

    public string Name => "Simulated Reader";
    public bool IsConnected { get; private set; }
    
    // PARTE I: Implementa IsInventoryRunning
    public bool IsInventoryRunning => _inventoryRunning;

    public event EventHandler<RfidTagReadEventArgs>? TagRead;

    public Task ConnectAsync(CancellationToken ct)
    {
        IsConnected = true;
        _log.Info("✅ Simulated reader conectado.");
        return Task.CompletedTask;
    }

    public Task DisconnectAsync(CancellationToken ct)
    {
        IsConnected = false;
        _loopCts?.Cancel();
        _log.Info("⛔ Simulated reader desconectado.");
        return Task.CompletedTask;
    }

    public Task SetPowerAsync(int power, CancellationToken ct)
    {
        _log.Info($"⚙️ (Sim) Power set: {power}");
        return Task.CompletedTask;
    }

    public Task StartReadingAsync(CancellationToken ct)
    {
        if (!IsConnected) throw new InvalidOperationException("Reader não conectado.");
        _loopCts?.Cancel();
        _loopCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        
        _inventoryRunning = true;

        _ = Task.Run(async () =>
        {
            _log.Info("▶️ (Sim) Leitura iniciada.");
            try
            {
                while (!_loopCts!.IsCancellationRequested)
                {
                    // generate some EPCs with repeats
                    var epc = "E200" + _rng.NextInt64(10_000_000_000, 99_999_999_999).ToString();
                    TagRead?.Invoke(this, new RfidTagReadEventArgs { Epc = epc, Rssi = -35 - _rng.NextDouble() * 25 });
                    await Task.Delay(_rng.Next(5, 25), _loopCts.Token).ContinueWith(_ => { });
                }
            }
            finally
            {
                _inventoryRunning = false;
            }
        }, _loopCts.Token);

        return Task.CompletedTask;
    }

    public Task StopReadingAsync(CancellationToken ct)
    {
        _loopCts?.Cancel();
        _inventoryRunning = false;
        _log.Info("⏸️ (Sim) Leitura parada.");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Lê UMA ÚNICA tag - NÃO SUPORTADO em modo simulado.
    /// Retorna null e log de erro.
    /// </summary>
    public Task<string?> ReadSingleTagAsync(CancellationToken ct)
    {
        _log.Error("❌ Leitura de tag única NÃO é suportada em modo simulado. Use apenas em modo R3Dll (hardware real).");
        return Task.FromResult<string?>(null);
    }

    public Task<string?> ConsultarTagAsync(TimeSpan timeout, CancellationToken ct)
    {
        _log.Error("❌ Consulta de tag (InventorySingle) NÃO é suportada em modo simulado. Use apenas em modo R3Dll (hardware real).");
        return Task.FromResult<string?>(null);
    }
}