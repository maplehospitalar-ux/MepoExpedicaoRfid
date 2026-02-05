namespace MepoExpedicaoRfid.Models;

public sealed class SkuLoteGroupInfo
{
    public string Sku { get; set; } = "DESCONHECIDO";
    public string Lote { get; set; } = "SEM_LOTE";
    public int Quantidade { get; set; }
}
