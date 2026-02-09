using System.Text.Json;
using System.Text.Json.Serialization;

namespace MepoExpedicaoRfid.Models;

public sealed class PedidoPrintPayload
{
    [JsonPropertyName("documento_id")] public Guid DocumentoId { get; set; }
    [JsonPropertyName("numero")] public string? Numero { get; set; }
    [JsonPropertyName("origem")] public string? Origem { get; set; }
    [JsonPropertyName("cliente_nome")] public string? ClienteNome { get; set; }
    [JsonPropertyName("valor_total")] public decimal? ValorTotal { get; set; }
    [JsonPropertyName("is_sem_nf")] public bool? IsSemNf { get; set; }
    [JsonPropertyName("observacao_expedicao")] public string? ObservacaoExpedicao { get; set; }
    [JsonPropertyName("status_expedicao")] public string? StatusExpedicao { get; set; }

    // pode vir como array json
    [JsonPropertyName("itens")] public JsonElement Itens { get; set; }
}
