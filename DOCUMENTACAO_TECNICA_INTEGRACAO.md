# 📋 DOCUMENTAÇÃO TÉCNICA C# DESKTOP - INTEGRAÇÃO SUPABASE

## Contexto do Sistema MEPO 2.0
Documentação completa de como o Desktop C# deve interagir com o backend Supabase para:
1. Carregar a Fila de Expedição
2. Criar e gerenciar sessões de SAÍDA (expedição de pedidos)
3. Criar e gerenciar sessões de ENTRADA (recebimento de estoque)
4. Enviar tags RFID lidas durante as operações

---

## PARTE 1: FILA DE EXPEDIÇÃO

### 1.1 Endpoint e Autenticação

```text
URL: {SUPABASE_URL}/rest/v1/v_fila_expedicao_csharp

HEADERS OBRIGATÓRIOS:
- apikey: {SUPABASE_ANON_KEY}          ← Anon Key, NÃO access token!
- Authorization: Bearer {ACCESS_TOKEN}  ← Token JWT do usuário
- Content-Type: application/json
```

### 1.2 Query Correta

```csharp
// ✅ CORRETO - Usar view v_fila_expedicao_csharp
public async Task<List<FilaItem>> GetFilaAsync(string[] statusFiltros, int limite = 300)
{
    // ⚠️ IMPORTANTE: Status corretos são:
    // - "na_fila"      → Pedidos aguardando início
    // - "preparando"   → Sessão criada, aguardando leitura
    // - "processando"  → Leitura em andamento
    // - "finalizada"   → Sessão concluída

    var statusQuery = string.Join(",", statusFiltros.Select(s => $"\"{s}\""));
    var path = $"/rest/v1/v_fila_expedicao_csharp?select=*&status=in.({statusQuery})&order=criado_em.desc&limit={limite}";

    var request = new HttpRequestMessage(HttpMethod.Get, $"{_supabaseUrl}{path}");
    request.Headers.Add("apikey", _anonKey);           // ← Anon Key aqui!
    request.Headers.Add("Authorization", $"Bearer {_accessToken}");

    var response = await _httpClient.SendAsync(request);
    var json = await response.Content.ReadAsStringAsync();

    return JsonSerializer.Deserialize<List<FilaItem>>(json, _jsonOptions);
}
```

### 1.3 Modelo C# - FilaItem

```csharp
public class FilaItem
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }  // NULL para pedidos na_fila

    [JsonPropertyName("numero_pedido")]
    public string? NumeroPedido { get; set; }

    [JsonPropertyName("cliente")]
    public string? Cliente { get; set; }

    [JsonPropertyName("total_itens")]
    public int TotalItens { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }  // na_fila, preparando, processando, finalizada

    [JsonPropertyName("criado_em")]
    public DateTime CriadoEm { get; set; }

    [JsonPropertyName("iniciado_em")]
    public DateTime? IniciadoEm { get; set; }

    [JsonPropertyName("finalizado_em")]
    public DateTime? FinalizadoEm { get; set; }

    [JsonPropertyName("prioridade")]
    public int Prioridade { get; set; }

    [JsonPropertyName("tags_lidas")]
    public int TagsLidas { get; set; }

    [JsonPropertyName("origem")]
    public string? Origem { get; set; }  // OMIE, CONTAAZUL, LEXOS, MANUAL
}
```

### 1.4 Colunas Retornadas pela View

| Coluna | Tipo SQL | Tipo C# | Descrição |
|--------|----------|---------|-----------|
| `id` | UUID | `Guid` | ID do documento ou sessão |
| `session_id` | TEXT | `string?` | NULL para pedidos na_fila, preenchido para sessões ativas |
| `numero_pedido` | TEXT | `string` | Número do pedido |
| `cliente` | TEXT | `string` | Nome do cliente |
| `total_itens` | INTEGER | `int` | Quantidade de SKUs no pedido |
| `status` | TEXT | `string` | na_fila, preparando, processando, finalizada |
| `criado_em` | TIMESTAMPTZ | `DateTime` | Data de criação |
| `iniciado_em` | TIMESTAMPTZ | `DateTime?` | Data de início da sessão |
| `finalizado_em` | TIMESTAMPTZ | `DateTime?` | Data de finalização |
| `prioridade` | INTEGER | `int` | 0 = normal |
| `tags_lidas` | INTEGER | `int` | Quantidade de tags lidas |
| `origem` | TEXT | `string` | OMIE, CONTAAZUL, LEXOS, MANUAL |

