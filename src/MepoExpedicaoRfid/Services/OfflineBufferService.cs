using Microsoft.Data.Sqlite;
using System.Text.Json;
using MepoExpedicaoRfid.Models;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// Serviço de buffer offline usando SQLite para armazenar tags quando sem conexão
/// </summary>
public sealed class OfflineBufferService : IDisposable
{
    private readonly string _dbPath;
    private readonly int _maxTags;
    private readonly AppLogger _log;
    private bool _disposed;

    public OfflineBufferService(string dbPath, int maxTags, AppLogger log)
    {
        _dbPath = dbPath;
        _maxTags = maxTags;
        _log = log;

        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var conn = new SqliteConnection($"Data Source={_dbPath}");
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS offline_tags_buffer (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                tag_epc TEXT NOT NULL,
                session_id TEXT NOT NULL,
                sku TEXT,
                lote TEXT,
                entrada_id TEXT,
                data_fabricacao TEXT,
                data_validade TEXT,
                descricao TEXT,
                rssi INTEGER,
                cmc REAL,
                status_original TEXT,
                status_novo TEXT,
                lida_em TEXT NOT NULL,
                created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                synced INTEGER DEFAULT 0,
                UNIQUE(tag_epc, session_id)
            );
            
            CREATE INDEX IF NOT EXISTS idx_synced ON offline_tags_buffer(synced);
            CREATE INDEX IF NOT EXISTS idx_session ON offline_tags_buffer(session_id);
        ";
        cmd.ExecuteNonQuery();

        TryAddColumn(conn, "offline_tags_buffer", "entrada_id", "TEXT");
        TryAddColumn(conn, "offline_tags_buffer", "data_fabricacao", "TEXT");
        TryAddColumn(conn, "offline_tags_buffer", "data_validade", "TEXT");

        _log.Info($"✅ SQLite database inicializado: {_dbPath}");
    }

