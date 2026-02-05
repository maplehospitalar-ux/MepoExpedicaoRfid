using System.Text.Json.Serialization;

namespace MepoExpedicaoRfid.Models;

/// <summary>
/// Representa uma tag RFID lida
/// </summary>
public class TagItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("epc")]
    public string Epc { get; set; } = string.Empty;

    [JsonPropertyName("sku")]
    public string? Sku { get; set; }

    [JsonPropertyName("lote")]
    public string? Lote { get; set; }

    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }

    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Tipo da sessão para evitar detecção por string
    /// </summary>
    [JsonPropertyName("tipo")]
    public SessionType Tipo { get; set; } = SessionType.Saida;

    [JsonPropertyName("entrada_id")]
    public string? EntradaId { get; set; }

    [JsonPropertyName("venda_numero")]
    public string? VendaNumero { get; set; }

    [JsonPropertyName("origem")]
    public string? Origem { get; set; }

    [JsonPropertyName("data_fabricacao")]
    public DateTime? DataFabricacao { get; set; }

    [JsonPropertyName("data_validade")]
    public DateTime? DataValidade { get; set; }

    /// <summary>
    /// Status anterior da tag no estoque (era StatusOriginal)
    /// </summary>
    [JsonPropertyName("status_anterior")]
    public string? StatusAnterior { get; set; }

    /// <summary>
    /// Status atual da tag (era StatusNovo)
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = "lida";

    // ❌ REMOVIDO: Cmc (não existe na tabela rfid_saidas_audit)

    [JsonPropertyName("rssi")]
    public int Rssi { get; set; }

    [JsonPropertyName("lida_em")]
    public DateTime LidaEm { get; set; } = DateTime.UtcNow;

    public bool Processada { get; set; } = false;
    public string? ErroMensagem { get; set; }
    
    /// <summary>
    /// Chave de idempotência para evitar duplicatas
    /// Formato: {SESSION_ID}:{EPC} (ordem correta!)
    /// </summary>
    public string IdempotencyKey => $"{SessionId}:{Epc}";
    
    /// <summary>
    /// Indica se a tag é válida (tem SKU e Lote)
    /// </summary>
    public bool IsValida => !string.IsNullOrEmpty(Sku) && !string.IsNullOrEmpty(Lote);
    
    /// <summary>
    /// Status visual para UI
    /// </summary>
    public TagStatus StatusVisual
    {
        get
        {
            if (string.IsNullOrEmpty(Sku)) return TagStatus.Desconhecida;
            if (!string.IsNullOrEmpty(ErroMensagem)) return TagStatus.Invalida;
            return TagStatus.Valida;
        }
    }
}

public enum TagStatus
{
    Valida,
    Invalida,
    Desconhecida,
    Excedente
}
