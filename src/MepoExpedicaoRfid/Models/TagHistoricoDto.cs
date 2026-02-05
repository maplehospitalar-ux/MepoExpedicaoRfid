namespace MepoExpedicaoRfid.Models;

public sealed class TagHistoricoDto
{
    public TagCurrent? Current { get; set; }
    public IReadOnlyList<TagMovement> Movimentos { get; set; } = Array.Empty<TagMovement>();
}
