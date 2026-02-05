-- ================================================
-- MIGRAÇÃO: Views de Compatibilidade C# Desktop
-- Data: 2026-02-04
-- Descrição: Cria views necessárias para compatibilidade
--            entre C# Desktop e MEPO Web
-- ================================================

-- ================================================
-- 1. v_tag_historico_completo
-- Histórico unificado de tags RFID
-- Compatível com C# SupabaseService.GetTagHistoricoAsync
-- ================================================

CREATE OR REPLACE VIEW public.v_tag_historico_completo AS
WITH 
-- Entradas de tags no estoque
tag_entradas AS (
  SELECT 
    t.id::TEXT as id,
    t.tag_rfid as epc,
    'entrada'::TEXT as tipo,
    t.sku,
    p.descricao,
    t.batch as lote,
    e.internal_number as numero_pedido,
    NULL::TEXT as operador,
    'ESTOQUE'::TEXT as local,
    t.created_at
  FROM rfid_tags_estoque t
  LEFT JOIN produtos p ON p.sku = t.sku
  LEFT JOIN entradas_estoque e ON e.id = t.entrada_id
),

-- Movimentações (ajustes, inventários, etc)
tag_movimentos AS (
  SELECT 
    m.id::TEXT,
    t.tag_rfid as epc,
    COALESCE(m.tipo, 'movimento')::TEXT as tipo,
    COALESCE(m.sku, t.sku) as sku,
    p.descricao,
    COALESCE(m.lote, t.batch) as lote,
    NULL::TEXT as numero_pedido,
    NULL::TEXT as operador,
    m.origem as local,
    COALESCE(m.data_movimento, m.created_at) as created_at
  FROM rfid_tag_movimentos m
  INNER JOIN rfid_tags_estoque t ON t.id = m.tag_id
  LEFT JOIN produtos p ON p.sku = COALESCE(m.sku, t.sku)
),

-- Expedições/Saídas
tag_saidas AS (
  SELECT 
    s.id::TEXT,
    s.tag_epc as epc,
    'saida'::TEXT as tipo,
    s.sku,
    p.descricao,
    s.lote,
    s.venda_numero as numero_pedido,
    NULL::TEXT as operador,
    s.origem as local,
    s.created_at
  FROM rfid_saidas_audit s
  LEFT JOIN produtos p ON p.sku = s.sku
)

SELECT * FROM tag_entradas
UNION ALL
SELECT * FROM tag_movimentos
UNION ALL
SELECT * FROM tag_saidas
ORDER BY created_at DESC;

-- Permissões
GRANT SELECT ON public.v_tag_historico_completo TO authenticated;
GRANT SELECT ON public.v_tag_historico_completo TO service_role;
GRANT SELECT ON public.v_tag_historico_completo TO anon;

COMMENT ON VIEW public.v_tag_historico_completo IS 
'Histórico unificado de tags RFID. Combina entradas, movimentações e saídas. Compatível com C# Desktop GetTagHistoricoAsync.';

-- ================================================
-- 2. v_fila_expedicao_csharp
-- Fila de expedição com colunas renomeadas
-- Compatível com FilaItem do C#
-- CORRIGIDO: Usa status em PORTUGUÊS (preparando, processando)
-- CORRIGIDO: Inclui pedidos da fila de documentos_comerciais
-- ================================================

CREATE OR REPLACE VIEW public.v_fila_expedicao_csharp AS
-- Pedidos na fila (aguardando processamento)
SELECT 
  dc.id,
  NULL::TEXT as session_id,
  dc.numero as numero_pedido,
  COALESCE(dc.cliente_nome, 'Cliente não informado') as cliente,
  COALESCE(
    (SELECT COUNT(*)::INTEGER FROM documentos_comerciais_itens WHERE documento_id = dc.id),
    0
  ) as total_itens,
  'na_fila' as status,
  dc.created_at as criado_em,
  dc.enviado_para_fila_at as iniciado_em,
  NULL::TIMESTAMPTZ as finalizado_em,
  0 as prioridade,
  0 as tags_lidas,
  dc.origem
FROM documentos_comerciais dc
WHERE dc.status_expedicao = 'preparando'
  AND dc.tipo = 'PEDIDO'
  AND dc.cancelado = false

UNION ALL

-- Sessões ativas (em processamento RFID)
SELECT 
  s.id,
  s.session_id,
  s.venda_numero as numero_pedido,
  COALESCE(dc.cliente_nome, 'Cliente não informado') as cliente,
  COALESCE(
    (SELECT COUNT(*)::INTEGER FROM rfid_saidas_audit a WHERE a.session_id = s.session_id),
    0
  ) as total_itens,
  s.status,
  s.created_at as criado_em,
  s.created_at as iniciado_em,
  s.finalized_at as finalizado_em,
  0 as prioridade,
  COALESCE(s.total_tags_received, 0) as tags_lidas,
  s.origem
FROM rfid_saidas_sessions s
LEFT JOIN documentos_comerciais dc 
  ON dc.numero = s.venda_numero 
  AND dc.tipo = 'PEDIDO'
WHERE s.status IN ('preparando', 'processando')  -- STATUS CORRETOS EM PORTUGUÊS!
ORDER BY 
  CASE status
    WHEN 'processando' THEN 1
    WHEN 'preparando' THEN 2
    WHEN 'na_fila' THEN 3
  END,
  criado_em DESC;

-- Permissões
GRANT SELECT ON public.v_fila_expedicao_csharp TO authenticated;
GRANT SELECT ON public.v_fila_expedicao_csharp TO service_role;
GRANT SELECT ON public.v_fila_expedicao_csharp TO anon;

COMMENT ON VIEW public.v_fila_expedicao_csharp IS 
'Fila de expedição compatível com C# Desktop. Colunas renomeadas para match com FilaItem.';

-- ================================================
-- 3. Índices de Performance (Opcional)
-- ================================================

-- Índice para busca de tags por EPC
CREATE INDEX IF NOT EXISTS idx_rfid_tags_estoque_tag_rfid 
ON rfid_tags_estoque(tag_rfid);

-- Índice para movimentos por tag_id
CREATE INDEX IF NOT EXISTS idx_rfid_tag_movimentos_tag_id 
ON rfid_tag_movimentos(tag_id);

-- Índice para saídas por session_id
CREATE INDEX IF NOT EXISTS idx_rfid_saidas_audit_session_id 
ON rfid_saidas_audit(session_id);

-- Índice para saídas por tag_epc
CREATE INDEX IF NOT EXISTS idx_rfid_saidas_audit_tag_epc 
ON rfid_saidas_audit(tag_epc);

-- ================================================
-- 4. Testes de Validação
-- ================================================

-- Teste 1: v_tag_historico_completo retorna dados
DO $$
DECLARE
  rec_count INTEGER;
BEGIN
  SELECT COUNT(*) INTO rec_count FROM v_tag_historico_completo;
  RAISE NOTICE 'v_tag_historico_completo: % registros encontrados', rec_count;
END $$;

-- Teste 2: v_fila_expedicao_csharp retorna dados
DO $$
DECLARE
  rec_count INTEGER;
BEGIN
  SELECT COUNT(*) INTO rec_count FROM v_fila_expedicao_csharp;
  RAISE NOTICE 'v_fila_expedicao_csharp: % registros encontrados', rec_count;
END $$;

-- ================================================
-- FIM DA MIGRAÇÃO
-- ================================================
