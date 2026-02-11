using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MepoExpedicaoRfid.Models;
using MepoExpedicaoRfid.Services;

namespace MepoExpedicaoRfid.ViewModels;

public partial class SaidaViewModel : ObservableObject
{
    // Resumo do pedido (exibição)
    [ObservableProperty] private string clienteNome = "";
    public ObservableCollection<DocumentoItemResumo> ItensResumo { get; } = new();

    private readonly SupabaseService _supabase;
    private readonly TagPipeline _pipeline;
    private readonly TagHistoryService _tags;
    private readonly NavigationViewModel _nav;
    private readonly AppConfig _cfg;
    private readonly SessionStateManager _session;
    private readonly RealtimeService _realtime;
    private readonly PrintService _printer;
    private readonly AppLogger _log;
    private bool _busyReading = false;  // Previne múltiplas leituras simultâneas

    // Última sessão finalizada (para mostrar resumo + imprimir/copiar)
    [ObservableProperty] private string lastPedidoNumero = "";
    [ObservableProperty] private string lastOrigem = "";
    [ObservableProperty] private string lastClienteNome = "";
    public ObservableCollection<SaidaResumoLinha> LastResumo { get; } = new();
    public ObservableCollection<SaidaResumoLinha> ResumoAtual { get; } = new();

    public IRelayCommand CopiarResumo { get; }
    public IRelayCommand ImprimirResumo { get; }

    [ObservableProperty] private string pedidoNumero = "";
    [ObservableProperty] private string sessionId = "";

    // Origem do pedido (OMIE / CONTAAZUL / LEXOS / MANUAL) selecionada pelo operador
    [ObservableProperty] private string origemSelecionada = "OMIE";

    public ObservableCollection<string> Origens { get; } = new() { "OMIE", "CONTAAZUL", "LEXOS", "MANUAL" };

    [ObservableProperty] private int totalTags;
    [ObservableProperty] private int totalEsperado;
    [ObservableProperty] private int skusUnicos;
    [ObservableProperty] private int lotesUnicos;
    [ObservableProperty] private double progressPercent;
    [ObservableProperty] private int divergencias;
    [ObservableProperty] private string mensagemDivergencia = "";
    public ObservableCollection<string> DivergenciasDetalhe { get; } = new();

    public ObservableCollection<SkuLoteGroupInfo> Groups { get; } = new();
    public ObservableCollection<string> Recent { get; } = new();

    public IAsyncRelayCommand CriarOuAbrirSessao { get; }
    public IAsyncRelayCommand IniciarLeitura { get; }
    public IAsyncRelayCommand PausarLeitura { get; }
    public IAsyncRelayCommand Finalizar { get; }
    public IAsyncRelayCommand Cancelar { get; }
    public IRelayCommand Limpar { get; }

    public SaidaViewModel(SupabaseService supabase, TagPipeline pipeline, TagHistoryService tags, NavigationViewModel nav, AppConfig cfg, SessionStateManager session, RealtimeService realtime, PrintService printer, AppLogger log)
    {
        _supabase = supabase;
        _pipeline = pipeline;
        _tags = tags;
        _nav = nav;
        _cfg = cfg;
        _session = session;
        _realtime = realtime;
        _printer = printer;
        _log = log;

        _pipeline.SnapshotUpdated += (_, __) => RefreshSnapshot();

        CriarOuAbrirSessao = new AsyncRelayCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(PedidoNumero)) return;

            // Regra: apenas uma sessão ativa por vez.
            if (_session.HasActiveSession)
            {
                _log.Warn($"Já existe uma sessão ativa ({_session.CurrentSession?.SessionId}). Finalize/cancele antes de criar outra.");
                return;
            }

            var origem = string.IsNullOrWhiteSpace(OrigemSelecionada)
                ? "OMIE"
                : OrigemSelecionada.Trim().ToUpperInvariant();

