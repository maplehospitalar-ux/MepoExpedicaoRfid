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
    private readonly RealtimeService _realtime;
    private readonly AppLogger _log;
    private bool _busyReading = false;  // Previne múltiplas leituras simultâneas
    private bool _busyCreatingSession = false; // Previne clique duplo em Criar Sessão
    private CancellationTokenSource? _skuLookupCts;

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
    public ObservableCollection<TagConflict> Conflicts { get; } = new();

    private readonly HashSet<string> _conflictChecked = new(StringComparer.OrdinalIgnoreCase);

    public IAsyncRelayCommand CriarSessao { get; }
    public IAsyncRelayCommand IniciarLeitura { get; }
    public IAsyncRelayCommand PararLeitura { get; }
    public IAsyncRelayCommand FinalizarEntrada { get; }
    public IAsyncRelayCommand Cancelar { get; }
    public IRelayCommand Limpar { get; }

    public IAsyncRelayCommand<TagConflict> AtualizarTagConflito { get; }
    public IRelayCommand<TagConflict> ExcluirConflito { get; }

    public EntradaViewModel(SupabaseService supabase, TagPipeline pipeline, NavigationViewModel nav, AppConfig cfg, SessionStateManager session, RealtimeService realtime, AppLogger log)
    {
        _supabase = supabase;
        _pipeline = pipeline;
        _nav = nav;
        _cfg = cfg;
        _session = session;
        _realtime = realtime;
        _log = log;

        _pipeline.SnapshotUpdated += (_, __) => RefreshSnapshot();

        CriarSessao = new AsyncRelayCommand(async () =>
        {
            if (_busyCreatingSession)
            {
                _log.Warn("⚠️ Criação de sessão já em andamento. Aguarde...");
                return;
            }

            if (string.IsNullOrWhiteSpace(Sku) || string.IsNullOrWhiteSpace(Lote))
            {
                _log.Warn("SKU e Lote são obrigatórios para criar sessão de entrada");
                return;
            }

            // ✅ Verifica se já existe sessão ativa de tipo diferente
            var currentSession = _session.CurrentSession;
            if (currentSession != null && currentSession.Tipo != SessionType.Entrada)
            {
                _log.Warn($"⚠️ Já existe uma sessão ativa de {currentSession.Tipo}. Finalize-a primeiro.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(SessionId))
            {
                _log.Warn($"Já existe uma sessão de entrada ativa: {SessionId}");
                return;
            }

            _busyCreatingSession = true;
            try
            {
                _log.Info("Criando sessão de entrada...");
                var result = await _supabase.CriarSessaoEntradaAsync(Sku, Lote, DataFabricacao, DataValidade);
                if (!result.Success || string.IsNullOrWhiteSpace(result.SessionId))
                {
                    _log.Warn($"Falha ao criar sessão de entrada: {result.ErrorMessage ?? result.Message}");
                    return;
                }

                // Estamos no thread do command; ainda assim garantimos atualização na UI thread
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
                _log.Info($"Sessão de entrada ativa: {result.SessionId} (entrada_id={result.EntradaId})");
            }
            catch (Exception ex)
            {
                _log.Error($"❌ Erro ao criar sessão: {ex.Message}", ex);
            }
            finally
            {
                _busyCreatingSession = false;
            }
        });

        IniciarLeitura = new AsyncRelayCommand(async () =>
        {
            // Previne múltiplas leituras simultâneas
            if (_busyReading || IsReading)
            {
                _log.Warn("⚠️ Leitura já em andamento. Aguarde...");
                return;
            }

            if (string.IsNullOrWhiteSpace(SessionId))
            {
                _log.Warn("Nenhuma sessão de entrada ativa. Clique em 'Criar Sessão' primeiro.");
                return;
            }

            // ⚠️ ENTRADA: tags só vão para o MEPO se tivermos o entrada_id.
            // Sem isso, o insert em rfid_tags_estoque falha.
            if (string.IsNullOrWhiteSpace(EntradaId))
            {
                _log.Warn("Sessão criada, mas o EntradaId ainda não foi carregado. Aguarde alguns segundos e tente novamente.");
                return;
            }

            _busyReading = true;
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                IsReading = true;
            });

            try
            {
                await _realtime.BroadcastReaderStartAsync(SessionId);
            }
            catch (Exception ex)
            {
                _log.Warn($"Falha ao enviar reader_start: {ex.Message}");
            }

            _log.Info("⏳ Iniciando leitura de entrada...");
            _ = Task.Run(async () =>
            {
                try
                {
                    await _pipeline.BeginReadingAsync();
                    _log.Info("✅ Leitura de entrada ativa - tags aparecerão automaticamente");
                }
                catch (Exception ex)
                {
                    _log.Error($"❌ Erro ao iniciar leitura: {ex.Message}", ex);
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
                if (!string.IsNullOrWhiteSpace(SessionId))
                {
                    await _realtime.BroadcastReaderStopAsync(SessionId);
                }
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

            // Para leitura + força flush antes de finalizar no backend.
            // (Sem isso, a Edge Function pode finalizar e enxergar 0 tags.)
            try { await _realtime.BroadcastReaderStopAsync(SessionId); } catch { }
            try { await _pipeline.EndReadingAsync(); } catch { }
            try { await _pipeline.FlushPendingAsync(); } catch { }
            try { await Task.Delay(250); } catch { }
            try { await _pipeline.FlushPendingAsync(); } catch { }

            var skuFinal = Sku;
            var loteFinal = Lote;
            var totalFinal = TotalTags;

            if (!string.IsNullOrWhiteSpace(SessionId))
            {
                await _supabase.FinalizarSessaoEdgeAsync(SessionId, "entrada");
                _session.EndSession();
            }

            _log.Info($"✅ Entrada finalizada: {totalFinal} tags - SKU: {skuFinal}, Lote: {loteFinal}");

            // Confirmação para o operador
            try
            {
                System.Windows.MessageBox.Show(
                    $"Entrada finalizada com sucesso.\n\nSKU: {skuFinal}\nLote: {loteFinal}\nQuantidade (tags únicas): {totalFinal}",
                    "Finalizar entrada",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch { }
            
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
            
            // Para leitura + força flush antes de cancelar (evita "sobrar" tag na fila)
            try { await _realtime.BroadcastReaderStopAsync(SessionId); } catch { }
            try { await _pipeline.EndReadingAsync(); } catch { }
            try { await _pipeline.FlushPendingAsync(); } catch { }

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
        
        AtualizarTagConflito = new AsyncRelayCommand<TagConflict>(async c =>
        {
            if (c is null) return;
            if (string.IsNullOrWhiteSpace(c.Epc)) return;

            var ok = await _supabase.UpdateTagEstoqueAsync(c.Epc, Sku, Lote, DataFabricacao, DataValidade);
            if (ok)
            {
                _log.Info($"✅ Tag atualizada no estoque: {c.Epc}");
                Conflicts.Remove(c);
            }
            else
            {
                _log.Warn($"Falha ao atualizar tag no MEPO: {c.Epc}");
            }
        });

        ExcluirConflito = new RelayCommand<TagConflict>(c =>
        {
            if (c is null) return;
            Conflicts.Remove(c);
        });

        Limpar = new RelayCommand(() =>
        {
            _pipeline.ResetSessionCounters();
            Conflicts.Clear();
            _conflictChecked.Clear();
            Sku = "";
            Descricao = "";
            Lote = "";
            DataFabricacao = null;
            DataValidade = null;
            SessionId = "";
            EntradaId = "";
        });

        _realtime.OnReaderStopReceived += async (_, __) =>
        {
            _log.Info("Comando reader_stop recebido do Web");
            if (_busyReading || IsReading)
            {
                await _pipeline.EndReadingAsync();
                _busyReading = false;
                IsReading = false;
            }
        };

        _realtime.OnSessionCancelReceived += async (_, payload) =>
        {
            var cancelSessionId = payload.TryGetProperty("session_id", out var sid)
                ? sid.GetString()
                : null;
            if (cancelSessionId == SessionId)
            {
                _log.Info("Sessão de entrada cancelada remotamente pelo Web");
                await Cancelar.ExecuteAsync(null);
            }
        };

        RefreshSnapshot();
    }

    partial void OnSkuChanged(string value)
    {
        // UI: ao digitar SKU, buscar descrição no MEPO e preencher automaticamente.
        // Debounce simples para não disparar request a cada tecla.
        try { _skuLookupCts?.Cancel(); } catch { }
        _skuLookupCts = new CancellationTokenSource();
        var ct = _skuLookupCts.Token;

        if (string.IsNullOrWhiteSpace(value))
        {
            Descricao = "";
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(250, ct);
                var desc = await _supabase.BuscarDescricaoProdutoAsync(value, ct);
                if (ct.IsCancellationRequested) return;

                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Descricao = string.IsNullOrWhiteSpace(desc)
                        ? "SKU não encontrado no MEPO (verifique o código)"
                        : desc;
                });
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _log.Warn($"Falha ao buscar descrição do SKU: {ex.Message}");
            }
        }, ct);
    }

    private void RefreshSnapshot()
    {
        // HOT PATH: sem log por tick (trava UI + cresce log)
        // ✅ Usa BeginInvoke para atualização assíncrona (evita deadlock)
        System.Windows.Application.Current?.Dispatcher.BeginInvoke(() =>
        {
            TotalTags = _pipeline.TotalUniqueTags;
            Recent.Clear();
            foreach (var t in _pipeline.RecentTags)
                Recent.Add(t);

            // Verifica conflito só para a última tag (evita flood)
            var last = Recent.LastOrDefault();
            if (!string.IsNullOrWhiteSpace(last))
            {
                _ = CheckConflictAsync(last);
            }
        });
    }

    private async Task CheckConflictAsync(string epc)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(SessionId)) return;
            if (string.IsNullOrWhiteSpace(EntradaId)) return;
            if (string.IsNullOrWhiteSpace(Sku) || string.IsNullOrWhiteSpace(Lote)) return;

            if (_conflictChecked.Contains(epc)) return;
            _conflictChecked.Add(epc);

            var snap = await _supabase.GetTagEstoqueSnapshotAsync(epc);
            if (snap is null) return; // não existe -> sem conflito

            var existingSku = snap.Sku ?? "";
            var existingLote = snap.Batch ?? "";

            // Se já existe e é diferente do que estamos tentando gravar, registrar conflito.
            if (!string.Equals(existingSku, Sku, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(existingLote, Lote, StringComparison.OrdinalIgnoreCase))
            {
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (Conflicts.Any(c => string.Equals(c.Epc, epc, StringComparison.OrdinalIgnoreCase))) return;
                    Conflicts.Add(new TagConflict
                    {
                        Epc = epc,
                        ExistingSku = snap.Sku,
                        ExistingLote = snap.Batch,
                        ExistingFabricacao = snap.ManufactureDate,
                        ExistingValidade = snap.ExpirationDate,
                        NewSku = Sku,
                        NewLote = Lote,
                        NewFabricacao = DataFabricacao,
                        NewValidade = DataValidade
                    });
                });

                _log.Warn($"⚠️ Conflito: tag {epc} já existe no estoque com SKU/Lote diferente.");
            }
        }
        catch (Exception ex)
        {
            _log.Debug($"CheckConflictAsync falhou: {ex.Message}");
        }
    }
}