    private static void TryAddColumn(SqliteConnection conn, string table, string column, string type)
    {
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {type}";
            cmd.ExecuteNonQuery();
        }
        catch
        {
            // Ignora se já existe
        }
    }

    /// <summary>
    /// Adiciona uma tag ao buffer offline
    /// </summary>
    public async Task<bool> AddTagAsync(TagItem tag)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OfflineBufferService));

        try
        {
            // Verifica limite
            var count = await GetPendingCountAsync();
            if (count >= _maxTags)
            {
                _log.Warn($"Buffer offline cheio ({_maxTags} tags). Tag não adicionada: {tag.Epc}");
                return false;
            }

            using var conn = new SqliteConnection($"Data Source={_dbPath}");
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT OR IGNORE INTO offline_tags_buffer 
                (tag_epc, session_id, sku, lote, entrada_id, data_fabricacao, data_validade, descricao, rssi, status_anterior, status, lida_em)
                VALUES (@epc, @session, @sku, @lote, @entradaId, @dataFab, @dataVal, @desc, @rssi, @statusAnt, @status, @lida)
            ";
            cmd.Parameters.AddWithValue("@epc", tag.Epc);
            cmd.Parameters.AddWithValue("@session", tag.SessionId);
            cmd.Parameters.AddWithValue("@sku", tag.Sku ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@lote", tag.Lote ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@entradaId", tag.EntradaId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@dataFab", tag.DataFabricacao?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@dataVal", tag.DataValidade?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@desc", tag.Descricao ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@rssi", tag.Rssi);
            cmd.Parameters.AddWithValue("@statusAnt", tag.StatusAnterior ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@status", tag.Status);
            cmd.Parameters.AddWithValue("@lida", tag.LidaEm.ToString("o"));

            var rows = await cmd.ExecuteNonQueryAsync();
            if (rows > 0)
            {
                _log.Info($"💾 Tag adicionada ao buffer offline: {tag.Epc}");
            }

            return rows > 0;
        }
        catch (Exception ex)
        {
            _log.Error($"Erro ao adicionar tag ao buffer offline: {tag.Epc}", ex);
            return false;
        }
    }

    /// <summary>
    /// Retorna tags pendentes de sincronização
    /// </summary>
    public async Task<List<TagItem>> GetPendingTagsAsync(int limit = 100)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OfflineBufferService));

        var tags = new List<TagItem>();

        try
        {
            using var conn = new SqliteConnection($"Data Source={_dbPath}");
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                  SELECT id, tag_epc, session_id, sku, lote, entrada_id, data_fabricacao, data_validade, descricao, rssi, 
                      status_anterior, status, lida_em
                FROM offline_tags_buffer
                WHERE synced = 0
                ORDER BY id ASC
                LIMIT @limit
            ";
            cmd.Parameters.AddWithValue("@limit", limit);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tags.Add(new TagItem
                {
                    Id = reader.GetInt32(0).ToString(),
                    Epc = reader.GetString(1),
                    SessionId = reader.GetString(2),
                    Sku = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Lote = reader.IsDBNull(4) ? null : reader.GetString(4),
                    EntradaId = reader.IsDBNull(5) ? null : reader.GetString(5),
                    DataFabricacao = reader.IsDBNull(6) ? null : DateTime.Parse(reader.GetString(6)),
                    DataValidade = reader.IsDBNull(7) ? null : DateTime.Parse(reader.GetString(7)),
                    Descricao = reader.IsDBNull(8) ? null : reader.GetString(8),
                    Rssi = reader.GetInt32(9),
                    StatusAnterior = reader.IsDBNull(10) ? null : reader.GetString(10),
                    Status = reader.GetString(11),
                    LidaEm = DateTime.Parse(reader.GetString(12))
                });
            }
        }
        catch (Exception ex)
        {
            _log.Error("Erro ao buscar tags pendentes do buffer offline", ex);
        }

        return tags;
    }

    /// <summary>
    /// Marca tags como sincronizadas
    /// </summary>
    public async Task<bool> MarkAsSyncedAsync(List<string> tagIds)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OfflineBufferService));

        try
        {
            using var conn = new SqliteConnection($"Data Source={_dbPath}");
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = $@"
                UPDATE offline_tags_buffer
                SET synced = 1
                WHERE id IN ({string.Join(",", tagIds)})
            ";

            var rows = await cmd.ExecuteNonQueryAsync();
            _log.Info($"✅ {rows} tags marcadas como sincronizadas");

            return rows > 0;
        }
        catch (Exception ex)
        {
            _log.Error("Erro ao marcar tags como sincronizadas", ex);
            return false;
        }
    }

    /// <summary>
    /// Limpa tags sincronizadas antigas
    /// </summary>
    public async Task<int> CleanupSyncedTagsAsync(int daysOld = 7)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OfflineBufferService));

        try
        {
            using var conn = new SqliteConnection($"Data Source={_dbPath}");
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM offline_tags_buffer
                WHERE synced = 1 
                AND datetime(created_at) < datetime('now', '-' || @days || ' days')
            ";
            cmd.Parameters.AddWithValue("@days", daysOld);

            var rows = await cmd.ExecuteNonQueryAsync();
            if (rows > 0)
            {
                _log.Info($"🗑️ {rows} tags antigas removidas do buffer offline");
            }

            return rows;
        }
        catch (Exception ex)
        {
            _log.Error("Erro ao limpar tags sincronizadas", ex);
            return 0;
        }
    }

    /// <summary>
    /// Retorna contagem de tags pendentes
    /// </summary>
    public async Task<int> GetPendingCountAsync()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OfflineBufferService));

        try
        {
            using var conn = new SqliteConnection($"Data Source={_dbPath}");
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM offline_tags_buffer WHERE synced = 0";

            var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return count;
        }
        catch (Exception ex)
        {
            _log.Error("Erro ao contar tags pendentes", ex);
            return 0;
        }
    }

    /// <summary>
    /// Sincroniza buffer offline com o backend
    /// </summary>
    public async Task<int> SyncWithBackendAsync(BatchTagInsertService batchService)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OfflineBufferService));

        try
        {
            var pendingTags = await GetPendingTagsAsync(100);
            if (pendingTags.Count == 0)
                return 0;

            _log.Info($"🔄 Sincronizando {pendingTags.Count} tags do buffer offline...");

            foreach (var tag in pendingTags)
            {
                batchService.EnqueueTag(tag);
            }

            // Aguarda flush
            await batchService.FlushAsync();

            // Marca como sincronizado
            var ids = pendingTags.Select(t => t.Id).ToList();
            await MarkAsSyncedAsync(ids);

            _log.Info($"✅ {pendingTags.Count} tags sincronizadas com sucesso");
            return pendingTags.Count;
        }
        catch (Exception ex)
        {
            _log.Error("Erro ao sincronizar buffer offline", ex);
            return 0;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        // Cleanup de tags antigas
        CleanupSyncedTagsAsync().Wait();
    }
}