---

## PARTE 2: SESSÃO DE SAÍDA (Expedição)

### 2.1 Criar Sessão de Saída

```text
URL: {SUPABASE_URL}/functions/v1/rfid-session-manager
METHOD: POST

HEADERS:
- Content-Type: application/json
- Authorization: Bearer {ACCESS_TOKEN}
```

**Request Body:**
```json
{
  "action": "criar_saida",
  "origem": "OMIE",
  "venda_numero": "123456",
  "tipo": "rfid",
  "observacao_expedicao": "Observação opcional",
  "prioridade": 0,
  "reader_id": "r3-desktop-02",
  "client_type": "desktop_csharp"
}
```

**Response (Sucesso):**
```json
{
  "success": true,
  "session_id": "SAIDA_OMIE_123456_1738692123456",
  "receipt_code": "OMIE-123456",
  "message": "Sessão de saída criada com sucesso",
  "existing": false
}
```

**Response (Sessão já existe):**
```json
{
  "success": true,
  "session_id": "SAIDA_OMIE_123456_1738692000000",
  "receipt_code": "OMIE-123456",
  "message": "Sessão existente retornada",
  "existing": true
}
```

### 2.2 Código C# - Criar Sessão Saída

```csharp
public class CreateSessionResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }

    [JsonPropertyName("receipt_code")]
    public string? ReceiptCode { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("existing")]
    public bool Existing { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }  // ← Compatibilidade C#
}

public async Task<CreateSessionResult> CriarSessaoSaidaAsync(
    string origem, 
    string vendaNumero, 
    string? observacao = null)
{
    var payload = new
    {
        action = "criar_saida",
        origem = origem.ToUpperInvariant(),  // ✅ Normalizar para maiúsculo
        venda_numero = vendaNumero,
        tipo = "rfid",
        observacao_expedicao = observacao ?? "",
        prioridade = 0,
        reader_id = _cfg.Device.Id,
        client_type = "desktop_csharp"
    };

    var request = new HttpRequestMessage(HttpMethod.Post, 
        $"{_supabaseUrl}/functions/v1/rfid-session-manager");
    request.Headers.Add("Authorization", $"Bearer {_accessToken}");
    request.Content = new StringContent(
        JsonSerializer.Serialize(payload), 
        Encoding.UTF8, 
        "application/json");

    var response = await _httpClient.SendAsync(request);
    var json = await response.Content.ReadAsStringAsync();

    return JsonSerializer.Deserialize<CreateSessionResult>(json, _jsonOptions);
}
```

### 2.3 Enviar Tags de SAÍDA

**TABELA: `rfid_saidas_audit`**

```text
URL: {SUPABASE_URL}/rest/v1/rfid_saidas_audit
METHOD: POST

HEADERS:
- apikey: {SUPABASE_ANON_KEY}
- Authorization: Bearer {ACCESS_TOKEN}
- Content-Type: application/json
- Prefer: return=minimal
```

**Payload CORRETO para cada tag:**
```json
{
  "session_id": "SAIDA_OMIE_123456_1738692123456",
  "tag_epc": "E2801191A5030069B5E22DE9",
  "sku": "1189",
  "lote": "202504003",
  "origem": "OMIE",
  "venda_numero": "123456",
  "status": "lida",
  "status_anterior": "available",
  "quantidade": 1,
  "idempotency_key": "SAIDA_OMIE_123456_1738692123456:E2801191A5030069B5E22DE9"
}
```

### 2.4 Código C# - Inserir Tags de Saída

