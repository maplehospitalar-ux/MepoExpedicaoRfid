# 🔍 AUDITORIA DE MODELS C# - MEPO RFID DESKTOP

**Data**: 04/02/2026  
**Status**: ⚠️ NECESSITA CORREÇÕES

---

## RESUMO EXECUTIVO

Esta auditoria identificou **inconsistências críticas** entre os models C# e o schema do backend Supabase conforme documentação técnica.

### Problemas Encontrados:
1. ✅ **FilaItem**: Falta `[JsonPropertyName]` em todas as propriedades
2. ⚠️ **TagItem**: Usa nomes incorretos (`StatusOriginal`, `StatusNovo`, `Cmc`)
3. ✅ **TagCurrent**: Correto
4. ✅ **TagMovement**: Correto

---

## 1. FilaItem (Models/FilaItem.cs)

### ❌ ESTADO ATUAL
```csharp
public sealed class FilaItem
{
    public string? Id { get; set; }
    public string? SessionId { get; set; }
    public string? NumeroPedido { get; set; }
    public string? Cliente { get; set; }
    public int TotalItens { get; set; }
    public string? Status { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? IniciadoEm { get; set; }
    public DateTime? FinalizadoEm { get; set; }
    public int Prioridade { get; set; }
    public int TagsLidas { get; set; }
    public string? Origem { get; set; }
}
```

### ⚠️ PROBLEMAS
- **CRÍTICO**: Nenhuma propriedade tem `[JsonPropertyName]`
- **CRÍTICO**: Tipo de `Id` deveria ser `Guid` (backend retorna UUID)
- JSON do Supabase usa snake_case, C# usa PascalCase sem atributos

### ✅ CORREÇÃO NECESSÁRIA
```csharp
using System.Text.Json.Serialization;

public sealed class FilaItem
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }

    [JsonPropertyName("numero_pedido")]
    public string? NumeroPedido { get; set; }

    [JsonPropertyName("cliente")]
    public string? Cliente { get; set; }

    [JsonPropertyName("total_itens")]
    public int TotalItens { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

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
    public string? Origem { get; set; }
}
```

---

## 2. TagItem (Models/TagItem.cs)

### ❌ ESTADO ATUAL
```csharp
public class TagItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Epc { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public string? Lote { get; set; }
    public string? Descricao { get; set; }
    public string SessionId { get; set; } = string.Empty;    
    public SessionType Tipo { get; set; } = SessionType.Saida;
    public string? EntradaId { get; set; }
    public string? VendaNumero { get; set; }
    public string? Origem { get; set; }
    public DateTime? DataFabricacao { get; set; }
    public DateTime? DataValidade { get; set; }
    public string StatusOriginal { get; set; } = "disponivel";  // ❌ ERRADO!
    public string StatusNovo { get; set; } = "staged";          // ❌ ERRADO!
    public decimal? Cmc { get; set; }                           // ❌ NÃO EXISTE!
    public int Rssi { get; set; }
    public DateTime LidaEm { get; set; } = DateTime.UtcNow;
    public bool Processada { get; set; } = false;
    public string? ErroMensagem { get; set; }
}
```

### ⚠️ PROBLEMAS

#### CRÍTICO 1: Campos com nomes incorretos
- `StatusOriginal` ❌ → Deveria ser `StatusAnterior` ✅
- `StatusNovo` ❌ → Deveria ser apenas `Status` ✅

#### CRÍTICO 2: Campo inexistente
- `Cmc` ❌ → **NÃO EXISTE** na tabela `rfid_saidas_audit`

#### PROBLEMA 3: Falta `[JsonPropertyName]`
- Usado apenas internamente, mas deveria ter para consistência

### ✅ CORREÇÃO NECESSÁRIA
```csharp
using System.Text.Json.Serialization;

public class TagItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("epc")]
    public string Epc { get; set; } = string.Empty;

    [JsonPropertyName("sku")]
    public string? Sku { get; set; }

    [JsonPropertyName("lote")]
    public string? Lote { get; set; }

    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }

    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = string.Empty;

    [JsonPropertyName("tipo")]
    public SessionType Tipo { get; set; } = SessionType.Saida;

    [JsonPropertyName("entrada_id")]
    public string? EntradaId { get; set; }

    [JsonPropertyName("venda_numero")]
    public string? VendaNumero { get; set; }

    [JsonPropertyName("origem")]
    public string? Origem { get; set; }

    [JsonPropertyName("data_fabricacao")]
    public DateTime? DataFabricacao { get; set; }

    [JsonPropertyName("data_validade")]
    public DateTime? DataValidade { get; set; }

    // ✅ CORRIGIDO: Nome correto da coluna
    [JsonPropertyName("status_anterior")]
    public string? StatusAnterior { get; set; }

    // ✅ CORRIGIDO: Nome correto da coluna
    [JsonPropertyName("status")]
    public string Status { get; set; } = "lida";

    // ❌ REMOVIDO: Cmc (não existe na tabela)

    [JsonPropertyName("rssi")]
    public int Rssi { get; set; }

    [JsonPropertyName("lida_em")]
    public DateTime LidaEm { get; set; } = DateTime.UtcNow;

    public bool Processada { get; set; } = false;
    public string? ErroMensagem { get; set; }
    
    public string IdempotencyKey => $"{SessionId}:{Epc}";  // ✅ Ordem correta!
    public bool IsValida => !string.IsNullOrEmpty(Sku) && !string.IsNullOrEmpty(Lote);
}
```