            // Se o operador colar um session_id/código do MEPO em vez do número do pedido,
            // tentamos resolver automaticamente.
            var resolved = await _supabase.ResolverNumeroPedidoNoMepoAsync(PedidoNumero).ConfigureAwait(true);
            if (!string.IsNullOrWhiteSpace(resolved) && !string.Equals(resolved, PedidoNumero, StringComparison.OrdinalIgnoreCase))
            {
                _log.Info($"🔎 Pedido informado resolvido via MEPO: '{PedidoNumero}' -> '{resolved}'");
                PedidoNumero = resolved;
            }
            else if (string.IsNullOrWhiteSpace(resolved) && !PedidoNumero.Trim().All(char.IsDigit))
            {
                _log.Warn($"Não consegui resolver o número do pedido no MEPO a partir de '{PedidoNumero}'. Informe o número do pedido (somente dígitos) ou um session_id válido da fila.");
                return;
            }

            var result = await _supabase.CriarSessaoSaidaAsync(origem, PedidoNumero).ConfigureAwait(true);
            if (!result.Success || string.IsNullOrWhiteSpace(result.SessionId))
            {
                _log.Warn($"Falha ao criar sessão de saída: {result.ErrorMessage ?? result.Message}");
                return;
            }

            SessionId = result.SessionId;
            _session.StartSession(new SessionInfo
            {
                SessionId = SessionId,
                Tipo = SessionType.Saida,
                Origem = origem,
                VendaNumero = PedidoNumero,
                ClienteNome = ClienteNome,
                ReaderId = _cfg.Device.Id,
                ClientType = _cfg.Device.ClientType
            });

