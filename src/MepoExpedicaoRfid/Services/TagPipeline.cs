using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Windows.Threading;
using MepoExpedicaoRfid.Models;

namespace MepoExpedicaoRfid.Services;

public sealed class TagPipeline
{
    private readonly IRfidReader _reader;
    private readonly RfidConfig _cfg;
    private readonly AppLogger _log;
    private readonly SessionStateManager _session;
    private readonly BatchTagInsertService _batch;
    private readonly RealtimeService _realtime;
    private readonly SupabaseService _supabase;

    // Enriquecimento (saída): evita ficar DESCONHECIDO na UI
    private readonly ConcurrentDictionary<string, byte> _enrichInFlight = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, (string sku, string lote)> _countedAs = new(StringComparer.OrdinalIgnoreCase);

    private readonly Channel<RfidTagReadEventArgs> _ch = Channel.CreateUnbounded<RfidTagReadEventArgs>(
        new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

    private readonly ConcurrentDictionary<string, DateTime> _lastSeen = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, (string sku, string lote)> _tagMeta = new(StringComparer.OrdinalIgnoreCase);

    private readonly ConcurrentDictionary<(string sku, string lote), int> _counts = new();
    private readonly ConcurrentQueue<string> _recent = new();
    private readonly object _recentLock = new();

    private CancellationTokenSource? _cts;

    public event EventHandler? SnapshotUpdated;

    public int TotalUniqueTags => _lastSeen.Count;
    public IReadOnlyList<string> RecentTags
    {
        get
        {
            lock (_recentLock)
            {
                var arr = _recent.ToArray();
                return arr.Skip(Math.Max(0, arr.Length - 10)).ToArray();
            }
        }
    }
    public IReadOnlyList<SkuLoteGroupInfo> Groups =>
        _counts.Select(kvp => new SkuLoteGroupInfo { Sku = kvp.Key.sku, Lote = kvp.Key.lote, Quantidade = kvp.Value })
               .OrderBy(x => x.Sku).ThenBy(x => x.Lote).ToList();

    public TagPipeline(IRfidReader reader, RfidConfig cfg, AppLogger log, SessionStateManager session, BatchTagInsertService batch, RealtimeService realtime, SupabaseService supabase)
    {
        _reader = reader;
        _cfg = cfg;
        _log = log;
        _session = session;
        _batch = batch;
        _realtime = realtime;
        _supabase = supabase;

        _reader.TagRead += (_, e) =>
        {
            // HOT PATH: evitar log por tag (vira gargalo e trava o desktop)
            _ch.Writer.TryWrite(e);
        };
    }

    public async Task StartAsync()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        
        // ✅ Só conecta se não estiver conectado (StatusViewModel pode já ter conectado)
        if (!_reader.IsConnected)
        {
            await _reader.ConnectAsync(_cts.Token);
            await _reader.SetPowerAsync(_cfg.Power, _cts.Token);
        }

        _ = Task.Run(() => ProcessorLoop(_cts.Token));
    }

    public async Task StopAsync()
    {
        if (_cts is null) return;
        await _reader.StopReadingAsync(_cts.Token);
        await _reader.DisconnectAsync(_cts.Token);
        _cts.Cancel();
    }

    public Task BeginReadingAsync()
        => _reader.StartReadingAsync(_cts?.Token ?? CancellationToken.None);

    public Task EndReadingAsync()
        => _reader.StopReadingAsync(_cts?.Token ?? CancellationToken.None);

    /// <summary>
    /// Força envio imediato das tags pendentes para o MEPO (batch).
    /// Importante chamar antes de finalizar uma sessão, para evitar finalizar com 0 tags no backend.
    /// </summary>
    public Task FlushPendingAsync()
        => _batch.FlushAsync();