```csharp
public class TagSaidaPayload
{
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = "";

    [JsonPropertyName("tag_epc")]
    public string TagEpc { get; set; } = "";

    [JsonPropertyName("sku")]
    public string? Sku { get; set; }

    [JsonPropertyName("lote")]
    public string? Lote { get; set; }

    [JsonPropertyName("origem")]
    public string Origem { get; set; } = "";

    [JsonPropertyName("venda_numero")]
    public string VendaNumero { get; set; } = "";

    [JsonPropertyName("status")]
    public string Status { get; set; } = "lida";

    [JsonPropertyName("status_anterior")]
    public string? StatusAnterior { get; set; }  // ⚠️ NÃO usar "status_original"!

    [JsonPropertyName("quantidade")]
    public int Quantidade { get; set; } = 1;

    [JsonPropertyName("idempotency_key")]
    public string IdempotencyKey { get; set; } = "";
}

public async Task<bool> InserirTagSaidaAsync(TagSaidaPayload tag)
{
    // ✅ Normalizar EPC
    tag.TagEpc = tag.TagEpc.Trim().ToUpperInvariant();

    // ✅ Gerar idempotency_key se não existir
    if (string.IsNullOrEmpty(tag.IdempotencyKey))
    {
        tag.IdempotencyKey = $"{tag.SessionId}:{tag.TagEpc}";
    }

    var request = new HttpRequestMessage(HttpMethod.Post, 
        $"{_supabaseUrl}/rest/v1/rfid_saidas_audit");
    request.Headers.Add("apikey", _anonKey);
    request.Headers.Add("Authorization", $"Bearer {_accessToken}");
    request.Headers.Add("Prefer", "return=minimal");
    request.Content = new StringContent(
        JsonSerializer.Serialize(tag), 
        Encoding.UTF8, 
        "application/json");

    var response = await _httpClient.SendAsync(request);

    // 201 = Criado, 409 = Duplicado (idempotency_key já existe)
    return response.StatusCode == HttpStatusCode.Created || 
           response.StatusCode == HttpStatusCode.Conflict;
}
```

### 2.5 Inserção em BATCH (Recomendado)

```csharp
public async Task<int> InserirTagsSaidaBatchAsync(List<TagSaidaPayload> tags)
{
    // Normalizar todas as tags
    foreach (var tag in tags)
    {
        tag.TagEpc = tag.TagEpc.Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(tag.IdempotencyKey))
        {
            tag.IdempotencyKey = $"{tag.SessionId}:{tag.TagEpc}";
        }
    }

    var request = new HttpRequestMessage(HttpMethod.Post, 
        $"{_supabaseUrl}/rest/v1/rfid_saidas_audit");
    request.Headers.Add("apikey", _anonKey);
    request.Headers.Add("Authorization", $"Bearer {_accessToken}");
    request.Headers.Add("Prefer", "return=minimal,resolution=ignore-duplicates");
    request.Content = new StringContent(
        JsonSerializer.Serialize(tags),  // Array de tags!
        Encoding.UTF8, 
        "application/json");

    var response = await _httpClient.SendAsync(request);

    return response.IsSuccessStatusCode ? tags.Count : 0;
}
```

### 2.6 Colunas da Tabela `rfid_saidas_audit`

| Coluna | Tipo | Obrigatório | Descrição |
|--------|------|-------------|-----------|
| `session_id` | TEXT | ✅ SIM | ID da sessão (ex: SAIDA_OMIE_123_timestamp) |
| `tag_epc` | TEXT | ✅ SIM | EPC da tag RFID |
| `sku` | TEXT | NÃO | Código do produto |
| `lote` | TEXT | NÃO | Número do lote |
| `origem` | TEXT | ✅ SIM | OMIE, CONTAAZUL, LEXOS, MANUAL |
| `venda_numero` | TEXT | ✅ SIM | Número do pedido |
| `status` | TEXT | ✅ SIM | "lida" (default) |
| `status_anterior` | TEXT | NÃO | Status anterior da tag no estoque |
| `quantidade` | NUMERIC | ✅ SIM | Sempre 1 |
| `idempotency_key` | TEXT | ✅ SIM | Formato: `{session_id}:{tag_epc}` |

⚠️ **COLUNAS QUE NÃO EXISTEM** (não enviar):
- `cmc` ❌
- `status_original` ❌ (usar `status_anterior`)
- `reader_id` ❌
- `lida_em` ❌ (usa `created_at` automático)

---

## PARTE 3: SESSÃO DE ENTRADA (Recebimento)

### 3.1 Criar Sessão de Entrada

```text
URL: {SUPABASE_URL}/functions/v1/rfid-session-manager
METHOD: POST
```

**Request Body:**
```json
{
  "action": "criar_entrada",
  "sku": "1189",
  "lote": "202504003",
  "data_fabricacao": "2025-04-01",
  "data_validade": "2027-04-01"
}
```

