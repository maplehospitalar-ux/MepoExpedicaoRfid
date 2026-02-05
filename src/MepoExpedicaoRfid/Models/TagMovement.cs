using System.Text.Json.Serialization;

namespace MepoExpedicaoRfid.Models;

public sealed class TagMovement
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("epc")]
    public string Epc { get; set; } = "";
    
    [JsonPropertyName("tipo")]
    public string Tipo { get; set; } = ""; // entrada | saida | movimento
    
    [JsonPropertyName("sku")]
    public string? Sku { get; set; }
    
    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }
    
    [JsonPropertyName("lote")]
    public string? Lote { get; set; }
    
    [JsonPropertyName("numero_pedido")]
    public string? NumeroPedido { get; set; }
    
    [JsonPropertyName("operador")]
    public string? Operador { get; set; }
    
    [JsonPropertyName("local")]
    public string? Local { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}
