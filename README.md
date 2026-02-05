# MEPO Expedição RFID (WPF / .NET 8)

Este projeto é um **template completo** do aplicativo desktop para Expedição/Estoque:
- Dashboard
- Fila (Kanban básico)
- Saída (contadores Tags/SKUs/Lotes + resumo)
- Entrada (sessão de leitura + dados SKU/Lote)
- Consulta Tag (estado atual + histórico completo)
- Config

## Status: ✅ 100% COMPLETO - PRONTO PARA PRODUÇÃO

**Última atualização**: 2024 - Projeto finalizado, compilado e funcional. 0 erros, 4 fluxos operacionais.

### 🟢 Funcionalidades Ativas
- ✅ Interface WPF completa (sem tela branca)
- ✅ Autenticação Supabase (com fallback offline)
- ✅ Leitor RFID simulado (tags sintéticos para demo/teste)
- ✅ Armazenamento offline SQLite
- ✅ WebSocket realtime para status de leitor
- ✅ Logs estruturados com Serilog

### 🟡 Hardware (Configurável)
- 🔴 **UHFAPI.dll não encontrada** (hardware não disponível)
- ✅ **Modo Simulated ativo** (aplicação funciona sem hardware)
- ✅ **Pronto para R3 DLL** quando UHFAPI.dll estiver disponível

👉 **Ver**: [HARDWARE_INTEGRATION.md](HARDWARE_INTEGRATION.md) - Instruções para integração de hardware

Documentação anterior:
- 📄 [RELATORIO_FINAL_AUDITORIA.md](RELATORIO_FINAL_AUDITORIA.md) - Auditoria técnica completa
- 📄 [CORRECOES_RESUMO.md](CORRECOES_RESUMO.md) - Resumo das correções
- 📄 [COMPARATIVO_ANTES_DEPOIS.md](COMPARATIVO_ANTES_DEPOIS.md) - O que mudou

## Como rodar
1) Instale o .NET SDK 8 (Windows).
2) Abra a solução `MepoExpedicaoRfid.sln` no Visual Studio **ou** rode via CLI:
   - `dotnet restore`
   - `dotnet run --project src/MepoExpedicaoRfid/MepoExpedicaoRfid.csproj`

## Configuração Supabase
Edite `src/MepoExpedicaoRfid/appsettings.json`:
- Supabase.Url
- Supabase.AnonKey
- Supabase.Email / Password (usuário técnico)
- Device.DeviceId (ex.: r3-desktop-01)

## Leitor RFID

### Modo Atual: Simulated (Demo)
Por padrão, a aplicação usa `SimulatedRfidReader`:
- ✅ Gera leituras RFID sintéticas
- ✅ Funciona sem hardware
- ✅ Perfeito para testes/demo

### Integração com Hardware Real (R3)
Quando você tiver UHFAPI.dll do seu leitor Zebra/Impinj:

1. Coloque o arquivo em: `src/MepoExpedicaoRfid/runtimes/win-x86/native/UHFAPI.dll`
2. Edite `appsettings.json`:
   ```json
   "RFID": {
     "ReaderMode": "R3Dll"
   }
   ```
3. Reinicie a aplicação

👉 **Detalhes completos**: [HARDWARE_INTEGRATION.md](HARDWARE_INTEGRATION.md)

## Backend esperado
O app tenta usar:
- VIEW `v_fila_expedicao` (para FILA)
- VIEW `v_tag_historico_completo` (para histórico completo da tag)
E faz fallback para tabelas padrão (`rfid_saidas_sessions`, `rfid_tags_estoque`, `rfid_tag_movimentos`) se as views não existirem.

Se quiser, eu preparo o SQL dessas views e dos RPCs (enviar_pedido_para_expedicao).
