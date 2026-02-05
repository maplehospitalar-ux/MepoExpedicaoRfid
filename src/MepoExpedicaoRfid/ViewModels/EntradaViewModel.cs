using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MepoExpedicaoRfid.Models;
using MepoExpedicaoRfid.Services;

namespace MepoExpedicaoRfid.ViewModels;

public partial class EntradaViewModel : ObservableObject
{
    private readonly SupabaseService _supabase;
    private readonly TagPipeline _pipeline;
    private readonly NavigationViewModel _nav;
    private readonly AppConfig _cfg;
    private readonly SessionStateManager _session;
    private readonly AppLogger _log;
    private bool _busyReading = false;  // Previne múltiplas leituras simultâneas

    [ObservableProperty] private string sku = "";
    [ObservableProperty] private string descricao = "";
    [ObservableProperty] private string lote = "";
    [ObservableProperty] private DateTime? dataFabricacao;
    [ObservableProperty] private DateTime? dataValidade;
    [ObservableProperty] private int totalTags;
    [ObservableProperty] private string sessionId = "";
    [ObservableProperty] private string entradaId = "";
    [ObservableProperty] private bool isReading = false;

    public ObservableCollection<string> Recent { get; } = new();

    public IAsyncRelayCommand IniciarLeitura { get; }
    public IAsyncRelayCommand PararLeitura { get; }
    public IAsyncRelayCommand FinalizarEntrada { get; }
    public IAsyncRelayCommand Cancelar { get; }
    public IRelayCommand Limpar { get; }

