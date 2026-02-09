using System.Collections.ObjectModel;
using MepoExpedicaoRfid.Models;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// FILA: neste projeto, carregamos via VIEW v_fila_expedicao.
/// Realtime: você pode usar Supabase Realtime SDK se preferir, mas a base já fica funcional só com refresh manual.
/// </summary>
public sealed class FilaService
{
    private readonly SupabaseService _supabase;
    private readonly AppLogger _log;

    public ObservableCollection<FilaItem> Pendentes { get; } = new();
    public ObservableCollection<FilaItem> EmSeparacao { get; } = new();
    public ObservableCollection<FilaItem> Finalizados { get; } = new();

    public FilaService(SupabaseService supabase, AppLogger log)
    {
        _supabase = supabase;
        _log = log;
    }

    public async Task RefreshAsync()
    {
        var rows = await _supabase.GetFilaAsync(new[] { "na_fila", "preparando", "processando", "finalizada" }, 300);

        // Dedupe: a view pode retornar o mesmo pedido 2x (ex.: documento + sessão ativa via UNION ALL).
        // Mantém o mais útil para o operador:
        // 1) preferir linha com SessionId preenchido
        // 2) senão, a mais recente por IniciadoEm/CriadoEm
        var deduped = rows
            .GroupBy(r => $"{(r.Origem ?? "").Trim().ToUpperInvariant()}|{(r.NumeroPedido ?? "").Trim()}" )
            .Select(g =>
            {
                var withSession = g.Where(x => !string.IsNullOrWhiteSpace(x.SessionId)).ToList();
                if (withSession.Count > 0)
                    return withSession.OrderByDescending(x => x.IniciadoEm ?? x.CriadoEm).First();

                return g.OrderByDescending(x => x.IniciadoEm ?? x.CriadoEm).First();
            })
            .ToList();

        Apply(deduped);
    }

    private void Apply(IReadOnlyList<FilaItem> rows)
    {
        Pendentes.Clear();
        EmSeparacao.Clear();
        Finalizados.Clear();

        foreach (var r in rows)
        {
            var st = (r.Status ?? "").ToLowerInvariant();

            if (st is "finalizada" or "finalizado")
                Finalizados.Add(r);
            else if (st is "processando" or "separacao" or "em_separacao")
                EmSeparacao.Add(r);
            else
                Pendentes.Add(r);
        }
    }

    /// <summary>
    /// Mapeia status do banco (PT) para exibição (EN) se necessário
    /// </summary>
    public static string MapStatusParaExibicao(string statusBanco) => statusBanco switch
    {
        "na_fila" => "Na Fila",
        "preparando" => "Preparando",
        "processando" => "Em Processo",
        "finalizada" => "Finalizada",
        "cancelada" => "Cancelada",
        "expirada" => "Expirada",
        _ => statusBanco
    };
}
