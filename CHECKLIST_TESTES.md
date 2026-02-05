# ✅ CHECKLIST DE TESTES - MEPO RFID C# DESKTOP

## 📋 TESTES OBRIGATÓRIOS ANTES DE DEPLOY

---

## 1. AUTENTICAÇÃO E CONECTIVIDADE

### 1.1 Heartbeat
- [ ] Heartbeat envia apenas `p_device_id` no payload
- [ ] Headers separados: `apikey` + `Authorization: Bearer {token}`
- [ ] Resposta 200 OK recebida
- [ ] Timer executa a cada 30 segundos
- [ ] Log não mostra erros de heartbeat

### 1.2 Conexão Hardware
- [ ] Leitor R3 detectado via USB
- [ ] Conexão bem-sucedida (retorna handle válido)
- [ ] Potência configurada corretamente (18 dBm padrão)
- [ ] Log mostra "Conectado ao R3 USB"

---

## 2. FILA DE EXPEDIÇÃO

### 2.1 Carregar Fila
- [ ] View `v_fila_expedicao_csharp` usada (não `v_fila_expedicao`)
- [ ] Status filtrados: `["na_fila", "preparando", "processando"]`
- [ ] Pedidos aparecem na lista da UI
- [ ] Colunas exibidas: número_pedido, cliente, total_itens, status, criado_em
- [ ] Headers corretos: `apikey` + `Authorization`

### 2.2 Exibição na UI
- [ ] Lista de pedidos carrega ao abrir tela
- [ ] Status traduzidos corretamente (na_fila → "Na Fila", etc.)
- [ ] Prioridade exibida (se > 0, mostrar ícone)
- [ ] Pedidos ordenados por `criado_em DESC`
- [ ] Refresh manual funciona (botão atualizar)

---

## 3. SESSÃO DE ENTRADA (Recebimento)

### 3.1 Criar Sessão Entrada
- [ ] Action: `criar_entrada`
- [ ] Campos obrigatórios preenchidos: SKU, Lote
- [ ] Datas opcionais no formato `YYYY-MM-DD`
- [ ] Response contém: `session_id`, `entrada_id` (string), `internal_number`
- [ ] SessionInfo criada no `SessionStateManager`
- [ ] Log mostra "Sessão de entrada ativa: {session_id}"

### 3.2 Leitura de Tags (Entrada)
- [ ] Botão "Iniciar Leitura" não trava UI
- [ ] Tags lidas aparecem na lista `Recent` em tempo real
- [ ] Total de tags atualiza dinamicamente
- [ ] EPC normalizado (uppercase, trimmed)
- [ ] Log mostra todo o fluxo: TagDetected → R3DllReader → TagPipeline → ViewModel

### 3.3 Envio de Tags para Supabase (Entrada)
- [ ] Tabela destino: `rfid_tags_estoque`
- [ ] Campo `entrada_id` é string (UUID)
- [ ] Campos corretos: `tag_rfid`, `sku`, `batch`, `manufacture_date`, `expiration_date`
- [ ] Status inicial: `staged`
- [ ] Batch insert funciona (múltiplas tags de uma vez)
- [ ] Duplicados ignorados (idempotency)

### 3.4 Finalizar Entrada
- [ ] Action: `finalizar_sessao`
- [ ] Response mostra `tags_processed` correto
- [ ] Tags permanecem em `staged` (OMIE manual)
- [ ] Sessão fechada no `SessionStateManager`
- [ ] Campos da UI limpos
- [ ] Log mostra "✅ Entrada finalizada: X tags"

---

## 4. SESSÃO DE SAÍDA (Expedição)

### 4.1 Criar Sessão Saída
- [ ] Action: `criar_saida`
- [ ] Campos obrigatórios: `origem`, `venda_numero`
- [ ] `client_type`: `desktop_csharp`
- [ ] `reader_id` do config utilizado
- [ ] Response contém: `session_id`, `receipt_code`, `existing`
- [ ] Se `existing: true`, sessão reutilizada
- [ ] SessionInfo criada com tipo `Saida`