    /// <summary>
    /// Lê UMA ÚNICA tag e fecha automaticamente.
    /// IMPORTANTE: Apenas funciona em modo R3Dll (hardware real), não em modo simulado.
    /// </summary>
    public Task<string?> ReadSingleTagAsync()
        => _reader.ReadSingleTagAsync(_cts?.Token ?? CancellationToken.None);

    /// <summary>
    /// Consulta tag com leitura única (InventorySingle).
    /// </summary>
    public Task<string?> ConsultarTagAsync(TimeSpan timeout)
        => _reader.ConsultarTagAsync(timeout, _cts?.Token ?? CancellationToken.None);

    public void ResetSessionCounters()
    {
        _lastSeen.Clear();
        _tagMeta.Clear();
        _counts.Clear();
        _countedAs.Clear();
        _enrichInFlight.Clear();
        while (_recent.TryDequeue(out _)) { }
        SnapshotUpdated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Se você já tem enriquecimento de EPC -> (sku/lote) vindo do Supabase,
    /// chame isso para melhorar contadores em tempo real.
    /// </summary>
    public void UpsertTagMeta(string epc, string? sku, string? lote)
    {
        var k = NormalizeEpc(epc);
        _tagMeta[k] = (sku ?? "DESCONHECIDO", lote ?? "SEM_LOTE");
    }

    private async Task ProcessorLoop(CancellationToken ct)
    {
        var lastUpdate = DateTime.UtcNow;
        var updateInterval = TimeSpan.FromMilliseconds(_cfg.UiUpdateMs);

        try
        {
            while (!ct.IsCancellationRequested)
            {
                // ✅ CORREÇÃO: Aguarda até que haja uma tag OU até o tempo de UI update
                var readTask = _ch.Reader.ReadAsync(ct).AsTask();
                var timerTask = Task.Delay(updateInterval, ct);
                var completedTask = await Task.WhenAny(readTask, timerTask);
                
                // Se uma tag chegou, processa ela e todas as outras disponíveis
                if (completedTask == readTask)
                {
                    var ev = await readTask;
                    ProcessTag(ev);
                    
                    // Processa todas as tags que estão disponíveis AGORA
                    while (_ch.Reader.TryRead(out var extraTag))
                    {
                        ProcessTag(extraTag);
                    }
                }
                
                // Atualiza UI a cada intervalo
                if ((DateTime.UtcNow - lastUpdate) >= updateInterval)
                {
                    _log.Debug($"[TagPipeline] SnapshotUpdated Total={_lastSeen.Count}, Recent={_recent.Count}");
                    SnapshotUpdated?.Invoke(this, EventArgs.Empty);
                    lastUpdate = DateTime.UtcNow;
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            _log.Error("Erro no TagPipeline.ProcessorLoop", ex);
        }
    }

    private void ProcessTag(RfidTagReadEventArgs ev)
    {
        var epc = NormalizeEpc(ev.Epc);
        var now = DateTime.UtcNow;

        // debounce per EPC
        if (_lastSeen.TryGetValue(epc, out var last) &&
            (now - last).TotalMilliseconds < _cfg.DebounceMs)
        {
            // HOT PATH: sem log por tag
            return;
        }

        _lastSeen[epc] = now;

        if (!_tagMeta.TryGetValue(epc, out var meta))
            meta = ("DESCONHECIDO", "SEM_LOTE");

        var session = _session.CurrentSession;

        // SAÍDA: se ainda está DESCONHECIDO, tenta enriquecer via MEPO/estoque em background.
        if (session is not null && session.Tipo == SessionType.Saida &&
            (string.Equals(meta.sku, "DESCONHECIDO", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(meta.lote, "SEM_LOTE", StringComparison.OrdinalIgnoreCase)))
        {
            _ = TryEnrichEpcFromEstoqueAsync(epc);
        }

        if (session is not null && session.Status == SessionStatus.Ativa)
        {
            var sku = session.Tipo == SessionType.Entrada ? session.Sku : meta.sku;
            var lote = session.Tipo == SessionType.Entrada ? session.Lote : meta.lote;

            var tag = new TagItem
            {
                Epc = epc,
                SessionId = session.SessionId,
                Tipo = session.Tipo,
                Origem = session.Origem,
                VendaNumero = session.VendaNumero,
                Sku = sku,
                Lote = lote,
                EntradaId = session.EntradaId,
                DataFabricacao = session.DataFabricacao,
                DataValidade = session.DataValidade,
                // ✅ Status correto por tipo (ENTRADA precisa ser "staged")
                Status = session.Tipo == SessionType.Entrada ? "staged" : "pendente",
                Rssi = (int)Math.Round(ev.Rssi ?? 0),
                LidaEm = now
            };

            _batch.EnqueueTag(tag);
            _ = _realtime.BroadcastTagReadAsync(tag.Epc, tag.Sku, tag.Lote, tag.SessionId, tag.Rssi);
        }

        var countSku = session?.Tipo == SessionType.Entrada && !string.IsNullOrWhiteSpace(session.Sku)
            ? session.Sku
            : meta.sku;
        var countLote = session?.Tipo == SessionType.Entrada && !string.IsNullOrWhiteSpace(session.Lote)
            ? session.Lote
            : meta.lote;

        _counts.AddOrUpdate((countSku, countLote), 1, (_, prev) => prev + 1);
        _countedAs[epc] = (countSku, countLote);

        // ✅ Adiciona à lista Recent apenas se não existir (mantém tags únicas)
        lock (_recentLock)
        {
            var existingTags = _recent.ToArray();
            if (!existingTags.Contains(epc, StringComparer.OrdinalIgnoreCase))
            {
                _recent.Enqueue(epc);
                while (_recent.Count > 30 && _recent.TryDequeue(out _)) { }
            }
        }
        // HOT PATH: evitar log por tag
    }

    private static string NormalizeEpc(string epc)
        => epc.Trim().ToUpperInvariant();

    private async Task TryEnrichEpcFromEstoqueAsync(string epc)
    {
        // não duplica requisições para o mesmo EPC
        if (!_enrichInFlight.TryAdd(epc, 1)) return;

        try
        {
            await _supabase.ConnectAsync(); // idempotente (vai manter token)

            // Usamos o método existente de histórico (que já funciona) para enriquecer a UI: 
            var hist = await _supabase.GetTagHistoricoAsync(epc, limit: 1).ConfigureAwait(false);
            var sku = hist.Current?.Sku;
            var lote = hist.Current?.Lote;

            if (string.IsNullOrWhiteSpace(sku) && string.IsNullOrWhiteSpace(lote))
                return;

            var newSku = string.IsNullOrWhiteSpace(sku) ? "DESCONHECIDO" : sku.Trim().ToUpperInvariant();
            var newLote = string.IsNullOrWhiteSpace(lote) ? "SEM_LOTE" : lote.Trim().ToUpperInvariant();

            _tagMeta[epc] = (newSku, newLote);

            // Ajusta contadores caso já tenhamos contado como DESCONHECIDO
            if (_countedAs.TryGetValue(epc, out var oldGroup))
            {
                if (!string.Equals(oldGroup.sku, newSku, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(oldGroup.lote, newLote, StringComparison.OrdinalIgnoreCase))
                {
                    if (_counts.TryGetValue(oldGroup, out var oldCount) && oldCount > 0)
                        _counts[oldGroup] = Math.Max(0, oldCount - 1);

                    _counts.AddOrUpdate((newSku, newLote), 1, (_, prev) => prev + 1);
                    _countedAs[epc] = (newSku, newLote);

                    SnapshotUpdated?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        catch (Exception ex)
        {
            _log.Debug($"[TagPipeline] Enrich falhou para EPC={epc}: {ex.Message}");
        }
        finally
        {
            _enrichInFlight.TryRemove(epc, out _);
        }
    }
}
