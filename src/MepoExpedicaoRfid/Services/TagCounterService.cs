using System.Collections.Concurrent;
using MepoExpedicaoRfid.Models;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// Serviço de contagem de tags por SKU/Lote com thread-safe
/// </summary>
public sealed class TagCounterService
{
    private readonly ConcurrentDictionary<string, TagCountInfo> _counts = new();
    private readonly ConcurrentDictionary<string, DateTime> _lastSeen = new();
    private readonly AppLogger _log;

    public int TotalUniqueEpcs => _lastSeen.Count;
    public int TotalSkuLoteGroups => _counts.Count;

    public event EventHandler? CountsUpdated;

    public TagCounterService(AppLogger log)
    {
        _log = log;
    }

    public void AddTag(string epc, string sku, string lote)
    {
        if (string.IsNullOrWhiteSpace(epc) || string.IsNullOrWhiteSpace(sku))
        {
            _log.Warn("[TagCounter] Invalid EPC or SKU");
            return;
        }

        // Dedupe por EPC
        _lastSeen.TryAdd(epc, DateTime.UtcNow);

        // Contador por SKU/Lote
        var key = $"{sku}|{lote ?? "SEM_LOTE"}";
        _counts.AddOrUpdate(key, 
            _ => new TagCountInfo { Sku = sku, Lote = lote, Count = 1, LastUpdate = DateTime.UtcNow },
            (_, existing) => 
            {
                existing.Count++;
                existing.LastUpdate = DateTime.UtcNow;
                return existing;
            });

        CountsUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void Clear()
    {
        _counts.Clear();
        _lastSeen.Clear();
        _log.Info("[TagCounter] Cleared all counts");
        CountsUpdated?.Invoke(this, EventArgs.Empty);
    }

    public IReadOnlyList<TagCountInfo> GetTopGroups(int limit = 10)
    {
        return _counts.Values
            .OrderByDescending(x => x.Count)
            .Take(limit)
            .ToList();
    }

    public IReadOnlyList<SkuLoteGroupInfo> GetAllGroups()
    {
        return _counts.Values
            .Select(x => new SkuLoteGroupInfo 
            { 
                Sku = x.Sku, 
                Lote = x.Lote ?? "", 
                Quantidade = x.Count 
            })
            .OrderBy(x => x.Sku)
            .ThenBy(x => x.Lote)
            .ToList();
    }

    public int GetCountForSkuLote(string sku, string? lote)
    {
        var key = $"{sku}|{lote ?? "SEM_LOTE"}";
        return _counts.TryGetValue(key, out var info) ? info.Count : 0;
    }

    public void RemoveTags(IEnumerable<string> epcs)
    {
        foreach (var epc in epcs)
        {
            _lastSeen.TryRemove(epc, out _);
        }
        _log.Info($"[TagCounter] Removed {epcs.Count()} EPCs");
    }
}

public sealed class TagCountInfo
{
    public string Sku { get; set; } = "";
    public string? Lote { get; set; }
    public int Count { get; set; }
    public DateTime LastUpdate { get; set; }
}
