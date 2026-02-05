namespace MepoExpedicaoRfid.Services;

public interface IRfidReader
{
    string Name { get; }
    bool IsConnected { get; }
    
    // PARTE I: Novos contratos para estado do reader
    bool IsInventoryRunning { get; }
    
    event EventHandler<RfidTagReadEventArgs>? TagRead;

    Task ConnectAsync(CancellationToken ct);
    Task DisconnectAsync(CancellationToken ct);

    Task StartReadingAsync(CancellationToken ct);
    Task StopReadingAsync(CancellationToken ct);

    Task SetPowerAsync(int power, CancellationToken ct);

    /// <summary>
    /// Lê uma ÚNICA tag e fecha automaticamente.
    /// IMPORTANTE: Apenas funciona em modo R3Dll (hardware real), não em modo simulado.
    /// Retorna o EPC da tag ou null se timeout.
    /// </summary>
    Task<string?> ReadSingleTagAsync(CancellationToken ct);

    /// <summary>
    /// Consulta tag com leitura única (InventorySingle).
    /// Retorna o EPC da tag ou null se timeout.
    /// </summary>
    Task<string?> ConsultarTagAsync(TimeSpan timeout, CancellationToken ct);
}

public sealed class RfidTagReadEventArgs : EventArgs
{
    public required string Epc { get; init; }
    public double? Rssi { get; init; }
    public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;
}