**Response:**
```json
{
  "success": true,
  "session_id": "ENTRADA_1189_1738692123456",
  "entrada_id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "internal_number": "RC-1738692123456",
  "message": "Sessão de entrada criada com sucesso"
}
```

### 3.2 Código C# - Criar Sessão Entrada

```csharp
public class CreateEntradaResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }

    [JsonPropertyName("entrada_id")]
    public string? EntradaId { get; set; }  // ⚠️ Backend retorna UUID como string

    [JsonPropertyName("internal_number")]
    public string? InternalNumber { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}

public async Task<CreateEntradaResult> CriarSessaoEntradaAsync(
    string sku,
    string lote,
    DateTime? dataFabricacao = null,
    DateTime? dataValidade = null)
{
    var payload = new
    {
        action = "criar_entrada",
        sku = sku.Trim(),
        lote = lote.Trim(),
        data_fabricacao = dataFabricacao?.ToString("yyyy-MM-dd"),
        data_validade = dataValidade?.ToString("yyyy-MM-dd")
    };

    var request = new HttpRequestMessage(HttpMethod.Post, 
        $"{_supabaseUrl}/functions/v1/rfid-session-manager");
    request.Headers.Add("Authorization", $"Bearer {_accessToken}");
    request.Content = new StringContent(
        JsonSerializer.Serialize(payload), 
        Encoding.UTF8, 
        "application/json");

    var response = await _httpClient.SendAsync(request);
    var json = await response.Content.ReadAsStringAsync();

    return JsonSerializer.Deserialize<CreateEntradaResult>(json, _jsonOptions);
}
```

### 3.3 Enviar Tags de ENTRADA

**TABELA: `rfid_tags_estoque`**

```text
URL: {SUPABASE_URL}/rest/v1/rfid_tags_estoque
METHOD: POST

HEADERS:
- apikey: {SUPABASE_ANON_KEY}
- Authorization: Bearer {ACCESS_TOKEN}
- Content-Type: application/json
- Prefer: return=minimal
```

**Payload CORRETO:**
```json
{
  "entrada_id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "tag_rfid": "E2801191A5030069B5E22DE9",
  "sku": "1189",
  "batch": "202504003",
  "description": "Produto Exemplo",
  "manufacture_date": "2025-04-01",
  "expiration_date": "2027-04-01",
  "status": "staged"
}
```

### 3.4 Código C# - Inserir Tags de Entrada

```csharp
public class TagEntradaPayload
{
    [JsonPropertyName("entrada_id")]
    public string EntradaId { get; set; } = "";  // ⚠️ Backend espera UUID como string

    [JsonPropertyName("tag_rfid")]
    public string TagRfid { get; set; } = "";

    [JsonPropertyName("sku")]
    public string Sku { get; set; } = "";

    [JsonPropertyName("batch")]              // ⚠️ NÃO usar "lote"!
    public string? Batch { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("manufacture_date")]   // ⚠️ NÃO usar "data_fabricacao"!
    public string? ManufactureDate { get; set; }

    [JsonPropertyName("expiration_date")]    // ⚠️ NÃO usar "data_validade"!
    public string? ExpirationDate { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "staged";
}

public async Task<bool> InserirTagEntradaAsync(TagEntradaPayload tag)
{
    // ✅ Normalizar tag_rfid
    tag.TagRfid = tag.TagRfid.Trim().ToUpperInvariant();

    var request = new HttpRequestMessage(HttpMethod.Post, 
        $"{_supabaseUrl}/rest/v1/rfid_tags_estoque");
    request.Headers.Add("apikey", _anonKey);
    request.Headers.Add("Authorization", $"Bearer {_accessToken}");
    request.Headers.Add("Prefer", "return=minimal");
    request.Content = new StringContent(
        JsonSerializer.Serialize(tag), 
        Encoding.UTF8, 
        "application/json");

    var response = await _httpClient.SendAsync(request);

    return response.StatusCode == HttpStatusCode.Created;
}
```

### 3.5 Colunas da Tabela `rfid_tags_estoque`