    public EntradaViewModel(SupabaseService supabase, TagPipeline pipeline, NavigationViewModel nav, AppConfig cfg, SessionStateManager session, AppLogger log)
    {
        _supabase = supabase;
        _pipeline = pipeline;
        _nav = nav;
        _cfg = cfg;
        _session = session;
        _log = log;

        _pipeline.SnapshotUpdated += (_, __) => RefreshSnapshot();

        IniciarLeitura = new AsyncRelayCommand(async () =>
        {
            // Previne múltiplas leituras simultâneas
            if (_busyReading || IsReading)
            {
                _log.Warn("⚠️ Leitura já em andamento. Aguarde...");
                return;
            }

            if (string.IsNullOrWhiteSpace(Sku) || string.IsNullOrWhiteSpace(Lote))
            {
                _log.Warn("SKU e Lote são obrigatórios para iniciar entrada");
                return;
            }

            // ✅ Verifica se já existe sessão ativa de tipo diferente
            var currentSession = _session.CurrentSession;
            if (currentSession != null && currentSession.Tipo != SessionType.Entrada)
            {
                _log.Warn($"⚠️ Já existe uma sessão ativa de {currentSession.Tipo}. Finalize-a primeiro.");
                return;
            }

            // ✅ CRÍTICO: Executa TUDO em background para não travar UI
            _ = Task.Run(async () =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(SessionId))
                    {
                        _log.Info("Criando sessão de entrada...");
                        var result = await _supabase.CriarSessaoEntradaAsync(Sku, Lote, DataFabricacao, DataValidade);
                        if (!result.Success || string.IsNullOrWhiteSpace(result.SessionId))
                        {
                            _log.Warn($"Falha ao criar sessão de entrada: {result.ErrorMessage ?? result.Message}");
                            return;
                        }

                        // Atualiza propriedades na UI thread
                        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            SessionId = result.SessionId;
                            EntradaId = result.EntradaId ?? "";
                        });

                        _session.StartSession(new SessionInfo
                        {
                            SessionId = result.SessionId,
                            Tipo = SessionType.Entrada,
                            Sku = Sku,
                            Lote = Lote,
                            EntradaId = result.EntradaId ?? "",
                            DataFabricacao = DataFabricacao,
                            DataValidade = DataValidade,
                            ReaderId = _cfg.Device.Id,
                            ClientType = _cfg.Device.ClientType
                        });

                        _pipeline.ResetSessionCounters();
                        _log.Info($"Sessão de entrada ativa: {result.SessionId}");
                    }

                    // Define flags
                    _busyReading = true;
                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        IsReading = true;
                    });
                    
                    _log.Info("⏳ Iniciando leitura de entrada...");
                    
                    await _pipeline.BeginReadingAsync();
                    _log.Info("✅ Leitura de entrada ativa - tags aparecerão automaticamente");
                }
                catch (Exception ex)
                {
                    _log.Error($"❌ Erro ao iniciar leitura: {ex.Message}", ex);
                    _busyReading = false;
                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        IsReading = false;
                    });
                }
            });
        });
        PararLeitura = new AsyncRelayCommand(async () => 
        {
            // ✅ Verifica pela sessão ativa, não pela flag
            var currentSession = _session.CurrentSession;
            if (currentSession == null || currentSession.Status != SessionStatus.Ativa)
            {
                _log.Warn("⚠️ Nenhuma sessão ativa");
                return;
            }
            
            _log.Info("⏳ Pausando leitura...");
            try
            {
                await _pipeline.EndReadingAsync();
                _busyReading = false;
                IsReading = false;
                _log.Info("⏸️ Leitura pausada com sucesso");
            }
            catch (Exception ex)
            {
                _log.Error($"❌ Erro ao pausar: {ex.Message}", ex);
            }
        });
        
        FinalizarEntrada = new AsyncRelayCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(Sku) || string.IsNullOrWhiteSpace(Lote))
            {
                _log.Warn("SKU e Lote são obrigatórios para finalizar entrada");
                return;
            }

            await _pipeline.EndReadingAsync();
            if (!string.IsNullOrWhiteSpace(SessionId))
            {
                await _supabase.FinalizarSessaoEdgeAsync(SessionId, "entrada");
                _session.EndSession();
            }

            _log.Info($"✅ Entrada finalizada: {TotalTags} tags - SKU: {Sku}, Lote: {Lote}");
            
            // Limpa flags
            _busyReading = false;
            IsReading = false;
            
            // Limpa campos
            Sku = "";
            Descricao = "";
            Lote = "";
            DataFabricacao = null;
            DataValidade = null;
            SessionId = "";
            EntradaId = "";
            _pipeline.ResetSessionCounters();
        });
        
        Cancelar = new AsyncRelayCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(SessionId)) return;
            
            var ok = await _supabase.CancelarSessaoEdgeAsync(SessionId, "Cancelado pelo operador").ConfigureAwait(true);
            if (ok)
            {
                _log.Info("⛔ Sessão de entrada cancelada.");
                _pipeline.ResetSessionCounters();
                _session.CancelSession("Cancelado pelo operador");
                
                // Limpa flags
                _busyReading = false;
                IsReading = false;
                
                // Limpa campos
                Sku = "";
                Descricao = "";
                Lote = "";
                DataFabricacao = null;
                DataValidade = null;
                SessionId = "";
                EntradaId = "";
            }
        });
        
        Limpar = new RelayCommand(() =>
        {
            _pipeline.ResetSessionCounters();
            Sku = "";
            Descricao = "";
            Lote = "";
            DataFabricacao = null;
            DataValidade = null;
            SessionId = "";
            EntradaId = "";
        });

        RefreshSnapshot();
    }

    private void RefreshSnapshot()
    {
        _log.Info($"🔔 EntradaViewModel.RefreshSnapshot chamado. Tags no pipeline: {_pipeline.TotalUniqueTags}");
        // ✅ CORRIGIDO: Usa BeginInvoke para atualização assíncrona (evita deadlock)
        System.Windows.Application.Current?.Dispatcher.BeginInvoke(() =>
        {
            TotalTags = _pipeline.TotalUniqueTags;
            Recent.Clear();
            foreach (var t in _pipeline.RecentTags) 
            {
                _log.Info($"  📋 Adicionando tag à lista Recent: {t}");
                Recent.Add(t);
            }
            _log.Info($"✅ EntradaViewModel.Recent atualizado: {Recent.Count} tags na lista");
        });
    }
}
