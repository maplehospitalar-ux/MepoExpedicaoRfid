namespace MepoExpedicaoRfid.Models;

public sealed class TagConflict
{
    public string Epc { get; set; } = "";

    public string? ExistingSku { get; set; }
    public string? ExistingLote { get; set; }
    public DateTime? ExistingFabricacao { get; set; }
    public DateTime? ExistingValidade { get; set; }

    public string? NewSku { get; set; }
    public string? NewLote { get; set; }
    public DateTime? NewFabricacao { get; set; }
    public DateTime? NewValidade { get; set; }

    public bool IsDifferent =>
        !string.Equals(ExistingSku ?? "", NewSku ?? "", StringComparison.OrdinalIgnoreCase) ||
        !string.Equals(ExistingLote ?? "", NewLote ?? "", StringComparison.OrdinalIgnoreCase) ||
        ExistingFabricacao != NewFabricacao ||
        ExistingValidade != NewValidade;
}