| Coluna | Tipo | Obrigatório | Descrição |
|--------|------|-------------|-----------|
| `entrada_id` | UUID | ✅ SIM | ID da entrada (do CreateEntradaResult) |
| `tag_rfid` | TEXT | ✅ SIM | EPC da tag RFID |
| `sku` | TEXT | ✅ SIM | Código do produto |
| `batch` | TEXT | NÃO | Número do lote |
| `description` | TEXT | NÃO | Descrição do produto |
| `manufacture_date` | DATE | NÃO | Data de fabricação (YYYY-MM-DD) |
| `expiration_date` | DATE | NÃO | Data de validade (YYYY-MM-DD) |
| `status` | TEXT | ✅ SIM | Sempre "staged" para entrada |

⚠️ **COLUNAS COM NOMES DIFERENTES** (usar nomes em INGLÊS):
- `batch` ✅ (não `lote`)
- `manufacture_date` ✅ (não `data_fabricacao`)
- `expiration_date` ✅ (não `data_validade`)
- `tag_rfid` ✅ (não `tag_epc`)

---

## PARTE 4: FINALIZAR E CANCELAR SESSÕES

### 4.1 Finalizar Sessão (Saída ou Entrada)

```text
URL: {SUPABASE_URL}/functions/v1/rfid-session-manager
METHOD: POST
```

**Request Body:**
```json
{
  "action": "finalizar_sessao",
  "session_id": "SAIDA_OMIE_123456_1738692123456"
}
```

**Response:**
```json
{
  "success": true,
  "session_id": "SAIDA_OMIE_123456_1738692123456",
  "session_type": "saida",
  "tags_processed": 7,
  "omie_result": {
    "success": true,
    "message": "Baixa realizada no OMIE"
  }
}
```

### 4.2 Código C# - Finalizar Sessão

```csharp
public class FinalizarResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }

    [JsonPropertyName("session_type")]
    public string? SessionType { get; set; }

    [JsonPropertyName("tags_processed")]
    public int TagsProcessed { get; set; }

    [JsonPropertyName("omie_result")]
    public OmieResult? OmieResult { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}

public class OmieResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("requires_manual_omie")]
    public bool RequiresManualOmie { get; set; }
}

public async Task<FinalizarResult> FinalizarSessaoAsync(string sessionId)
{
    var payload = new
    {
        action = "finalizar_sessao",
        session_id = sessionId
    };

    var request = new HttpRequestMessage(HttpMethod.Post, 
        $"{_supabaseUrl}/functions/v1/rfid-session-manager");
    request.Headers.Add("Authorization", $"Bearer {_accessToken}");
    request.Content = new StringContent(
        JsonSerializer.Serialize(payload), 
        Encoding.UTF8, 
        "application/json");

    var response = await _httpClient.SendAsync(request);
    var json = await response.Content.ReadAsStringAsync();

    return JsonSerializer.Deserialize<FinalizarResult>(json, _jsonOptions);
}
```

### 4.3 Cancelar Sessão

```json
{
  "action": "cancelar_sessao",
  "session_id": "SAIDA_OMIE_123456_1738692123456"
}
```

---

## PARTE 5: HEARTBEAT

### 5.1 Endpoint Heartbeat

```text
URL: {SUPABASE_URL}/rest/v1/rpc/device_heartbeat
METHOD: POST

HEADERS:
- apikey: {SUPABASE_ANON_KEY}
- Authorization: Bearer {ACCESS_TOKEN}
- Content-Type: application/json
```

**Request Body (CORRETO):**
```json
{
  "p_device_id": "r3-desktop-02"
}
```

⚠️ **ATENÇÃO**: O payload deve conter APENAS `p_device_id`. Não enviar outros campos!

### 5.2 Código C# - Heartbeat

```csharp
public async Task<bool> EnviarHeartbeatAsync(string deviceId)
{
    try
    {
        var payload = new
        {
            p_device_id = deviceId  // ✅ Apenas este campo!
        };

        var request = new HttpRequestMessage(HttpMethod.Post, 
            $"{_supabaseUrl}/rest/v1/rpc/device_heartbeat");
        
        // ⚠️ IMPORTANTE: Headers separados!
        request.Headers.Add("apikey", _anonKey);
        request.Headers.Add("Authorization", $"Bearer {_accessToken}");
        
        request.Content = new StringContent(
            JsonSerializer.Serialize(payload), 
            Encoding.UTF8, 
            "application/json");

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
        _log.Warn($"⚠️ Erro ao enviar heartbeat: {ex.Message}");
        return false;
    }
}
```

