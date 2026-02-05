namespace MepoExpedicaoRfid.Models;

/// <summary>
/// Informações de um pedido na fila
/// </summary>
public class PedidoInfo
{
    public string Id { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Origem { get; set; } = "OMIE";
    public string ClienteNome { get; set; } = string.Empty;
    public string? ClienteDocumento { get; set; }
    public int TotalItens { get; set; }
    public decimal ValorTotal { get; set; }
    public PedidoStatus Status { get; set; } = PedidoStatus.Pendente;
    public bool Urgente { get; set; }
    public int Prioridade { get; set; }
    public string? OperadorNome { get; set; }
    public string? SessionId { get; set; }
    public DateTime? IniciadoEm { get; set; }
    public DateTime? FinalizadoEm { get; set; }
    public DateTime CriadoEm { get; set; }
    
    /// <summary>
    /// Tempo em processamento (se iniciado)
    /// </summary>
    public TimeSpan? TempoProcessamento =>
        IniciadoEm.HasValue
            ? (FinalizadoEm ?? DateTime.UtcNow) - IniciadoEm.Value
            : null;
    
    /// <summary>
    /// String formatada do tempo
    /// </summary>
    public string? TempoFormatado =>
        TempoProcessamento?.ToString(@"hh\:mm\:ss");
}

public enum PedidoStatus
{
    Pendente,
    EmSeparacao,
    Finalizado,
    Cancelado
}
