using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MepoExpedicaoRfid.Models;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// Implementação leve e robusta usando endpoints oficiais do Supabase (Auth + PostgREST + RPC).
/// Não depende de detalhes internos do SDK (varia por versão).
/// </summary>
public sealed class SupabaseService
{
    private readonly SupabaseConfig _cfg;
    private readonly AuthConfig _auth;
    private readonly DeviceConfig _device;
    private readonly AppLogger _log;

    private readonly HttpClient _http = new();
    private string? _accessToken;
    private string? _authUserId;

    public bool IsConnected => !string.IsNullOrWhiteSpace(_accessToken);
    public string BaseUrl => _cfg.Url;
    public string AnonKey => _cfg.AnonKey;
    public string? AccessToken => _accessToken;
    public string? AuthUserId => _authUserId;

    public SupabaseService(SupabaseConfig cfg, AuthConfig auth, DeviceConfig device, AppLogger log)
    {
        _cfg = cfg;
        _auth = auth;
        _device = device;
        _log = log;

        _http.Timeout = TimeSpan.FromSeconds(20);
        _http.DefaultRequestHeaders.Add("apikey", _cfg.AnonKey);
    }

    public async Task ConnectAsync()
    {
        if (string.IsNullOrWhiteSpace(_cfg.Url) || string.IsNullOrWhiteSpace(_cfg.AnonKey))
            throw new InvalidOperationException("Configure Supabase.Url e Supabase.AnonKey em appsettings.json");

        // Password grant
        var url = $"{_cfg.Url}/auth/v1/token?grant_type=password";
        var payload = JsonSerializer.Serialize(new { email = _auth.Email, password = _auth.Password });
        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        using var resp = await _http.SendAsync(req);
        var body = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new InvalidOperationException($"Falha Auth Supabase: {resp.StatusCode} {body}");

        using var doc = JsonDocument.Parse(body);
        _accessToken = doc.RootElement.GetProperty("access_token").GetString();

        if (string.IsNullOrWhiteSpace(_accessToken))
            throw new InvalidOperationException("Supabase não retornou access_token.");

        // Tenta descobrir o UUID do usuário autenticado (necessário para alguns RPCs que exigem uuid)
        try
        {
            _authUserId = await FetchAuthUserIdAsync();
            if (!string.IsNullOrWhiteSpace(_authUserId))
                _log.Info($"✅ Supabase autenticado. user_id={_authUserId}");
            else
                _log.Info("✅ Supabase autenticado.");
        }
        catch (Exception ex)
        {
            _log.Warn($"Supabase autenticado, mas falhei ao obter user_id: {ex.Message}");
        }
    }

    private async Task<string?> FetchAuthUserIdAsync()
    {
        if (string.IsNullOrWhiteSpace(_accessToken)) return null;

        using var req = new HttpRequestMessage(HttpMethod.Get, $"{_cfg.Url}/auth/v1/user");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        req.Headers.Add("apikey", _cfg.AnonKey);

        using var resp = await _http.SendAsync(req);
        var body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new InvalidOperationException($"Auth user failed: {resp.StatusCode} {body}");

        using var doc = JsonDocument.Parse(body);
        if (doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("id", out var id))
            return id.GetString();

        return null;
    }

    private async Task EnsureConnectedAsync()
    {
        if (!IsConnected)
            await ConnectAsync();
    }

    private HttpRequestMessage NewAuthedRequest(HttpMethod method, string pathAndQuery)
    {
        if (string.IsNullOrWhiteSpace(_accessToken))
            throw new InvalidOperationException("Supabase não conectado.");

        var req = new HttpRequestMessage(method, $"{_cfg.Url}{pathAndQuery}");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        req.Headers.Add("apikey", _cfg.AnonKey);
        return req;
    }

    public async Task HeartbeatAsync()
    {
        await EnsureConnectedAsync();
        // RPC: rfid_device_heartbeat(p_device_id)
        var rpcPath = "/rest/v1/rpc/rfid_device_heartbeat";
        using var req = NewAuthedRequest(HttpMethod.Post, rpcPath);
        req.Content = new StringContent(JsonSerializer.Serialize(new { p_device_id = _device.Id }), Encoding.UTF8, "application/json");
        using var resp = await _http.SendAsync(req);
        if (!resp.IsSuccessStatusCode)
        {
            var b = await resp.Content.ReadAsStringAsync();
            _log.Warn($"Heartbeat falhou: {resp.StatusCode} {b}");
        }
    }