### 4.2 Carregar Pedido da Fila
- [ ] Duplo-clique no pedido carrega dados
- [ ] Campos preenchidos: número_pedido, cliente, origem
- [ ] Total esperado carregado
- [ ] Itens do pedido exibidos (SKU, quantidade)
- [ ] Sessão criada automaticamente

### 4.3 Leitura de Tags (Saída)
- [ ] Botão "Iniciar Leitura" não trava UI
- [ ] Tags lidas aparecem na lista `Recent`
- [ ] Total de tags atualiza
- [ ] Progress bar atualiza (tags_lidas / total_esperado)
- [ ] Divergências calculadas corretamente
- [ ] Agrupamento por SKU/Lote exibido

### 4.4 Envio de Tags para Supabase (Saída)
- [ ] Tabela destino: `rfid_saidas_audit`
- [ ] Campos obrigatórios: `session_id`, `tag_epc`, `origem`, `venda_numero`, `status`, `quantidade`
- [ ] `status`: `lida`
- [ ] `status_anterior` preenchido (não `status_original`)
- [ ] `idempotency_key`: `{session_id}:{tag_epc}`
- [ ] NÃO enviar: `cmc`, `reader_id`, `lida_em`
- [ ] Batch insert funciona
- [ ] Duplicados retornam 409 (OK)

### 4.5 Finalizar Saída
- [ ] Action: `finalizar_sessao`
- [ ] Response mostra `omie_result.success`
- [ ] Tags movidas para `used` no estoque
- [ ] Pedido marcado como `finalizada` na fila
- [ ] Sessão fechada
- [ ] Log mostra "✅ Saída finalizada"

---

## 5. CONSULTA DE TAG

### 5.1 Busca por EPC
- [ ] Input normaliza EPC (uppercase, trim)
- [ ] Timeout de 15 segundos aplicado
- [ ] View `v_fila_expedicao_csharp` usada
- [ ] Lookup de produto em `produtos` funciona
- [ ] Descrição do produto exibida

### 5.2 Exibição de Resultado
- [ ] Campos exibidos: SKU, Lote, Descrição, Status
- [ ] Datas formatadas: Data Fabricação, Data Validade
- [ ] Status traduzido (staged → "Recebido", available → "Disponível", etc.)
- [ ] Histórico de movimentações exibido (tabela)
- [ ] Log detalhado: "Consultando tag", "Tag encontrada", "DTO retornado"

---

## 6. FLUXO DE EVENTOS RFID

### 6.1 RfidReaderService
- [ ] `TagDetected` event dispara quando tag lida
- [ ] Log: "🔔 RfidReaderService.TagDetected disparado: EPC=..."
- [ ] Deduplicação funciona (janela de 500ms)
- [ ] RSSI calculado corretamente

### 6.2 R3DllReader
- [ ] Subscreve `RfidReaderService.TagDetected`
- [ ] Converte para `RfidTagReadEventArgs`
- [ ] Dispara `TagRead` event
- [ ] Log: "🔔 R3DllReader.TagRead disparado: EPC=..."

### 6.3 TagPipeline
- [ ] Subscreve `R3DllReader.TagRead`
- [ ] Escreve no Channel
- [ ] Log: "🔔 TagPipeline recebeu TagRead: EPC=..."
- [ ] Log: "📝 TagPipeline.Channel.TryWrite = true"
- [ ] `ProcessorLoop` lê do channel
- [ ] Log: "📖 TagPipeline.ProcessorLoop leu do channel: EPC=..."
- [ ] Debounce aplicado (config.DebounceMs)
- [ ] Tag enfileirada para batch insert
- [ ] `SnapshotUpdated` event dispara a cada 80-150ms
- [ ] Log: "🔔 TagPipeline.SnapshotUpdated disparado. Total=X, Recent=Y"

