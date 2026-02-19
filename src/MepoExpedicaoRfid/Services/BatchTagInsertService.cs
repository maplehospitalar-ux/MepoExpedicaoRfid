using System.Collections.Concurrent;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using MepoExpedicaoRfid.Models;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// Serviço de inserção em batch com debounce e idempotência
/// PARTE E: Usa HttpClient singleton para melhor performance
/// </summary>
public sealed class BatchTagInsertService : IDisposable
{
    // PARTE E: HttpClient singleton - evita socket exhaustion e melhora performance
    private static readonly HttpClient _httpClient = new HttpClient();
    
    private readonly SupabaseService _supabase;
    private readonly OfflineBufferService _offline;
    private readonly AppLogger _log;
    private readonly int _batchSize;
    private readonly int _flushIntervalMs;

    private readonly ConcurrentQueue<TagItem> _queue = new();
    private readonly ConcurrentDictionary<string, byte> _processedKeys = new(StringComparer.OrdinalIgnoreCase);
    private readonly System.Timers.Timer _flushTimer;
    private readonly SemaphoreSlim _flushLock = new(1, 1);
    private bool _disposed;

    public int PendingCount => _queue.Count;

    public BatchTagInsertService(SupabaseService supabase, OfflineBufferService offline, AppLogger log, int batchSize = 50, int flushIntervalMs = 500)
    {
        _supabase = supabase;
        _offline = offline;
        _log = log;
        _batchSize = batchSize;
        _flushIntervalMs = flushIntervalMs;

        _flushTimer = new System.Timers.Timer(_flushIntervalMs);
        _flushTimer.Elapsed += async (_, __) => await FlushAsync();
        _flushTimer.AutoReset = true;
        _flushTimer.Start();
    }