---

## 3. TagCurrent (Models/TagCurrent.cs)

### ✅ ESTADO ATUAL - **CORRETO**
```csharp
using System.Text.Json.Serialization;

public sealed class TagCurrent
{
    public string Epc { get; set; } = "";
    public string? Sku { get; set; }
    public string? Descricao { get; set; }
    public string? Lote { get; set; }
    public string? Status { get; set; }
    public string? Local { get; set; }
    
    [JsonPropertyName("manufacture_date")]
    public DateTime? DataFabricacao { get; set; }
    
    [JsonPropertyName("expiration_date")]
    public DateTime? DataValidade { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
```

### ✅ APROVADO
- Nomes de colunas em inglês corretos
- `[JsonPropertyName]` aplicado nas propriedades necessárias
- Tipos corretos

---

## 4. TagMovement (Models/TagMovement.cs)

### ✅ ESTADO ATUAL - **CORRETO**
```csharp
using System.Text.Json.Serialization;

public sealed class TagMovement
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("epc")]
    public string Epc { get; set; } = "";
    
    [JsonPropertyName("tipo")]
    public string Tipo { get; set; } = "";
    
    [JsonPropertyName("sku")]
    public string? Sku { get; set; }
    
    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }
    
    [JsonPropertyName("lote")]
    public string? Lote { get; set; }
    
    [JsonPropertyName("numero_pedido")]
    public string? NumeroPedido { get; set; }
    
    [JsonPropertyName("operador")]
    public string? Operador { get; set; }
    
    [JsonPropertyName("local")]
    public string? Local { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}
```

### ✅ APROVADO
- Todos os campos com `[JsonPropertyName]`
- Nomes corretos conforme schema
- Tipos corretos

---

## 5. BatchTagInsertService.cs

### ⚠️ ESTADO ATUAL
```csharp
// InsertSaidaBatchAsync
var payload = JsonSerializer.Serialize(batch.Select(t => new
{
    session_id = t.SessionId,
    tag_epc = t.Epc,
    sku = t.Sku,
    lote = t.Lote,
    status_anterior = t.StatusOriginal,  // ❌ ERRADO! Propriedade não existe mais
    status = t.StatusNovo ?? "used",      // ❌ ERRADO! Propriedade não existe mais
    idempotency_key = $"{t.Epc}_{t.SessionId}",  // ⚠️ Ordem invertida!
    venda_numero = t.VendaNumero,
    origem = t.Origem ?? "desktop_csharp"
}).ToList());
```

### ⚠️ PROBLEMAS
1. Usa `t.StatusOriginal` que não existe mais (deveria ser `t.StatusAnterior`)
2. Usa `t.StatusNovo` que não existe mais (deveria ser `t.Status`)
3. `idempotency_key` usa `{Epc}_{SessionId}` mas deveria ser `{SessionId}:{Epc}`

### ✅ CORREÇÃO NECESSÁRIA
```csharp
// InsertSaidaBatchAsync
var payload = JsonSerializer.Serialize(batch.Select(t => new
{
    session_id = t.SessionId,
    tag_epc = t.Epc,
    sku = t.Sku,
    lote = t.Lote,
    status_anterior = t.StatusAnterior ?? "available",  // ✅ Correto
    status = t.Status ?? "lida",                         // ✅ Correto
    idempotency_key = $"{t.SessionId}:{t.Epc}",        // ✅ Ordem correta
    quantidade = 1,                                      // ✅ Obrigatório
    venda_numero = t.VendaNumero,
    origem = t.Origem ?? "desktop_csharp"
}).ToList());
```

---

## 6. MAPEAMENTO COMPLETO DE COLUNAS

### 6.1 Tabela: `rfid_saidas_audit`

| Coluna DB | Model C# | Obrigatório | Notas |
|-----------|----------|-------------|-------|
| `session_id` | `SessionId` | ✅ | ID da sessão |
| `tag_epc` | `Epc` | ✅ | EPC da tag RFID |
| `sku` | `Sku` | NÃO | Código do produto |
| `lote` | `Lote` | NÃO | Número do lote |
| `origem` | `Origem` | ✅ | OMIE, CONTAAZUL, etc. |
| `venda_numero` | `VendaNumero` | ✅ | Número do pedido |
| `status` | `Status` | ✅ | "lida" (default) |
| `status_anterior` | `StatusAnterior` | NÃO | Status anterior no estoque |
| `quantidade` | - | ✅ | Sempre 1 (hardcoded) |
| `idempotency_key` | `IdempotencyKey` | ✅ | `{session_id}:{tag_epc}` |