### 6.4 ViewModels (Entrada/Saída)
- [ ] Subscreve `TagPipeline.SnapshotUpdated`
- [ ] `RefreshSnapshot()` chamado
- [ ] Log: "🔔 EntradaViewModel.RefreshSnapshot chamado. Tags no pipeline: X"
- [ ] `Dispatcher.BeginInvoke()` usado (não `Invoke()`)
- [ ] `TotalTags` atualizado
- [ ] `Recent.Clear()` executado
- [ ] Cada tag adicionada ao `Recent`
- [ ] Log: "📋 Adicionando tag à lista Recent: ..."
- [ ] Log final: "✅ EntradaViewModel.Recent atualizado: X tags na lista"

---

## 7. THREADING E DISPATCHER

### 7.1 ConfigureAwait
- [ ] NENHUM `ConfigureAwait(false)` em ViewModels
- [ ] NENHUM `ConfigureAwait(true)` em ViewModels
- [ ] Usar padrão WPF (sem ConfigureAwait)

### 7.2 Dispatcher
- [ ] `Dispatcher.BeginInvoke()` usado para updates assíncronos
- [ ] NUNCA usar `Dispatcher.Invoke()` (bloqueia)
- [ ] Updates de coleções (`ObservableCollection`) no Dispatcher

### 7.3 Background Tasks
- [ ] `BeginReadingAsync()` wrapped em `Task.Run()`
- [ ] Operações de hardware não bloqueiam UI
- [ ] CancellationToken propagado corretamente

---

## 8. BATCH INSERT

### 8.1 BatchTagInsertService (Entrada)
- [ ] Tags enfileiradas em queue thread-safe
- [ ] Flush a cada 3 segundos ou 50 tags (o que vier primeiro)
- [ ] Request com array JSON de tags
- [ ] Header: `Prefer: return=minimal,resolution=ignore-duplicates`
- [ ] Log: "✅ Batch enviado: X tags"

### 8.2 BatchTagInsertService (Saída)
- [ ] Mesma lógica para `rfid_saidas_audit`
- [ ] `idempotency_key` gerado para cada tag
- [ ] Duplicados ignorados (409 = OK)

---

## 9. LOGS E DEBUGGING

### 9.1 Logs Obrigatórios
- [ ] Cada evento dispara log com emoji (🔔, 📝, 📖, ✅, ⚠️, ❌)
- [ ] EPC incluído em cada log
- [ ] Timestamps corretos
- [ ] Nível de log adequado (Info, Warn, Error)

### 9.2 Análise de Fluxo
- [ ] Seguir tag específica do hardware até UI (verificar todos os logs)
- [ ] Identificar ponto de falha se tag não aparece
- [ ] Verificar se todos os eventos dispararam

---

## 10. VALIDAÇÃO DE MODELS C#

### 10.1 FilaItem
- [ ] Todas as propriedades com `[JsonPropertyName]`
- [ ] Tipos corretos: `Guid`, `DateTime`, `DateTime?`, `int`, `string?`

### 10.2 TagItem
- [ ] `VendaNumero` e `Origem` existem
- [ ] `SessionType` enum presente

### 10.3 TagCurrent
- [ ] `DataFabricacao` e `DataValidade` com `[JsonPropertyName]`

### 10.4 TagMovement
- [ ] Todos os campos com `[JsonPropertyName]`

### 10.5 CreateSessionResult / CreateEntradaResult
- [ ] `ErrorMessage` (não apenas `error`)
- [ ] `entrada_id` como `string` (não `Guid`)

### 10.6 TagSaidaPayload
- [ ] `status_anterior` (não `status_original`)
- [ ] NÃO tem: `cmc`, `reader_id`, `lida_em`

### 10.7 TagEntradaPayload
- [ ] `batch` (não `lote`)
- [ ] `manufacture_date` (não `data_fabricacao`)
- [ ] `expiration_date` (não `data_validade`)
- [ ] `tag_rfid` (não `tag_epc`)
- [ ] `entrada_id` como `string`

---

## 11. SMOKE TESTS