    /// <summary>
    /// Adiciona uma tag à fila
    /// PARTE D: Usa ConcurrentDictionary para idempotência thread-safe
    /// </summary>
    public void EnqueueTag(TagItem tag)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(BatchTagInsertService));

        if (string.IsNullOrWhiteSpace(tag.SessionId))
        {
            _log.Debug("Tag ignorada: SessionId vazio.");
            return;
        }

        // PARTE D: Idempotência thread-safe usando ConcurrentDictionary
        // TryAdd garante que apenas a primeira inserção vence, duplicatas são ignoradas
        if (!_processedKeys.TryAdd(tag.IdempotencyKey, 1))
        {
            _log.Debug($"Tag já processada (idempotência): {tag.Epc}");
            return;
        }

        _queue.Enqueue(tag);

        // Flush imediato se atingiu o batch size
        if (_queue.Count >= _batchSize)
        {
            _ = FlushAsync();
        }
    }

    /// <summary>
    /// Força flush de todas as tags pendentes
    /// </summary>
    public async Task FlushAsync()
    {
        if (!await _flushLock.WaitAsync(0))
            return; // Já está executando flush

        try
        {
            if (_queue.IsEmpty)
                return;

            var batch = new List<TagItem>();
            while (batch.Count < _batchSize && _queue.TryDequeue(out var tag))
            {
                batch.Add(tag);
            }

            if (batch.Count == 0)
                return;

            await InsertBatchAsync(batch);
        }
        finally
        {
            _flushLock.Release();
        }
    }

    private async Task InsertBatchAsync(List<TagItem> batch)
    {
        try
        {
            if (!_supabase.IsConnected)
                await _supabase.ConnectAsync();

            // Determina tipo de inserção pela primeira tag
            var firstTag = batch[0];
            // ✅ CORRIGIDO: Uso robusto de SessionType ao invés de string
            var isSaida = firstTag.Tipo == SessionType.Saida;

            if (isSaida)
            {
                await InsertSaidaBatchAsync(batch);
            }
            else
            {
                await InsertEstoqueBatchAsync(batch);
            }

            _log.Info($"✅ Batch inserido: {batch.Count} tags");
        }
        catch (Exception ex)
        {
            _log.Error($"Erro ao inserir batch de {batch.Count} tags", ex);
            foreach (var tag in batch)
                await _offline.AddTagAsync(tag);
        }
    }

    private async Task InsertSaidaBatchAsync(List<TagItem> batch)
    {
        // ✅ ENRIQUECIMENTO (garante que não vá sku=DESCONHECIDO quando o estoque já conhece a tag)
        // Como o TagPipeline enriquece em background, o batch pode chegar aqui ainda com Sku/Lote vazios.
        // Fazemos lookup em batch em rfid_tags_estoque antes do INSERT.
        var needs = batch
            .Where(t => string.IsNullOrWhiteSpace(t.Sku) ||
                        string.Equals(t.Sku, "DESCONHECIDO", StringComparison.OrdinalIgnoreCase) ||
                        string.IsNullOrWhiteSpace(t.Lote) ||
                        string.Equals(t.Lote, "SEM_LOTE", StringComparison.OrdinalIgnoreCase))
            .Select(t => t.Epc)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (needs.Count > 0)
        {
            try
            {
                var snapshots = await _supabase.GetTagsEstoqueSnapshotsAsync(needs);
                foreach (var t in batch)
                {
                    if (snapshots.TryGetValue(t.Epc, out var snap))
                    {
                        // Preenche somente se o batch está desconhecido (não sobrescreve se já veio ok)
                        if (string.IsNullOrWhiteSpace(t.Sku) || string.Equals(t.Sku, "DESCONHECIDO", StringComparison.OrdinalIgnoreCase))
                            t.Sku = string.IsNullOrWhiteSpace(snap.Sku) ? t.Sku : snap.Sku;
                        if (string.IsNullOrWhiteSpace(t.Lote) || string.Equals(t.Lote, "SEM_LOTE", StringComparison.OrdinalIgnoreCase))
                            t.Lote = string.IsNullOrWhiteSpace(snap.Batch) ? t.Lote : snap.Batch;

                        // Ajuda o backend (status_anterior correto)
                        if (string.IsNullOrWhiteSpace(t.StatusAnterior) && !string.IsNullOrWhiteSpace(snap.Status))
                            t.StatusAnterior = snap.Status;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Warn($"Enriquecimento batch (saída) falhou: {ex.Message}");
            }
        }

        var payload = JsonSerializer.Serialize(batch.Select(t => new
        {
            session_id = t.SessionId,
            tag_epc = t.Epc,
            // fallback final (ainda pode ser desconhecido se a tag realmente não existe no estoque)
            sku = string.IsNullOrWhiteSpace(t.Sku) ? "DESCONHECIDO" : t.Sku,
            lote = string.IsNullOrWhiteSpace(t.Lote) ? "SEM_LOTE" : t.Lote,
            // ✅ CORRIGIDO: Usa StatusAnterior (não StatusOriginal)
            status_anterior = string.IsNullOrWhiteSpace(t.StatusAnterior) ? "available" : t.StatusAnterior,
            status = "pendente",
            idempotency_key = $"{t.SessionId}:{t.Epc}",
            quantidade = 1,
            venda_numero = t.VendaNumero ?? "SEM_VENDA",
            origem = string.IsNullOrWhiteSpace(t.Origem) ? "MANUAL" : t.Origem.Trim().ToUpperInvariant(),
            // ✅ metadata ajuda debug no MEPO
            metadata = new
            {
                reader_id = _supabase.DeviceId,
                client_type = _supabase.ClientType,
                inserted_at = DateTime.UtcNow.ToString("o")
            }
        }).ToList());

        // IMPORTANTE: para upsert por chave idempotency_key (não-PK), precisamos informar on_conflict.
        using var req = new HttpRequestMessage(HttpMethod.Post, $"{_supabase.BaseUrl}/rest/v1/rfid_saidas_audit?on_conflict=idempotency_key");
        req.Headers.Add("apikey", _supabase.AnonKey);
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _supabase.AccessToken);
        req.Headers.Add("Prefer", "resolution=merge-duplicates");
        req.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        // PARTE E: Usa HttpClient singleton
        var resp = await _httpClient.SendAsync(req);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Falha ao inserir batch saída: {resp.StatusCode} {body}");
        }
    }

    private async Task InsertEstoqueBatchAsync(List<TagItem> batch)
    {
        var payload = JsonSerializer.Serialize(batch.Select(t => new
        {
            tag_rfid = t.Epc,
            sku = t.Sku,
            batch = t.Lote,
            description = t.Descricao,
            // ✅ ENTRADA: status precisa ser SEMPRE "staged" (check constraint do banco)
            status = "staged",
            entrada_id = t.EntradaId,
            // ✅ CORRIGIDO: Nomes corretos das colunas
            manufacture_date = t.DataFabricacao?.ToString("yyyy-MM-dd"),
            expiration_date = t.DataValidade?.ToString("yyyy-MM-dd")
            // ❌ REMOVIDO: cmc (não existe na tabela)
        }).ToList());

        // IMPORTANTE: para upsert por chave única tag_rfid (não-PK), precisamos informar on_conflict.
        using var req = new HttpRequestMessage(HttpMethod.Post, $"{_supabase.BaseUrl}/rest/v1/rfid_tags_estoque?on_conflict=tag_rfid");
        req.Headers.Add("apikey", _supabase.AnonKey);
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _supabase.AccessToken);
        req.Headers.Add("Prefer", "resolution=merge-duplicates");
        req.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        // PARTE E: Usa HttpClient singleton
        var resp = await _httpClient.SendAsync(req);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Falha ao inserir batch estoque: {resp.StatusCode} {body}");
        }
    }

    /// <summary>
    /// Limpa as chaves processadas (resetar por sessão)
    /// </summary>
    public void ClearProcessedKeys()
    {
        _processedKeys.Clear();
        _log.Info("Chaves de idempotência limpas");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _flushTimer?.Stop();
        _flushTimer?.Dispose();

        // Flush final
        FlushAsync().Wait();

        _flushLock.Dispose();
    }
}
