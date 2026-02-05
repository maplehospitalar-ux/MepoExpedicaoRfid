using System.Text.Json.Serialization;

namespace MepoExpedicaoRfid.Models;

public sealed class FilaItem
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }

    [JsonPropertyName("numero_pedido")]
    public string? NumeroPedido { get; set; }

    [JsonPropertyName("cliente")]
    public string? Cliente { get; set; }

    [JsonPropertyName("total_itens")]
    public int TotalItens { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("criado_em")]
    public DateTime CriadoEm { get; set; }

    [JsonPropertyName("iniciado_em")]
    public DateTime? IniciadoEm { get; set; }

    [JsonPropertyName("finalizado_em")]
    public DateTime? FinalizadoEm { get; set; }

    [JsonPropertyName("prioridade")]
    public int Prioridade { get; set; }

    [JsonPropertyName("tags_lidas")]
    public int TagsLidas { get; set; }

    [JsonPropertyName("origem")]
    public string? Origem { get; set; }
}
