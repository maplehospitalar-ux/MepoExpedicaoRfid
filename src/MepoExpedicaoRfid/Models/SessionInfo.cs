namespace MepoExpedicaoRfid.Models;

/// <summary>
/// Informações de uma sessão RFID
/// </summary>
public class SessionInfo
{
    public string SessionId { get; set; } = string.Empty;
    public SessionType Tipo { get; set; }
    public SessionStatus Status { get; set; } = SessionStatus.Pendente;
    public string? Origem { get; set; }
    public string? VendaNumero { get; set; }
    public string? ClienteNome { get; set; }
    public string? Sku { get; set; }
    public string? Lote { get; set; }
    public string? EntradaId { get; set; }
    public DateTime? DataFabricacao { get; set; }
    public DateTime? DataValidade { get; set; }
    public string ReaderId { get; set; } = "r3-desktop-02";
    public string ClientType { get; set; } = "desktop_csharp";
    public DateTime IniciadaEm { get; set; } = DateTime.UtcNow;
    public DateTime? FinalizadaEm { get; set; }
    public int TotalTagsLidas { get; set; }
    public int TotalTagsValidas { get; set; }
    public int TotalTagsInvalidas { get; set; }
    
    /// <summary>
    /// Tempo decorrido desde o início
    /// </summary>
    public TimeSpan TempoDecorrido => 
        (FinalizadaEm ?? DateTime.UtcNow) - IniciadaEm;
    
    /// <summary>
    /// String formatada do tempo
    /// </summary>
    public string TempoFormatado => 
        TempoDecorrido.ToString(@"hh\:mm\:ss");
}

public enum SessionType
{
    Saida,
    Entrada,
    Inventario
}

public enum SessionStatus
{
    Pendente,
    Ativa,
    Pausada,
    Finalizada,
    Cancelada
}