### 5.3 Intervalo Recomendado

```csharp
// ✅ Enviar heartbeat a cada 30 segundos
private Timer? _heartbeatTimer;

public void IniciarHeartbeat(string deviceId)
{
    _heartbeatTimer = new Timer(async _ =>
    {
        await EnviarHeartbeatAsync(deviceId);
    }, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
}
```

---

## PARTE 6: CHECKLIST DE VALIDAÇÃO

### ✅ Fila de Expedição
- [ ] Usar `v_fila_expedicao_csharp` (não `v_fila_expedicao`)
- [ ] Status corretos: `na_fila`, `preparando`, `processando`, `finalizada`
- [ ] Headers: `apikey` + `Authorization: Bearer`

### ✅ Sessão de Saída
- [ ] Action `criar_saida` na Edge Function
- [ ] `client_type`: `desktop_csharp`
- [ ] Tags inseridas em `rfid_saidas_audit`
- [ ] `idempotency_key` = `{session_id}:{tag_epc}`
- [ ] Usar `status_anterior` (não `status_original`)
- [ ] NÃO enviar: `cmc`, `reader_id`, `lida_em`

### ✅ Sessão de Entrada
- [ ] Action `criar_entrada` na Edge Function
- [ ] Tags inseridas em `rfid_tags_estoque`
- [ ] Usar `batch` (não `lote`)
- [ ] Usar `manufacture_date` e `expiration_date` (não português)
- [ ] Status inicial: `staged`
- [ ] Campo `entrada_id` é string (UUID serializado)

### ✅ Finalização
- [ ] Action `finalizar_sessao`
- [ ] Para ENTRADA: tags permanecem `staged` (OMIE manual)
- [ ] Para SAÍDA: tags viram `used`, estoque atualizado

### ✅ Heartbeat
- [ ] Payload contém APENAS `p_device_id`
- [ ] Headers: `apikey` + `Authorization` separados
- [ ] Intervalo: 30 segundos
- [ ] Endpoint: `/rest/v1/rpc/device_heartbeat`

---

## RESUMO DE ENDPOINTS

| Operação | Método | Endpoint |
|----------|--------|----------|
| Carregar Fila | GET | `/rest/v1/v_fila_expedicao_csharp` |
| Criar Sessão Saída | POST | `/functions/v1/rfid-session-manager` |
| Criar Sessão Entrada | POST | `/functions/v1/rfid-session-manager` |
| Enviar Tag Saída | POST | `/rest/v1/rfid_saidas_audit` |
| Enviar Tag Entrada | POST | `/rest/v1/rfid_tags_estoque` |
| Finalizar Sessão | POST | `/functions/v1/rfid-session-manager` |
| Cancelar Sessão | POST | `/functions/v1/rfid-session-manager` |
| Heartbeat | POST | `/rest/v1/rpc/device_heartbeat` |

---

## PARTE 7: ERROS COMUNS E SOLUÇÕES

### 7.1 Erro: "Column does not exist"

**Causa**: Nome de coluna incorreto no payload
**Solução**: Verificar tabela de colunas acima e usar nomes corretos

### 7.2 Erro: "Duplicate key value violates unique constraint"

**Causa**: `idempotency_key` duplicado em `rfid_saidas_audit`
**Solução**: OK! O sistema está evitando duplicados. Status 409 é esperado.

### 7.3 Erro: "JWT expired"

**Causa**: Token de autenticação expirado
**Solução**: Renovar token com `RefreshTokenAsync()`

### 7.4 Erro: "permission denied for table"

**Causa**: Usando `apikey` incorreto ou token de usuário inválido
**Solução**: Verificar se está usando ANON_KEY no header `apikey` e ACCESS_TOKEN no `Authorization`

### 7.5 Tags não aparecem na UI

**Causa**: Fluxo de eventos quebrado ou Dispatcher incorreto
**Solução**: 
1. Verificar logs detalhados nos 3 pontos: RfidReaderService → R3DllReader → TagPipeline → ViewModel
2. Confirmar que `SnapshotUpdated` está disparando
3. Confirmar que `RefreshSnapshot()` está sendo chamado
4. Usar `Dispatcher.BeginInvoke()` (não `Invoke()`)

---

Este documento contém TODAS as informações necessárias para o C# Desktop interagir corretamente com o sistema MEPO 2.0.
