using System.Text.Json.Serialization;

namespace MepoExpedicaoRfid.Models;

public sealed class TagCurrent
{
    public string Epc { get; set; } = "";
    public string? Sku { get; set; }
    public string? Descricao { get; set; }
    public string? Lote { get; set; }
    public string? Status { get; set; }
    public string? Local { get; set; }
    
    [JsonPropertyName("manufacture_date")]
    public DateTime? DataFabricacao { get; set; }
    
    [JsonPropertyName("expiration_date")]
    public DateTime? DataValidade { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