### 11.1 Teste Completo Entrada
1. [ ] Abrir tela Entrada
2. [ ] Preencher SKU: `1189`, Lote: `TEST001`
3. [ ] Preencher datas (opcional)
4. [ ] Clicar "Iniciar Leitura"
5. [ ] Aproximar tag RFID do leitor
6. [ ] Verificar tag aparece na lista `Recent`
7. [ ] Total incrementa
8. [ ] Verificar logs completos
9. [ ] Clicar "Finalizar Entrada"
10. [ ] Verificar sucesso

### 11.2 Teste Completo Saída
1. [ ] Abrir tela Saída
2. [ ] Carregar fila de expedição
3. [ ] Duplo-clique em pedido
4. [ ] Sessão criada automaticamente
5. [ ] Clicar "Iniciar Leitura"
6. [ ] Aproximar tags do leitor
7. [ ] Verificar tags aparecem na lista
8. [ ] Progress bar atualiza
9. [ ] Verificar divergências
10. [ ] Clicar "Finalizar Saída"
11. [ ] Verificar integração OMIE (se aplicável)

### 11.3 Teste de Consulta
1. [ ] Abrir tela Consulta
2. [ ] Digitar EPC conhecido
3. [ ] Clicar "Buscar"
4. [ ] Verificar dados exibidos
5. [ ] Verificar descrição do produto
6. [ ] Verificar histórico

---

## 12. TESTES DE ERRO

### 12.1 Sem Hardware
- [ ] Mensagem clara: "Leitor não conectado"
- [ ] Não trava aplicação
- [ ] Permite reconectar

### 12.2 Sem Internet
- [ ] Heartbeat falha gracefully
- [ ] Tags enfileiradas para envio posterior (se implementado)
- [ ] Log mostra erro de rede

### 12.3 Token Expirado
- [ ] Detecta erro 401
- [ ] Tenta refresh automático
- [ ] Redireciona para login se necessário

### 12.4 Pedido Duplicado
- [ ] Sessão existente reutilizada
- [ ] Mensagem: "Sessão já existe para este pedido"
- [ ] Continua normalmente

---

## 13. PERFORMANCE

### 13.1 Leitura Contínua
- [ ] 100+ tags lidas sem travar UI
- [ ] Memória estável (não aumenta indefinidamente)
- [ ] CPU < 30% durante leitura

### 13.2 Batch Insert
- [ ] Envia tags em lotes (não uma por uma)
- [ ] Latência < 500ms por batch
- [ ] Retry em caso de falha (se implementado)

---

## 14. BUILD E DEPLOY

### 14.1 Compilação
- [ ] `.\build.ps1` executa sem erros
- [ ] Apenas warnings (não errors)
- [ ] Executável gerado: `MepoExpedicaoRfid.exe`
- [ ] Tamanho do EXE razoável (< 50 MB)

### 14.2 Dependências
- [ ] `UHFAPI.dll` copiada para bin
- [ ] `appsettings.json` presente
- [ ] Runtimes Win-x86 incluídos

---

## 15. CHECKLIST FINAL PRÉ-DEPLOY

- [ ] Todos os testes acima passaram
- [ ] Logs sem erros críticos
- [ ] UI responsiva e sem travamentos
- [ ] Hardware conecta e desconecta corretamente
- [ ] Tags aparecem em tempo real
- [ ] Integração Supabase 100% funcional
- [ ] Batch inserts funcionando
- [ ] Heartbeat ativo
- [ ] Documentação atualizada
- [ ] README.md reflete estado atual

---

**Status**: 🔴 EM TESTE | 🟡 PARCIAL | 🟢 APROVADO

**Data do Teste**: _______________

**Testado por**: _______________

**Ambiente**: ☐ Desenvolvimento ☐ Homologação ☐ Produção

**Versão**: _______________

---

**NOTAS**:
- Executar TODOS os testes antes de deploy em produção
- Documentar qualquer falha encontrada
- Anexar logs completos de cada teste
- Validar em ambiente real com hardware R3
