using MepoExpedicaoRfid.Models;

namespace MepoExpedicaoRfid.Services;

public sealed class TagHistoryService
{
    private readonly SupabaseService _supabase;
    private readonly AppLogger _log;

    public TagHistoryService(SupabaseService supabase, AppLogger log)
    {
        _supabase = supabase;
        _log = log;
    }

    public Task<TagHistoricoDto> GetAsync(string epc, int limit = 200)
        => _supabase.GetTagHistoricoAsync(epc, limit);
}
