using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MepoExpedicaoRfid.Models;
using MepoExpedicaoRfid.Services;

namespace MepoExpedicaoRfid.ViewModels;

public partial class SaidaViewModel : ObservableObject
{
    private readonly SupabaseService _supabase;
    private readonly TagPipeline _pipeline;
    private readonly TagHistoryService _tags;
    private readonly NavigationViewModel _nav;
    private readonly AppConfig _cfg;
    private readonly SessionStateManager _session;
    private readonly RealtimeService _realtime;
    private readonly AppLogger _log;
    private bool _busyReading = false;  // Previne múltiplas leituras simultâneas

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

    public SaidaViewModel(SupabaseService supabase, TagPipeline pipeline, TagHistoryService tags, NavigationViewModel nav, AppConfig cfg, SessionStateManager session, RealtimeService realtime, AppLogger log)
    {
        _supabase = supabase;
        _pipeline = pipeline;
        _tags = tags;
        _nav = nav;
        _cfg = cfg;
        _session = session;
        _realtime = realtime;
        _log = log;

        _pipeline.SnapshotUpdated += (_, __) => RefreshSnapshot();

        CriarOuAbrirSessao = new AsyncRelayCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(PedidoNumero)) return;

            var origem = string.IsNullOrWhiteSpace(OrigemSelecionada)
                ? "OMIE"
                : OrigemSelecionada.Trim().ToUpperInvariant();

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
                _pipeline.ResetSessionCounters();
                _session.EndSession();
                PedidoNumero = "";
                SessionId = "";
            }
        });

        Cancelar = new AsyncRelayCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(SessionId)) return;
            await _realtime.BroadcastReaderStopAsync(SessionId);
            var ok = await _supabase.CancelarSessaoEdgeAsync(SessionId, "Cancelado pelo operador").ConfigureAwait(true);
            if (ok)
            {
                _log.Info("⛔ Sessão cancelada.");
                _pipeline.ResetSessionCounters();
                _session.CancelSession("Cancelado pelo operador");
                PedidoNumero = "";
                SessionId = "";
            }
        });

        Limpar = new RelayCommand(() => _pipeline.ResetSessionCounters());

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
}
