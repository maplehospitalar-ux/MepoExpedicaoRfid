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
        var payload = JsonSerializer.Serialize(batch.Select(t => new
        {
            session_id = t.SessionId,
            tag_epc = t.Epc,
            sku = t.Sku,
            lote = t.Lote,
            // ✅ CORRIGIDO: Usa StatusAnterior (não StatusOriginal)
            status_anterior = t.StatusAnterior ?? "available",
            // ✅ CORRIGIDO: status permitido pela tabela rfid_saidas_audit (check constraint)
            // Padrão: "lida" (ver DOCUMENTACAO_TECNICA_INTEGRACAO.md)
            // Status aceitos no Supabase (exemplos encontrados): pendente, sincronizado, pending, completed
            // Usamos "pendente" na criação do audit de saída.
            status = "pendente",
            // ✅ CORRIGIDO: Formato correto {session_id}:{tag_epc}
            idempotency_key = $"{t.SessionId}:{t.Epc}",
            // ✅ ADICIONADO: Quantidade obrigatória
            quantidade = 1,
            // ✅ CORRIGIDO: venda_numero com valor padrão quando NULL
            venda_numero = t.VendaNumero ?? "SEM_VENDA",
            // ✅ CORRIGIDO: origem deve ser OMIE, CONTAAZUL, LEXOS ou MANUAL (maiúsculo)
            origem = string.IsNullOrWhiteSpace(t.Origem) ? "MANUAL" : t.Origem.Trim().ToUpperInvariant()
        }).ToList());

        using var req = new HttpRequestMessage(HttpMethod.Post, $"{_supabase.BaseUrl}/rest/v1/rfid_saidas_audit");
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

        using var req = new HttpRequestMessage(HttpMethod.Post, $"{_supabase.BaseUrl}/rest/v1/rfid_tags_estoque");
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
