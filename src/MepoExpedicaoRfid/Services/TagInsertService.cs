using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MepoExpedicaoRfid.Models;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// Serviço de inserção individual de tags (fallback do batch)
/// </summary>
public sealed class TagInsertService
{
    private readonly SupabaseAuthService _auth;
    private readonly string _baseUrl;
    private readonly string _anonKey;
    private readonly AppLogger _log;
    private readonly HttpClient _http = new();

    public TagInsertService(SupabaseAuthService auth, string baseUrl, string anonKey, AppLogger log)
    {
        _auth = auth;
        _baseUrl = baseUrl;
        _anonKey = anonKey;
        _log = log;
    }

    public async Task<bool> InsertSaidaAsync(TagItem tag, string sessionId)
    {
        try
        {
            var token = await _auth.GetValidTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _log.Error("[TagInsert] No auth token available");
                return false;
            }

            var payload = new
            {
                session_id = sessionId,
                tag_epc = tag.Epc,
                sku = tag.Sku,
                lote = tag.Lote,
                // ✅ CORRIGIDO: Usa StatusAnterior (não StatusOriginal)
                status_anterior = tag.StatusAnterior ?? "available",
                // ✅ CORRIGIDO: Usa Status (não StatusNovo)
                status = tag.Status ?? "lida",
                // ✅ CORRIGIDO: Formato correto {session_id}:{tag_epc}
                idempotency_key = tag.IdempotencyKey,
                // ❌ REMOVIDO: cmc (não existe na tabela)
                // ✅ ADICIONADO: Campos obrigatórios
                quantidade = 1,
                venda_numero = tag.VendaNumero,
                origem = tag.Origem ?? "desktop_csharp"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/rest/v1/rfid_saidas_audit");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("apikey", _anonKey);
            request.Headers.Add("Prefer", "return=minimal");
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                _log.Debug($"[TagInsert] Saída inserted: {tag.Epc}");
                return true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _log.Warn($"[TagInsert] Failed: {error}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _log.Error($"[TagInsert] Exception: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> InsertEstoqueAsync(TagItem tag, string entradaId, DateTime? dataFabricacao, DateTime? dataValidade)
    {
        try
        {
            var token = await _auth.GetValidTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _log.Error("[TagInsert] No auth token available");
                return false;
            }

            var payload = new
            {
                tag_rfid = tag.Epc,
                sku = tag.Sku,
                batch = tag.Lote,
                manufacture_date = dataFabricacao?.ToString("yyyy-MM-dd"),
                expiration_date = dataValidade?.ToString("yyyy-MM-dd"),
                status = "staged",
                entrada_id = entradaId
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/rest/v1/rfid_tags_estoque");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("apikey", _anonKey);
            request.Headers.Add("Prefer", "return=minimal");
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                _log.Debug($"[TagInsert] Estoque inserted: {tag.Epc}");
                return true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _log.Warn($"[TagInsert] Failed: {error}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _log.Error($"[TagInsert] Exception: {ex.Message}");
            return false;
        }
    }

    public async Task<TagHistoricoDto?> ConsultarTagAsync(string epc)
    {
        try
        {
            var token = await _auth.GetValidTokenAsync();
            if (string.IsNullOrEmpty(token))
                return null;

            var request = new HttpRequestMessage(HttpMethod.Get, 
                $"{_baseUrl}/rest/v1/rfid_tags_estoque?tag_rfid=eq.{epc}&select=*");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("apikey", _anonKey);

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var results = JsonSerializer.Deserialize<List<TagHistoricoDto>>(json);

            return results?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _log.Error($"[TagInsert] Consulta exception: {ex.Message}");
            return null;
        }
    }
}
