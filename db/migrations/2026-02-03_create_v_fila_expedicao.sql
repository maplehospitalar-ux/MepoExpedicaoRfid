-- Create or replace view v_fila_expedicao
-- Required by Desktop C# FilaService

CREATE OR REPLACE VIEW public.v_fila_expedicao AS
SELECT 
  dc.id,
  dc.numero,
  dc.origem,
  dc.cliente_nome,
  dc.valor_total,
  dc.observacao_expedicao,
  dc.created_at,
  COALESCE(s.session_id, NULL) as session_id,
  COALESCE(s.status, 'pendente') as status_expedicao,
  COALESCE(s.tags_count, 0) as tags_lidas
FROM documentos_comerciais dc
LEFT JOIN rfid_saidas_sessions s 
  ON s.venda_numero = dc.numero 
  AND s.origem = dc.origem
  AND s.status IN ('preparando', 'processando')
WHERE dc.tipo = 'PEDIDO' 
  AND dc.status_fila = 'FILA_EXPEDICAO'
ORDER BY 
  CASE WHEN s.status = 'processando' THEN 0 
       WHEN s.status = 'preparando' THEN 1 
       ELSE 2 END,
  dc.created_at;
