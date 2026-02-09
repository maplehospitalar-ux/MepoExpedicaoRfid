namespace MepoExpedicaoRfid.Models;

public sealed class DocumentoItemResumo
{
    public string? Sku { get; set; }
    public string? Descricao { get; set; }
    public decimal Quantidade { get; set; }
    public decimal? PrecoUnitario { get; set; }
    public decimal? ValorTotal { get; set; }
}
