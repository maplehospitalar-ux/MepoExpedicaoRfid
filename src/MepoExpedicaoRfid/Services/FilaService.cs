using System.Collections.ObjectModel;
using MepoExpedicaoRfid.Models;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// FILA: neste projeto, carregamos via VIEW v_fila_expedicao.
/// Realtime: você pode usar Supabase Realtime SDK se preferir, mas a base já fica funcional só com refresh manual.
/// </summary>
public sealed class FilaService
{
    public async Task<string?> BuildPrintTextAsync(Guid documentoId)
    {
        try
        {
            // Preferir o payload pronto (cabecalho + itens) para impressão.
            var rows = await _supabase.GetFilaAsync(new[] { "na_fila", "preparando", "processando", "finalizada" }, 300);
            var row = rows.FirstOrDefault(r => r.Id == documentoId);

            var origem = (row?.Origem ?? "").Trim().ToUpperInvariant();
            var numero = (row?.NumeroPedido ?? "").Trim();

            if (string.IsNullOrWhiteSpace(origem) || string.IsNullOrWhiteSpace(numero))
                return null;

            var payload = await _supabase.GetPedidoPrintPayloadAsync(origem, numero);
            if (payload is null) return null;

            static string Cut(string? s, int max)
            {
                if (string.IsNullOrWhiteSpace(s)) return "";
                s = s.Trim();
                if (s.Length <= max) return s;
                return s.Substring(0, Math.Max(0, max - 1)) + "…";
            }

            static string PadRight(string s, int len)
            {
                s ??= "";
                return s.Length >= len ? s.Substring(0, len) : s.PadRight(len);
            }

            static string PadLeft(string s, int len)
            {
                s ??= "";
                return s.Length >= len ? s.Substring(0, len) : s.PadLeft(len);
            }

            var sb = new System.Text.StringBuilder();

            // Layout otimizado para térmica (Elgin i9) - 32-42 colunas (depende do driver)
            sb.AppendLine("MEPO - GUIA DE SEPARACAO");
            sb.AppendLine(new string('=', 32));
            sb.AppendLine($"PEDIDO: {payload.Numero}");
            sb.AppendLine($"ORIGEM: {payload.Origem}");
            if (!string.IsNullOrWhiteSpace(payload.ClienteNome)) sb.AppendLine($"CLIENTE: {Cut(payload.ClienteNome, 28)}");
            if (payload.IsSemNf == true) sb.AppendLine("*** PEDIDO SEM NF ***");
            if (!string.IsNullOrWhiteSpace(payload.ObservacaoExpedicao))
            {
                sb.AppendLine(new string('-', 32));
                sb.AppendLine(Cut(payload.ObservacaoExpedicao, 96));
            }

            sb.AppendLine(new string('-', 32));
            sb.AppendLine($"{PadRight("SKU", 12)} {PadLeft("QTD", 4)} {PadRight("DESC", 13)}");
            sb.AppendLine(new string('-', 32));

            if (payload.Itens.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                foreach (var it in payload.Itens.EnumerateArray())
                {
                    var sku = it.TryGetProperty("sku", out var s) ? (s.GetString() ?? "") : "";
                    var desc = it.TryGetProperty("descricao", out var d) ? (d.GetString() ?? "") : "";
                    var qtd = it.TryGetProperty("quantidade", out var q) ? q.ToString() : "";

                    sb.AppendLine($"{PadRight(Cut(sku, 12), 12)} {PadLeft(Cut(qtd, 4), 4)} {PadRight(Cut(desc, 13), 13)}");
                }
            }

            sb.AppendLine(new string('-', 32));
            sb.AppendLine("INICIO: ____/____ ____:____");
            sb.AppendLine("FIM:    ____/____ ____:____");
            sb.AppendLine("RFID:   OK ( )  DIVERG ( )");
            sb.AppendLine(new string('=', 32));

            return sb.ToString();
        }
        catch (Exception ex)
        {
            _log.Warn($"Falha ao montar texto de impressão do pedido {documentoId}: {ex.Message}");
            return null;
        }
    }
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
            var st = (r.Status ?? "").Trim().ToLowerInvariant();

            // Regra prática (mais fiel ao chão de fábrica):
            // - finalizada => Finalizados
            // - se existe sessão ativa (SessionId) OU status processando/em_separacao => Em separação
            // - caso contrário => Pendentes
            if (st is "finalizada" or "finalizado")
            {
                Finalizados.Add(r);
                continue;
            }

            var hasSession = !string.IsNullOrWhiteSpace(r.SessionId);
            if (hasSession || st is "processando" or "em_separacao" or "em separacao" or "separacao")
            {
                EmSeparacao.Add(r);
                continue;
            }

            // 'na_fila' / 'preparando' e qualquer outro vira pendente
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