**❌ NÃO EXISTEM:**
- `cmc` ❌
- `status_original` ❌ (usar `status_anterior`)
- `reader_id` ❌
- `lida_em` ❌ (usa `created_at` automático)

### 6.2 Tabela: `rfid_tags_estoque`

| Coluna DB | Model C# | Obrigatório | Notas |
|-----------|----------|-------------|-------|
| `entrada_id` | `EntradaId` | ✅ | UUID como string |
| `tag_rfid` | `Epc` | ✅ | EPC da tag RFID |
| `sku` | `Sku` | ✅ | Código do produto |
| `batch` | `Lote` | NÃO | Número do lote |
| `description` | `Descricao` | NÃO | Descrição do produto |
| `manufacture_date` | `DataFabricacao` | NÃO | Data de fabricação |
| `expiration_date` | `DataValidade` | NÃO | Data de validade |
| `status` | `Status` | ✅ | "staged" para entrada |

---

## 7. AÇÕES CORRETIVAS OBRIGATÓRIAS

### PRIORIDADE CRÍTICA:

1. **Corrigir FilaItem.cs**
   - [ ] Adicionar `[JsonPropertyName]` em todas as propriedades
   - [ ] Mudar `Id` de `string` para `Guid`
   - [ ] Compilar e testar

2. **Corrigir TagItem.cs**
   - [ ] Renomear `StatusOriginal` → `StatusAnterior`
   - [ ] Renomear `StatusNovo` → `Status`
   - [ ] Remover propriedade `Cmc`
   - [ ] Adicionar `[JsonPropertyName]` em todas as propriedades
   - [ ] Atualizar `IdempotencyKey` para `{SessionId}:{Epc}`

3. **Corrigir BatchTagInsertService.cs**
   - [ ] Atualizar payload de `InsertSaidaBatchAsync`
   - [ ] Usar `t.StatusAnterior` ao invés de `t.StatusOriginal`
   - [ ] Usar `t.Status` ao invés de `t.StatusNovo`
   - [ ] Corrigir formato de `idempotency_key`
   - [ ] Adicionar campo `quantidade = 1`

4. **Corrigir InsertEstoqueBatchAsync**
   - [ ] Verificar se está usando nomes corretos: `manufacture_date`, `expiration_date`, `batch`
   - [ ] Verificar se `entrada_id` é string (não Guid)

### PRIORIDADE ALTA:

5. **Testar Desserialização**
   - [ ] Testar carga de Fila (FilaItem)
   - [ ] Verificar se todas as propriedades são populadas
   - [ ] Adicionar logs para debug

6. **Validar Payloads**
   - [ ] Comparar JSON enviado com documentação
   - [ ] Adicionar testes unitários para serialização
   - [ ] Validar com Postman/Insomnia

---

## 8. IMPACTO DAS CORREÇÕES

### Funcionalidades Afetadas:
- ✅ **Fila de Expedição**: Desserialização quebrada sem `[JsonPropertyName]`
- ✅ **Envio de Tags de Saída**: Campos incorretos causarão erro 400/500
- ✅ **Envio de Tags de Entrada**: Pode funcionar mas com warnings
- ✅ **Batch Insert**: Payload incorreto causará erros no Supabase

### Riscos:
- **ALTO**: Sistema pode não carregar Fila de Expedição
- **ALTO**: Tags de saída não serão inseridas corretamente
- **MÉDIO**: Batch inserts falharão silenciosamente
- **BAIXO**: TagCurrent e TagMovement já estão corretos

---

## 9. CHECKLIST DE VALIDAÇÃO PÓS-CORREÇÃO

- [ ] FilaItem deserializa corretamente JSON de `v_fila_expedicao_csharp`
- [ ] TagItem não tem mais `StatusOriginal`, `StatusNovo`, `Cmc`
- [ ] BatchTagInsertService usa nomes corretos de propriedades
- [ ] `idempotency_key` gerado no formato `{session_id}:{tag_epc}`
- [ ] Payload de saída contém `quantidade = 1`
- [ ] Payload de entrada usa nomes em inglês (`batch`, `manufacture_date`, etc.)
- [ ] Compilação sem erros
- [ ] Testes unitários passam
- [ ] Teste manual de inserção de tags

---

## 10. CONCLUSÃO

**Status Final**: ⚠️ **NECESSITA CORREÇÕES URGENTES**

Foram identificadas **4 inconsistências críticas** que impedem o funcionamento correto do sistema:

1. FilaItem sem `[JsonPropertyName]` → Fila não carrega
2. TagItem com nomes incorretos → Batch insert falha
3. BatchTagInsertService usa propriedades inexistentes → Runtime error
4. `idempotency_key` com formato incorreto → Duplicados não detectados

**Recomendação**: Aplicar todas as correções antes de próximo deploy em produção.

---

**Auditado por**: GitHub Copilot  
**Data**: 04/02/2026  
**Versão do Sistema**: v1.0.0-beta
