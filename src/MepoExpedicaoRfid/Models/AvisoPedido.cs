using System.Text.Json.Serialization;

namespace MepoExpedicaoRfid.Models;

public sealed class AvisoPedido
{
    [JsonPropertyName("numero")]
    public string Numero { get; set; } = "";

    [JsonPropertyName("origem")]
    public string Origem { get; set; } = "";

    [JsonPropertyName("cliente_nome")]
    public string ClienteNome { get; set; } = "";

    [JsonPropertyName("observacao")]
    public string Observacao { get; set; } = "";

    // "sem_nf" | "observacao" | ""
    [JsonPropertyName("aviso_tipo")]
    public string AvisoTipo { get; set; } = "";

    [JsonPropertyName("enviado_por")]
    public string EnviadoPor { get; set; } = "";

    [JsonPropertyName("enviado_at")]
    public DateTime? EnviadoAt { get; set; }

    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = "";

    public bool IsSemNf => string.Equals(AvisoTipo, "sem_nf", StringComparison.OrdinalIgnoreCase)
                           || (Observacao?.Contains("SEM NF", StringComparison.OrdinalIgnoreCase) ?? false);
}