            _pipeline.ResetSessionCounters();
            await _realtime.BroadcastReaderStartAsync(SessionId);
            _log.Info($"Sessão de saída ativa: {SessionId}");
        });

        IniciarLeitura = new AsyncRelayCommand(async () =>
        {
            // Previne múltiplas leituras simultâneas
            if (_busyReading)
            {
                _log.Warn("⚠️ Leitura já em andamento. Aguarde...");
                return;
            }

            if (string.IsNullOrWhiteSpace(SessionId))
            {
                _log.Warn("Nenhuma sessão de saída ativa. Crie ou abra uma sessão primeiro.");
                return;
            }

            _busyReading = true;
            try
            {
                // ✅ Emite broadcast de reader_start
                await _realtime.BroadcastReaderStartAsync(SessionId);
                
                // ✅ CORRIGIDO: Executa BeginReadingAsync em Task separada para não travar UI
                _log.Info("⏳ Iniciando leitura de saída...");
                
                // Inicia leitura em background task
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _pipeline.BeginReadingAsync();
                        _log.Info("✅ Leitura de saída ativa - tags aparecerão automaticamente");
                    }
                    catch (Exception taskEx)
                    {
                        _log.Error($"❌ Erro ao iniciar leitura em background: {taskEx.Message}", taskEx);
                    }
                });
                
                // Retorna imediatamente para não travar UI
                await Task.Delay(100); // Pequeno delay para garantir que iniciou
            }
            catch (Exception ex)
            {
                _log.Error($"❌ Erro ao iniciar leitura: {ex.Message}", ex);
                _busyReading = false;
            }
        });
        PausarLeitura = new AsyncRelayCommand(async () => 
        {
            if (!_busyReading)
            {
                _log.Warn("⚠️ Nenhuma leitura em andamento");
                return;
            }
            
            _log.Info("⏳ Pausando leitura...");
            _busyReading = false;
            try
            {
                // ✅ Emite broadcast de reader_stop
                if (!string.IsNullOrWhiteSpace(SessionId))
                {
                    await _realtime.BroadcastReaderStopAsync(SessionId);
                }
                
                // CORRIGIDO: Executa em Task separada para não bloquear UI
                await Task.Run(() => _pipeline.EndReadingAsync()).ConfigureAwait(false);
                _log.Info("⏸️ Leitura pausada com sucesso");
            }
            catch (Exception ex)
            {
                _log.Error($"❌ Erro ao pausar: {ex.Message}", ex);
            }
        });

        Finalizar = new AsyncRelayCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(SessionId)) return;

            // Para leitura + força flush antes de finalizar no backend.
            // (Sem isso, a Edge Function pode finalizar e enxergar 0 tags.)
            try { await _realtime.BroadcastReaderStopAsync(SessionId); } catch { }
            try { await _pipeline.EndReadingAsync(); } catch { }
            try { await _pipeline.FlushPendingAsync(); } catch { }
            try { await Task.Delay(250); } catch { }
            try { await _pipeline.FlushPendingAsync(); } catch { }

            var ok = await _supabase.FinalizarSessaoEdgeAsync(SessionId, "saida").ConfigureAwait(true);
            if (ok)
            {
                _log.Info("✅ Sessão finalizada.");
                _busyReading = false;

                // Sugestão rápida ao operador para copiar lote/validade (quando existir)
                try
                {
                    var linhas = ResumoAtual.Count > 0 ? ResumoAtual : LastResumo;
                    var primeira = linhas.FirstOrDefault();
                    if (primeira != null)
                    {
                        var txt = $"SKU: {primeira.Sku}\nDescrição: {primeira.Descricao}\nLote: {primeira.Lote}";
                        System.Windows.Clipboard.SetText(txt);
                        _log.Info("📋 Copiado para a área de transferência: SKU/Descrição/Lote");
                    }
                }
                catch { }

                try
                {
                    System.Windows.MessageBox.Show(
                        "Sessão finalizada.\n\nDica: o primeiro item lido foi copiado (SKU/Descrição/Lote) para você colar onde precisar.",
                        "Finalizar saída",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                }
                catch { }

                // Se houve divergência, alerta operador (e pode virar procedimento de qualidade)
                if (DivergenciasDetalhe.Count > 0)
                {
                    try
                    {
                        var txt = "DIVERGENCIA DETECTADA:\n" + string.Join("\n", DivergenciasDetalhe);
                        System.Windows.MessageBox.Show(txt, "Divergência", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);

                        // Imprime um aviso curto para anexar no pedido
                        _printer.PrintText("*** DIVERGENCIA ***\nPedido: " + PedidoNumero + "\n" + string.Join("\n", DivergenciasDetalhe.Take(12)));
                    }
                    catch { }
                }

                // Guarda resumo da última sessão para exibição + imprimir/copiar
                LastPedidoNumero = PedidoNumero;
                LastOrigem = OrigemSelecionada;
                LastClienteNome = ClienteNome;
                LastResumo.Clear();
                foreach (var r in ResumoAtual) LastResumo.Add(r);

                _pipeline.ResetSessionCounters();
                _session.EndSession();

                // Limpa sessão ativa (mas mantém LastResumo)
                PedidoNumero = "";
                SessionId = "";
            }
        });

        Cancelar = new AsyncRelayCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(SessionId))
            {
                _log.Warn("Cancelar: nenhuma sessão ativa na tela (SessionId vazio)");
                return;
            }

            var sid = SessionId; // captura antes de limpar

            // Sempre tenta parar leitura local (não pode depender do backend)
            try
            {
                await _realtime.BroadcastReaderStopAsync(sid);
            }
            catch (Exception ex)
            {
                _log.Warn($"Cancelar: falha ao enviar reader_stop: {ex.Message}");
            }

            try
            {
                await _pipeline.EndReadingAsync();
            }
            catch (Exception ex)
            {
                _log.Warn($"Cancelar: falha ao parar leitura local: {ex.Message}");
            }
            finally
            {
                _busyReading = false;
            }

            // IMPORTANTE:
            // Cancelar a sessão RFID NÃO deve cancelar/remover o pedido da fila.
            // Padronizamos com o fluxo da ENTRADA: usa Edge Function (rfid-session-manager).
            // Isso evita divergências de permissão/RLS e o problema clássico do p_user_id (UUID) no RPC.
            var ok = false;
            try
            {
                ok = await _supabase.CancelarSessaoEdgeAsync(sid, "Cancelado pelo operador").ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                _log.Warn($"Cancelar: exceção ao chamar cancelar_sessao via Edge Function: {ex.Message}");
            }

            if (!ok)
            {
                // Mesmo se backend falhar, encerra a sessão local para destravar o operador.
                _log.Warn("Cancelar: backend não confirmou cancelamento; encerrando sessão local mesmo assim.");
                try
                {
                    System.Windows.MessageBox.Show(
                        "Não consegui confirmar o cancelamento no MEPO (RPC falhou).\nA sessão foi encerrada localmente para você continuar.\nSe o pedido ficar preso como 'em separação' no MEPO, avise para ajustarmos o backend.",
                        "Cancelar sessão",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Warning);
                }
                catch { }
            }
            else
            {
                _log.Info("⛔ Sessão cancelada (apenas sessão; pedido permanece na fila).");
            }

            _pipeline.ResetSessionCounters();
            _session.CancelSession("Cancelado pelo operador");

            // Mantém PedidoNumero/Origem/Cliente visíveis, mas remove sessão ativa
            SessionId = "";

            // Volta pra Fila (opcional, melhora a dinâmica do operador)
            _nav.Fila?.Execute(null);
        });

        Limpar = new RelayCommand(() =>
        {
            _pipeline.ResetSessionCounters();

            // Se não há sessão ativa, pode limpar TUDO da tela.
            if (!_session.HasActiveSession)
            {
                PedidoNumero = "";
                SessionId = "";
                ClienteNome = "";
                ItensResumo.Clear();
                TotalEsperado = 0;
                ResumoAtual.Clear();
                DivergenciasDetalhe.Clear();
                MensagemDivergencia = "";
                Divergencias = 0;
                ProgressPercent = 0;
                SkusUnicos = 0;
                LotesUnicos = 0;
            }
            else
            {
                _log.Warn("Limpar: existe sessão ativa. Limpei apenas as tags lidas (contador/recents).");
            }
        });

        CopiarResumo = new RelayCommand(() =>
        {
            var text = BuildResumoText(LastResumo.Count > 0 ? LastResumo : ResumoAtual,
                LastResumo.Count > 0 ? LastPedidoNumero : PedidoNumero,
                LastResumo.Count > 0 ? LastOrigem : OrigemSelecionada,
                LastResumo.Count > 0 ? LastClienteNome : ClienteNome);

            if (string.IsNullOrWhiteSpace(text)) return;
            try
            {
                System.Windows.Clipboard.SetText(text);
                _log.Info("📋 Resumo copiado para área de transferência.");
            }
            catch (Exception ex)
            {
                _log.Warn($"Falha ao copiar resumo: {ex.Message}");
            }
        });

        ImprimirResumo = new RelayCommand(() =>
        {
            var text = BuildResumoText(LastResumo.Count > 0 ? LastResumo : ResumoAtual,
                LastResumo.Count > 0 ? LastPedidoNumero : PedidoNumero,
                LastResumo.Count > 0 ? LastOrigem : OrigemSelecionada,
                LastResumo.Count > 0 ? LastClienteNome : ClienteNome);

            if (string.IsNullOrWhiteSpace(text)) return;
            try
            {
                _printer.PrintText(text);
            }
            catch (Exception ex)
            {
                _log.Warn($"Falha ao imprimir: {ex.Message}");
            }
        });

        _realtime.OnReaderStopReceived += async (_, __) =>
        {
            _log.Info("Comando reader_stop recebido do Web");
            if (_busyReading)
            {
                await _pipeline.EndReadingAsync();
                _busyReading = false;
            }
        };

        _realtime.OnSessionCancelReceived += async (_, payload) =>
        {
            var cancelSessionId = payload.TryGetProperty("session_id", out var sid)
                ? sid.GetString()
                : null;
            if (cancelSessionId == SessionId)
            {
                _log.Info("Sessão cancelada remotamente pelo Web");
                await Cancelar.ExecuteAsync(null);
            }
        };

        RefreshSnapshot();
    }

    /// <summary>
    /// Fluxo da Fila (B): operador seleciona pedido na fila; ao abrir, o Desktop cria a sessão.
    /// Mantém o fluxo atual de Saída (leitura/pipeline) e evita sessão fantasma.
    /// </summary>
    public async Task<bool> OpenFromFilaAsync(FilaItem item)
    {
        if (item is null) return false;

        // Regra: apenas uma sessão ativa por vez.
        if (_session.HasActiveSession)
        {
            _log.Warn($"Já existe uma sessão ativa ({_session.CurrentSession?.SessionId}). Finalize/cancele antes de abrir outro pedido.");
            return false;
        }

        var origem = string.IsNullOrWhiteSpace(item.Origem) ? "OMIE" : item.Origem.Trim().ToUpperInvariant();
        OrigemSelecionada = origem;

        // Número do pedido (já vem limpo da view)
        PedidoNumero = item.NumeroPedido ?? "";

        // Resumo (exibição)
        ClienteNome = item.Cliente ?? "";
        ItensResumo.Clear();
        try
        {
            // 1) tenta por documento_id via view v_documentos_comerciais_itens_csharp
            var docId = item.Id;
            _log.Info($"📦 Carregando itens do pedido (view): origem={origem}, numero={PedidoNumero}, documento_id={docId}");
            var itens = await _supabase.GetDocumentoItensResumoAsync(docId).ConfigureAwait(true);
            _log.Info($"📦 Itens carregados por documento_id: count={itens.Count}");

            // 2) fallback robusto: payload pronto (v_pedido_print_payload)
            if (itens.Count == 0)
            {
                var payload = await _supabase.GetPedidoPrintPayloadAsync(origem, PedidoNumero).ConfigureAwait(true);
                if (payload is not null)
                {
                    _log.Info($"📦 Payload encontrado: documento_id={payload.DocumentoId}");
                    if (!string.IsNullOrWhiteSpace(payload.ClienteNome)) ClienteNome = payload.ClienteNome;

                    if (payload.Itens.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        foreach (var el in payload.Itens.EnumerateArray())
                        {
                            ItensResumo.Add(new DocumentoItemResumo
                            {
                                Sku = el.TryGetProperty("sku", out var s) ? s.GetString() : null,
                                Descricao = el.TryGetProperty("descricao", out var d) ? d.GetString() : null,
                                Quantidade = el.TryGetProperty("quantidade", out var q) ? q.GetDecimal() : 0,
                                PrecoUnitario = el.TryGetProperty("preco_unitario", out var pu) && pu.ValueKind != System.Text.Json.JsonValueKind.Null ? pu.GetDecimal() : null,
                                ValorTotal = el.TryGetProperty("valor_total", out var vt) && vt.ValueKind != System.Text.Json.JsonValueKind.Null ? vt.GetDecimal() : null,
                            });
                        }
                    }
                }
            }
            else
            {
                foreach (var it in itens) ItensResumo.Add(it);
            }

            // Total esperado (para progresso/divergência)
            try
            {
                TotalEsperado = (int)Math.Round(ItensResumo.Sum(x => x.Quantidade));
            }
            catch
            {
                TotalEsperado = 0;
            }
        }
        catch (Exception ex)
        {
            _log.Warn($"Não consegui carregar itens do pedido: {ex.Message}");
        }

        // Cria sessão agora (B: ao abrir)
        if (string.IsNullOrWhiteSpace(PedidoNumero))
        {
            _log.Warn("Pedido selecionado não tem numero_pedido.");
            return false;
        }

        var result = await _supabase.CriarSessaoSaidaAsync(origem, PedidoNumero).ConfigureAwait(true);
        if (!result.Success || string.IsNullOrWhiteSpace(result.SessionId))
        {
            _log.Warn($"Falha ao criar sessão de saída: {result.ErrorMessage ?? result.Message}");
            return false;
        }

        SessionId = result.SessionId;
        _session.StartSession(new SessionInfo
        {
            SessionId = SessionId,
            Tipo = SessionType.Saida,
            Origem = origem,
            VendaNumero = PedidoNumero,
            ClienteNome = ClienteNome,
            ReaderId = _cfg.Device.Id,
            ClientType = _cfg.Device.ClientType
        });

        _pipeline.ResetSessionCounters();
        await _realtime.BroadcastReaderStartAsync(SessionId);
        _log.Info($"Sessão de saída ativa (fila): {SessionId}");

        return true;
    }

    /// <summary>
    /// Quando sair da tela de Saída, pausa a sessão atual (evita sessão fantasma) e para leitura.
    /// </summary>
    public async Task PauseOnNavigateAwayAsync()
    {
        try
        {
            if (!_session.HasActiveSession) return;
            if (string.IsNullOrWhiteSpace(SessionId)) return;

            await _realtime.BroadcastReaderStopAsync(SessionId);
            await _pipeline.EndReadingAsync();
            _session.PauseCurrentSession();
            _log.Info($"Sessão pausada ao sair da tela: {SessionId}");
        }
        catch (Exception ex)
        {
            _log.Warn($"Falha ao pausar sessão ao sair da tela: {ex.Message}");
        }
    }

    private void RefreshSnapshot()
    {
        // HOT PATH: sem log por tick (trava UI + cresce log)
        // ✅ Usa BeginInvoke para atualização assíncrona (evita deadlock)
        System.Windows.Application.Current?.Dispatcher.BeginInvoke(() =>
        {
            TotalTags = _pipeline.TotalUniqueTags;

            var groups = _pipeline.Groups;
            Groups.Clear();
            foreach (var g in groups) Groups.Add(g);

            // Resumo atual por SKU/Lote (com descrição quando possível)
            ResumoAtual.Clear();
            foreach (var g in groups)
            {
                // Se não leu nada, não polui UI com DESCONHECIDO/SEM_LOTE
                if (TotalTags == 0) break;
                if (g.Quantidade <= 0) continue;

                var desc = ItensResumo.FirstOrDefault(x => string.Equals(x.Sku, g.Sku, StringComparison.OrdinalIgnoreCase))?.Descricao;
                if (string.IsNullOrWhiteSpace(desc))
                {
                    // fallback: busca descrição no MEPO pelo SKU (async, sem travar UI)
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(g.Sku))
                            {
                                var d = await _supabase.BuscarDescricaoProdutoAsync(g.Sku).ConfigureAwait(false);
                                if (!string.IsNullOrWhiteSpace(d))
                                {
                                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                                    {
                                        // atualiza linha correspondente
                                        var row = ResumoAtual.FirstOrDefault(r => r.Sku == g.Sku && r.Lote == g.Lote);
                                        if (row != null) row.Descricao = d;
                                    });
                                }
                            }
                        }
                        catch { }
                    });
                }

                ResumoAtual.Add(new SaidaResumoLinha { Sku = g.Sku, Descricao = desc, Lote = g.Lote, Quantidade = g.Quantidade });
            }

            Recent.Clear();
            foreach (var t in _pipeline.RecentTags)
                Recent.Add(t);

        SkusUnicos = groups.Select(g => g.Sku).Distinct(StringComparer.OrdinalIgnoreCase).Count();
        LotesUnicos = groups.Select(g => g.Lote).Distinct(StringComparer.OrdinalIgnoreCase).Count();

        // Calcula progress
        if (TotalEsperado > 0)
        {
            ProgressPercent = Math.Min(100, (TotalTags * 100.0 / TotalEsperado));
        }
        else
        {
            ProgressPercent = 0;
        }

        // Divergência (qualidade): só quando existe "esperado".
        // Em MANUAL/sem itens esperados, não mostrar "SKU não esperado".
        DivergenciasDetalhe.Clear();

        var temEsperado = TotalEsperado > 0 && ItensResumo.Count > 0;
        if (temEsperado)
        {
            try
            {
                var esperadoPorSku = ItensResumo
                    .Where(x => !string.IsNullOrWhiteSpace(x.Sku))
                    .GroupBy(x => x.Sku!.Trim().ToUpperInvariant())
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantidade));

                var lidoPorSku = groups
                    .GroupBy(g => (g.Sku ?? "").Trim().ToUpperInvariant())
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantidade));

                foreach (var kv in lidoPorSku.OrderBy(k => k.Key))
                {
                    var sku = kv.Key;
                    if (string.IsNullOrWhiteSpace(sku)) continue;
                    if (kv.Value <= 0) continue;

                    if (!esperadoPorSku.TryGetValue(sku, out var exp))
                    {
                        DivergenciasDetalhe.Add($"SKU não esperado: {sku} (lido {kv.Value})");
                    }
                    else
                    {
                        var diff = kv.Value - exp;
                        if (Math.Abs(diff) > 0.0001m)
                            DivergenciasDetalhe.Add($"SKU {sku}: esperado {exp} / lido {kv.Value}");
                    }
                }

                foreach (var kv in esperadoPorSku.OrderBy(k => k.Key))
                {
                    var sku = kv.Key;
                    if (kv.Value <= 0) continue;
                    if (!lidoPorSku.ContainsKey(sku))
                        DivergenciasDetalhe.Add($"SKU faltando: {sku} (esperado {kv.Value})");
                }

                if (lidoPorSku.TryGetValue("DESCONHECIDO", out var unk) && unk > 0)
                    DivergenciasDetalhe.Add($"Atenção: {unk} tags com SKU DESCONHECIDO");
            }
            catch { }

            var diffTotal = TotalTags - TotalEsperado;
            Divergencias = Math.Abs(diffTotal);

            if (DivergenciasDetalhe.Count > 0)
                MensagemDivergencia = $"⚠️ Divergência detectada ({DivergenciasDetalhe.Count} itens)";
            else if (diffTotal < 0)
                MensagemDivergencia = $"⚠️ Faltam {Divergencias} itens";
            else if (diffTotal > 0)
                MensagemDivergencia = $"⚠️ +{Divergencias} itens excedentes";
            else
                MensagemDivergencia = "✅ Quantidade correta";
        }
        else
        {
            Divergencias = 0;
            MensagemDivergencia = "";
        }
        }); // Fecha Dispatcher.BeginInvoke
    }

    private static string BuildResumoText(IEnumerable<SaidaResumoLinha> linhas, string pedido, string origem, string cliente)
    {
        var list = (linhas ?? Array.Empty<SaidaResumoLinha>()).ToList();
        if (list.Count == 0) return string.Empty;

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Pedido: {pedido}  Origem: {origem}");
        if (!string.IsNullOrWhiteSpace(cliente)) sb.AppendLine($"Cliente: {cliente}");
        sb.AppendLine(new string('-', 32));

        foreach (var l in list.OrderBy(x => x.Sku).ThenBy(x => x.Lote))
        {
            var desc = string.IsNullOrWhiteSpace(l.Descricao) ? "" : (" - " + l.Descricao);
            sb.AppendLine($"SKU: {l.Sku}{desc} - Lote {l.Lote} (qtd {l.Quantidade:00})");
        }

        return sb.ToString();
    }
}