    public async Task<IReadOnlyList<FilaItem>> GetFilaAsync(string[] statuses, int limit = 200)
    {
        await EnsureConnectedAsync();
        // Preferir VIEW v_fila_expedicao_csharp (compatível com C#). Fallback: rfid_saidas_sessions.
        var select = "*";
        var statusFilter = string.Join(",", statuses.Select(s => $"\"{s}\""));
        var path = $"/rest/v1/v_fila_expedicao_csharp?select={Uri.EscapeDataString(select)}&status=in.({Uri.EscapeDataString(statusFilter)})&order=prioridade.desc,criado_em.asc&limit={limit}";

        try
        {
            using var req = NewAuthedRequest(HttpMethod.Get, path);
            using var resp = await _http.SendAsync(req);
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode) throw new Exception(body);
            return JsonSerializer.Deserialize<List<FilaItem>>(body, JsonOpts()) ?? new();
        }
        catch (Exception ex)
        {
            _log.Warn($"v_fila_expedicao indisponível ou erro. Tentando fallback rfid_saidas_sessions. Motivo: {ex.Message}");
            // fallback minimal (fields may differ; adjust to your schema)
            var select2 = "id,session_id,venda_numero,status,created_at,finalized_at,total_tags_received,origem";
            var path2 = $"/rest/v1/rfid_saidas_sessions?select={Uri.EscapeDataString(select2)}&status=in.({Uri.EscapeDataString(statusFilter)})&order=created_at.asc&limit={limit}";
            using var req2 = NewAuthedRequest(HttpMethod.Get, path2);
            using var resp2 = await _http.SendAsync(req2);
            var body2 = await resp2.Content.ReadAsStringAsync();
            if (!resp2.IsSuccessStatusCode) throw new InvalidOperationException(body2);

            // minimal mapping
            using var doc = JsonDocument.Parse(body2);
            var list = new List<FilaItem>();
            foreach (var el in doc.RootElement.EnumerateArray())
            {
                list.Add(new FilaItem
                {
                    Id = Guid.TryParse(el.GetProperty("id").ToString(), out var guid) ? guid : Guid.Empty,
                    SessionId = el.TryGetProperty("session_id", out var sid) ? sid.GetString() : null,
                    NumeroPedido = el.TryGetProperty("venda_numero", out var vn) ? vn.GetString() : null,
                    Status = el.TryGetProperty("status", out var st) ? st.GetString() : null,
                    CriadoEm = el.TryGetProperty("created_at", out var ca) ? ca.GetDateTime() : DateTime.MinValue,
                    FinalizadoEm = el.TryGetProperty("finalized_at", out var fa) ? fa.GetDateTime() : null,
                    TagsLidas = el.TryGetProperty("total_tags_received", out var tt) ? tt.GetInt32() : 0,
                    Origem = el.TryGetProperty("origem", out var o) ? o.GetString() : null,
                });
            }
            return list;
        }
    }

    public async Task<Guid?> GetDocumentoIdByOrigemNumeroPedidoAsync(string origem, string numeroPedido)
    {
        await EnsureConnectedAsync();
        if (string.IsNullOrWhiteSpace(origem) || string.IsNullOrWhiteSpace(numeroPedido)) return null;

        // documentos_comerciais: buscar id pelo par (origem, numero_pedido)
        var select = "id";
        // numero_pedido pode ter letras/hífens (ex.: CA-841). Usar ilike para evitar erro quando coluna for text.
        var path = $"/rest/v1/documentos_comerciais?select={Uri.EscapeDataString(select)}&origem=eq.{Uri.EscapeDataString(origem.Trim().ToUpperInvariant())}&numero_pedido=ilike.{Uri.EscapeDataString(numeroPedido.Trim())}&limit=1";
        using var req = NewAuthedRequest(HttpMethod.Get, path);
        using var resp = await _http.SendAsync(req);
        var body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode) return null;

        using var doc = JsonDocument.Parse(body);
        var first = doc.RootElement.EnumerateArray().FirstOrDefault();
        if (first.ValueKind == JsonValueKind.Undefined) return null;

        if (first.TryGetProperty("id", out var idProp) && Guid.TryParse(idProp.ToString(), out var guid))
            return guid;

        return null;
    }

    public async Task<IReadOnlyList<DocumentoItemResumo>> GetDocumentoItensResumoAsync(Guid documentoId)
    {
        await EnsureConnectedAsync();

        // RLS: o desktop não consegue ler direto documentos_comerciais_itens.
        // Usamos a view exposta para o C#.
        var select = "sku,descricao,quantidade,preco_unitario,valor_total";
        var path = $"/rest/v1/v_documentos_comerciais_itens_csharp?select={Uri.EscapeDataString(select)}&documento_id=eq.{Uri.EscapeDataString(documentoId.ToString())}&order=sku.asc";
        using var req = NewAuthedRequest(HttpMethod.Get, path);
        using var resp = await _http.SendAsync(req);
        var body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new InvalidOperationException($"Erro ao buscar itens do documento {documentoId}: {resp.StatusCode} {body}");

        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<List<DocumentoItemResumo>>(body, opts) ?? new();
    }

    public async Task<PedidoPrintPayload?> GetPedidoPrintPayloadAsync(string origem, string numero)
    {
        await EnsureConnectedAsync();
        if (string.IsNullOrWhiteSpace(origem) || string.IsNullOrWhiteSpace(numero)) return null;

        var select = "documento_id,numero,origem,cliente_nome,valor_total,is_sem_nf,observacao_expedicao,status_expedicao,itens";
        var path = $"/rest/v1/v_pedido_print_payload?select={Uri.EscapeDataString(select)}&origem=eq.{Uri.EscapeDataString(origem.Trim().ToUpperInvariant())}&numero=eq.{Uri.EscapeDataString(numero.Trim())}&limit=1";
        using var req = NewAuthedRequest(HttpMethod.Get, path);
        using var resp = await _http.SendAsync(req);
        var body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new InvalidOperationException($"Erro ao buscar payload de impressão {origem}/{numero}: {resp.StatusCode} {body}");

        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var list = JsonSerializer.Deserialize<List<PedidoPrintPayload>>(body, opts) ?? new();
        return list.FirstOrDefault();
    }

    public async Task<TagHistoricoDto>  GetTagHistoricoAsync(string epc, int limit = 200)
    {
        await EnsureConnectedAsync();
        var norm = epc.Trim().ToUpperInvariant();
        _log.Info($"Consultando tag: {norm}");

        // CancellationToken com timeout de 15 segundos
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var ct = cts.Token;

        // estado atual com TODAS as colunas incluindo datas
        TagCurrent? current = null;
        try
        {
            var pathCur = $"/rest/v1/rfid_tags_estoque?select=tag_rfid,sku,batch,status,manufacture_date,expiration_date,updated_at&tag_rfid=eq.{Uri.EscapeDataString(norm)}&limit=1";
            using var req = NewAuthedRequest(HttpMethod.Get, pathCur);
            using var resp = await _http.SendAsync(req, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            _log.Debug($"Resposta rfid_tags_estoque: {resp.StatusCode} - {body}");

            if (resp.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(body);
                var first = doc.RootElement.EnumerateArray().FirstOrDefault();
                if (first.ValueKind != JsonValueKind.Undefined)
                {
                    current = new TagCurrent
                    {
                        Epc = norm,
                        Sku = first.TryGetProperty("sku", out var s) ? s.GetString() : null,
                        Lote = first.TryGetProperty("batch", out var b) ? b.GetString() : null,
                        Status = first.TryGetProperty("status", out var st) ? st.GetString() : null,
                        DataFabricacao = first.TryGetProperty("manufacture_date", out var mf) && mf.ValueKind != JsonValueKind.Null ? mf.GetDateTime() : null,
                        DataValidade = first.TryGetProperty("expiration_date", out var ev) && ev.ValueKind != JsonValueKind.Null ? ev.GetDateTime() : null,
                        UpdatedAt = first.TryGetProperty("updated_at", out var ua) ? ua.GetDateTime() : null,
                    };
                    _log.Info($"Tag encontrada: SKU={current.Sku}, Lote={current.Lote}, Status={current.Status}");

                    // Buscar descrição do produto se tiver SKU
                    if (!string.IsNullOrEmpty(current.Sku))
                    {
                        try
                        {
                            var prodPath = $"/rest/v1/produtos?select=descricao&sku=eq.{Uri.EscapeDataString(current.Sku)}&limit=1";
                            using var reqProd = NewAuthedRequest(HttpMethod.Get, prodPath);
                            using var respProd = await _http.SendAsync(reqProd, ct);
                            if (respProd.IsSuccessStatusCode)
                            {
                                var bodyProd = await respProd.Content.ReadAsStringAsync(ct);
                                using var docProd = JsonDocument.Parse(bodyProd);
                                var firstProd = docProd.RootElement.EnumerateArray().FirstOrDefault();
                                if (firstProd.ValueKind != JsonValueKind.Undefined && firstProd.TryGetProperty("descricao", out var desc))
                                {
                                    current.Descricao = desc.GetString();
                                    _log.Info($"Descrição encontrada: {current.Descricao}");
                                }
                            }
                        }
                        catch (Exception exProd)
                        {
                            _log.Warn($"Não foi possível buscar descrição do produto: {exProd.Message}");
                        }
                    }
                }
                else
                {
                    _log.Warn($"Tag não encontrada no estoque: {norm}");
                }
            }
        }
        catch (Exception ex)
        {
            _log.Error($"Erro ao buscar estado atual da tag: {ex.Message}");
        }

        // histórico unificado via view v_tag_historico_completo
        IReadOnlyList<TagMovement> movimentos;
        try
        {
            var select = "id,epc,tipo,sku,descricao,lote,numero_pedido,operador,local,created_at";
            var path = $"/rest/v1/v_tag_historico_completo?select={Uri.EscapeDataString(select)}&epc=eq.{Uri.EscapeDataString(norm)}&order=created_at.desc&limit={limit}";

            _log.Debug($"Consultando view: {path}");

            using var req = NewAuthedRequest(HttpMethod.Get, path);
            using var resp = await _http.SendAsync(req, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            _log.Debug($"Resposta v_tag_historico_completo: {resp.StatusCode} - {body.Substring(0, Math.Min(500, body.Length))}");

            if (!resp.IsSuccessStatusCode)
            {
                _log.Error($"Erro ao consultar view: {resp.StatusCode} - {body}");
                throw new Exception($"View retornou {resp.StatusCode}: {body}");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            movimentos = JsonSerializer.Deserialize<List<TagMovement>>(body, options) ?? new();
            _log.Info($"Histórico: {movimentos.Count} eventos encontrados");
        }
        catch (Exception ex)
        {
            _log.Error($"Erro ao consultar view v_tag_historico_completo: {ex.Message}");
            _log.Warn($"Retornando histórico vazio para EPC {norm}");
            movimentos = new List<TagMovement>();
        }

        var result = new TagHistoricoDto { Current = current, Movimentos = movimentos };
        _log.Info($"Retornando TagHistoricoDto - Current={(current != null ? $"SKU:{current.Sku}" : "NULL")}, Movimentos={movimentos.Count}");

        return result;
    }

    public sealed class EstoqueTagSnapshot
    {
        public string? Sku { get; set; }
        public string? Batch { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? Status { get; set; }
    }

    public async Task<EstoqueTagSnapshot?> GetTagEstoqueSnapshotAsync(string epc, CancellationToken ct = default)
    {
        await EnsureConnectedAsync();
        if (string.IsNullOrWhiteSpace(epc)) return null;

        var norm = epc.Trim().ToUpperInvariant();
        var path = $"/rest/v1/rfid_tags_estoque?select=sku,batch,manufacture_date,expiration_date,status&tag_rfid=eq.{Uri.EscapeDataString(norm)}&limit=1";

        try
        {
            using var req = NewAuthedRequest(HttpMethod.Get, path);
            using var resp = await _http.SendAsync(req, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode) return null;

            using var doc = JsonDocument.Parse(body);
            var first = doc.RootElement.EnumerateArray().FirstOrDefault();
            if (first.ValueKind == JsonValueKind.Undefined) return null;

            return new EstoqueTagSnapshot
            {
                Sku = first.TryGetProperty("sku", out var s) ? s.GetString() : null,
                Batch = first.TryGetProperty("batch", out var b) ? b.GetString() : null,
                ManufactureDate = first.TryGetProperty("manufacture_date", out var mf) && mf.ValueKind != JsonValueKind.Null ? mf.GetDateTime() : null,
                ExpirationDate = first.TryGetProperty("expiration_date", out var ev) && ev.ValueKind != JsonValueKind.Null ? ev.GetDateTime() : null,
                Status = first.TryGetProperty("status", out var st) ? st.GetString() : null,
            };
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateTagEstoqueAsync(string epc, string sku, string lote, DateTime? fab, DateTime? val)
    {
        await EnsureConnectedAsync();
        if (string.IsNullOrWhiteSpace(epc)) return false;

        var norm = epc.Trim().ToUpperInvariant();
        var path = $"/rest/v1/rfid_tags_estoque?tag_rfid=eq.{Uri.EscapeDataString(norm)}";
        using var req = NewAuthedRequest(HttpMethod.Patch, path);
        req.Headers.Add("Prefer", "return=minimal");
        req.Content = new StringContent(JsonSerializer.Serialize(new
        {
            sku = sku?.Trim(),
            batch = lote?.Trim(),
            manufacture_date = fab?.ToString("yyyy-MM-dd"),
            expiration_date = val?.ToString("yyyy-MM-dd"),
            status = "staged"
        }), Encoding.UTF8, "application/json");

        using var resp = await _http.SendAsync(req);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateSessaoStatusAsync(string sessionId, string status)
    {
        await EnsureConnectedAsync();
        // update rfid_saidas_sessions by session_id
        var path = $"/rest/v1/rfid_saidas_sessions?session_id=eq.{Uri.EscapeDataString(sessionId)}";
        using var req = NewAuthedRequest(HttpMethod.Patch, path);
        req.Headers.Add("Prefer", "return=minimal");
        req.Content = new StringContent(JsonSerializer.Serialize(new { status }), Encoding.UTF8, "application/json");

        using var resp = await _http.SendAsync(req);
        if (!resp.IsSuccessStatusCode)
        {
            var b = await resp.Content.ReadAsStringAsync();
            _log.Warn($"Update status falhou: {resp.StatusCode} {b}");
            return false;
        }
        return true;
    }

    public async Task<string?> EnviarPedidoParaExpedicaoAsync(string vendaNumero, string origem = "OMIE", int prioridade = 0)
    {
        await EnsureConnectedAsync();
        // RPC: enviar_pedido_para_expedicao
        var rpc = "/rest/v1/rpc/enviar_pedido_para_expedicao";
        using var req = NewAuthedRequest(HttpMethod.Post, rpc);
        req.Content = new StringContent(JsonSerializer.Serialize(new { p_venda_numero = vendaNumero, p_origem = origem, p_prioridade = prioridade }),
            Encoding.UTF8, "application/json");
        using var resp = await _http.SendAsync(req);
        var body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            _log.Warn($"RPC enviar_pedido_para_expedicao falhou: {resp.StatusCode} {body}");
            return null;
        }

        // espera retorno {session_id: "..."} ou string
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("session_id", out var sid))
                return sid.GetString();
        }
        catch { }
        return body.Trim('"');
    }

    private string? ResolveUserIdUuid(string? userIdCandidate)
    {
        // Alguns RPCs esperam UUID (auth.users.id). No Desktop, o caller às vezes passa device_id (ex: r3-desktop-02).
        // Aqui garantimos um UUID válido.
        if (!string.IsNullOrWhiteSpace(userIdCandidate) && Guid.TryParse(userIdCandidate, out _))
            return userIdCandidate;

        if (!string.IsNullOrWhiteSpace(_authUserId) && Guid.TryParse(_authUserId, out _))
            return _authUserId;

        return null;
    }

    public async Task<string?> BuscarDescricaoProdutoAsync(string sku, CancellationToken ct = default)
    {
        await EnsureConnectedAsync();
        if (string.IsNullOrWhiteSpace(sku)) return null;

        try
        {
            var norm = sku.Trim();
            var path = $"/rest/v1/produtos?select=descricao&sku=eq.{Uri.EscapeDataString(norm)}&limit=1";
            using var req = NewAuthedRequest(HttpMethod.Get, path);
            using var resp = await _http.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode) return null;

            var body = await resp.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(body);
            var first = doc.RootElement.EnumerateArray().FirstOrDefault();
            if (first.ValueKind == JsonValueKind.Undefined) return null;
            return first.TryGetProperty("descricao", out var d) ? d.GetString() : null;
        }
        catch (Exception ex)
        {
            _log.Warn($"Falha ao buscar descrição do SKU '{sku}': {ex.Message}");
            return null;
        }
    }

    public async Task<bool> FinalizarSessaoAsync(string sessionId, string userId)
    {
        await EnsureConnectedAsync();
        var rpc = "/rest/v1/rpc/finalizar_sessao_rfid";

        var uid = ResolveUserIdUuid(userId);
        if (string.IsNullOrWhiteSpace(uid))
        {
            _log.Warn("RPC finalizar_sessao_rfid: não tenho um user_id UUID válido (auth).");
            return false;
        }

        using var req = NewAuthedRequest(HttpMethod.Post, rpc);
        req.Content = new StringContent(JsonSerializer.Serialize(new { p_session_id = sessionId, p_user_id = uid }),
            Encoding.UTF8, "application/json");
        using var resp = await _http.SendAsync(req);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            _log.Warn($"RPC finalizar_sessao_rfid falhou: {resp.StatusCode} {body}");
            return false;
        }
        return true;
    }

    public async Task<bool> CancelarSessaoAsync(string sessionId, string userId)
    {
        await EnsureConnectedAsync();
        var rpc = "/rest/v1/rpc/cancelar_sessao_rfid";

        var uid = ResolveUserIdUuid(userId);
        if (string.IsNullOrWhiteSpace(uid))
        {
            _log.Warn("RPC cancelar_sessao_rfid: não tenho um user_id UUID válido (auth).");
            return false;
        }

        using var req = NewAuthedRequest(HttpMethod.Post, rpc);
        req.Content = new StringContent(JsonSerializer.Serialize(new { p_session_id = sessionId, p_user_id = uid }),
            Encoding.UTF8, "application/json");
        using var resp = await _http.SendAsync(req);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            _log.Warn($"RPC cancelar_sessao_rfid falhou: {resp.StatusCode} {body}");
            return false;
        }
        return true;
    }

    public async Task<SessionCreateResult> CriarSessaoSaidaAsync(string origem, string vendaNumero)
    {
        await EnsureConnectedAsync();
        var payload = new
        {
            action = "criar_saida",
            origem,
            venda_numero = vendaNumero,
            reader_id = _device.Id,
            client_type = _device.ClientType
        };

        return await CallSessionManagerAsync(payload);
    }

    /// <summary>
    /// Resolve o número do pedido (venda_numero) a partir de um identificador que o operador possa colar no Desktop.
    /// Ex: session_id do MEPO / fila (SAIDA_...), receipt_code/internal_number etc.
    ///
    /// Estratégia:
    /// - se já parecer um número de pedido (só dígitos), retorna como está.
    /// - tenta procurar na VIEW v_fila_expedicao_csharp por session_id.
    /// - fallback: procura em rfid_saidas_sessions por session_id e usa venda_numero.
    ///
    /// Retorna null se não conseguiu resolver.
    /// </summary>
    public async Task<string?> ResolverNumeroPedidoNoMepoAsync(string input)
    {
        await EnsureConnectedAsync();
        if (string.IsNullOrWhiteSpace(input)) return null;
        var raw = input.Trim();

        // Caso mais comum: já é número de pedido.
        if (raw.All(char.IsDigit)) return raw;

        // Caso comum no MEPO: session_id = SAIDA_<ORIGEM>_<NUMERO_PEDIDO>_<ID>
        // Ex: SAIDA_CONTAAZUL_865_1770652139312
        try
        {
            var parts = raw.Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length >= 4 && parts[0].Equals("SAIDA", StringComparison.OrdinalIgnoreCase))
            {
                var numero = parts[2];
                if (!string.IsNullOrWhiteSpace(numero) && numero.All(char.IsDigit))
                    return numero;
            }
        }
        catch { }

        // 1) tenta view da fila (campo numero_pedido)
        try
        {
            var path = $"/rest/v1/v_fila_expedicao_csharp?select=numero_pedido&session_id=eq.{Uri.EscapeDataString(raw)}&limit=1";
            using var req = NewAuthedRequest(HttpMethod.Get, path);
            using var resp = await _http.SendAsync(req);
            var body = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(body);
                var first = doc.RootElement.EnumerateArray().FirstOrDefault();
                if (first.ValueKind != JsonValueKind.Undefined && first.TryGetProperty("numero_pedido", out var np))
                {
                    var v = np.GetString();
                    if (!string.IsNullOrWhiteSpace(v)) return v.Trim();
                }
            }
        }
        catch (Exception ex)
        {
            _log.Debug($"ResolverNumeroPedidoNoMepoAsync(view) falhou: {ex.Message}");
        }

        // 2) fallback: tabela de sessões (campo venda_numero)
        try
        {
            var path2 = $"/rest/v1/rfid_saidas_sessions?select=venda_numero&session_id=eq.{Uri.EscapeDataString(raw)}&limit=1";
            using var req2 = NewAuthedRequest(HttpMethod.Get, path2);
            using var resp2 = await _http.SendAsync(req2);
            var body2 = await resp2.Content.ReadAsStringAsync();
            if (resp2.IsSuccessStatusCode)
            {
                using var doc2 = JsonDocument.Parse(body2);
                var first2 = doc2.RootElement.EnumerateArray().FirstOrDefault();
                if (first2.ValueKind != JsonValueKind.Undefined && first2.TryGetProperty("venda_numero", out var vn))
                {
                    var v = vn.GetString();
                    if (!string.IsNullOrWhiteSpace(v)) return v.Trim();
                }
            }
        }
        catch (Exception ex)
        {
            _log.Debug($"ResolverNumeroPedidoNoMepoAsync(sessions) falhou: {ex.Message}");
        }

        return null;
    }

    public async Task<SessionCreateResult> CriarSessaoEntradaAsync(string sku, string lote, DateTime? dataFabricacao, DateTime? dataValidade)
    {
        await EnsureConnectedAsync();
        var payload = new
        {
            action = "criar_entrada",
            sku,
            lote,
            data_fabricacao = dataFabricacao?.ToString("yyyy-MM-dd"),
            data_validade = dataValidade?.ToString("yyyy-MM-dd"),
            reader_id = _device.Id,
            client_type = _device.ClientType
        };

        return await CallSessionManagerAsync(payload);
    }

    public async Task<bool> FinalizarSessaoEdgeAsync(string sessionId, string tipo)
    {
        await EnsureConnectedAsync();
        var payload = new
        {
            action = "finalizar_sessao",
            session_id = sessionId,
            tipo
        };

        var result = await CallSessionManagerAsync(payload);
        return result.Success;
    }

    public async Task<bool> CancelarSessaoEdgeAsync(string sessionId, string motivo)
    {
        await EnsureConnectedAsync();
        var payload = new
        {
            action = "cancelar_sessao",
            session_id = sessionId,
            motivo
        };

        var result = await CallSessionManagerAsync(payload);
        return result.Success;
    }

    private async Task<SessionCreateResult> CallSessionManagerAsync(object payload)
    {
        var path = "/functions/v1/rfid-session-manager";
        using var req = NewAuthedRequest(HttpMethod.Post, path);
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var resp = await _http.SendAsync(req);
        var body = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
        {
            _log.Warn($"rfid-session-manager falhou: {resp.StatusCode} {body}");
            return new SessionCreateResult { Success = false, ErrorMessage = body };
        }

        try
        {
            return JsonSerializer.Deserialize<SessionCreateResult>(body, JsonOpts())
                   ?? new SessionCreateResult { Success = false, ErrorMessage = "Resposta inválida" };
        }
        catch (Exception ex)
        {
            return new SessionCreateResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public sealed class SessionCreateResult
    {
        [System.Text.Json.Serialization.JsonPropertyName("success")]
        public bool Success { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("session_id")]
        public string? SessionId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("receipt_code")]
        public string? ReceiptCode { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("existing")]
        public bool Existing { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("entrada_id")]
        public string? EntradaId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("internal_number")]
        public string? InternalNumber { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("message")]
        public string? Message { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("error_message")]
        public string? ErrorMessage { get; set; }
    }

    private static JsonSerializerOptions JsonOpts() => new()
    {
        PropertyNameCaseInsensitive = true
    };
}
