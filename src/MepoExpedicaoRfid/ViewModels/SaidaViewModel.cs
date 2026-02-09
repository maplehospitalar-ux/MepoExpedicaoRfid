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
            await _realtime.BroadcastReaderStopAsync(SessionId);
            var ok = await _supabase.FinalizarSessaoEdgeAsync(SessionId, "saida").ConfigureAwait(true);
            if (ok)
            {
                _log.Info("✅ Sessão finalizada.");

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
            if (string.IsNullOrWhiteSpace(SessionId)) return;

            await _realtime.BroadcastReaderStopAsync(SessionId);

            // IMPORTANTE:
            // Cancelar a sessão RFID NÃO deve cancelar/remover o pedido da fila.
            // O edge function (rfid-session-manager: cancelar_sessao) pode alterar status do pedido no MEPO.
            // Aqui usamos o RPC cancelar_sessao_rfid (sessão) para manter o pedido como pendente.
            var ok = await _supabase.CancelarSessaoAsync(SessionId, _cfg.Device.Id).ConfigureAwait(true);
            if (ok)
            {
                _log.Info("⛔ Sessão cancelada (apenas sessão; pedido permanece na fila).");
                _pipeline.ResetSessionCounters();
                _session.CancelSession("Cancelado pelo operador");

                // Mantém PedidoNumero/Origem/Cliente visíveis, mas remove sessão ativa
                SessionId = "";

                // Volta pra Fila (opcional, melhora a dinâmica do operador)
                _nav.Fila?.Execute(null);
            }
        });

        Limpar = new RelayCommand(() => _pipeline.ResetSessionCounters());

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
        _log.Info($"🔔 SaidaViewModel.RefreshSnapshot chamado. Tags no pipeline: {_pipeline.TotalUniqueTags}");
        // ✅ CORRIGIDO: Usa BeginInvoke para atualização assíncrona (evita deadlock)
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
                var desc = ItensResumo.FirstOrDefault(x => string.Equals(x.Sku, g.Sku, StringComparison.OrdinalIgnoreCase))?.Descricao;
                ResumoAtual.Add(new SaidaResumoLinha { Sku = g.Sku, Descricao = desc, Lote = g.Lote, Quantidade = g.Quantidade });
            }

            Recent.Clear();
            foreach (var t in _pipeline.RecentTags) 
            {
                _log.Info($"  📋 Adicionando tag à lista Recent: {t}");
                Recent.Add(t);
            }
            _log.Info($"✅ SaidaViewModel.Recent atualizado: {Recent.Count} tags na lista");

        SkusUnicos = groups.Select(g => g.Sku).Distinct(StringComparer.OrdinalIgnoreCase).Count();
        LotesUnicos = groups.Select(g => g.Lote).Distinct(StringComparer.OrdinalIgnoreCase).Count();

        // Calcula progress
        if (TotalEsperado > 0)
        {
            ProgressPercent = Math.Min(100, (TotalTags * 100.0 / TotalEsperado));
            
            // Calcula divergências
            var diff = TotalTags - TotalEsperado;
            Divergencias = Math.Abs(diff);

            if (diff < 0)
            {
                MensagemDivergencia = $"⚠️ Faltam {Divergencias} itens";
            }
            else if (diff > 0)
            {
                MensagemDivergencia = $"⚠️ +{Divergencias} itens excedentes";
            }
            else
            {
                MensagemDivergencia = "✅ Quantidade correta";
            }
        }
        else
        {
            ProgressPercent = 0;
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
