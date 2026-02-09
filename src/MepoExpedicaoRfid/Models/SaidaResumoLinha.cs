namespace MepoExpedicaoRfid.Models;

public sealed class SaidaResumoLinha
{
    public string Sku { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Lote { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}
