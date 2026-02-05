using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MepoExpedicaoRfid.Models;
using MepoExpedicaoRfid.Services;

namespace MepoExpedicaoRfid.ViewModels;

public partial class ConsultaTagViewModel : ObservableObject
{
    private readonly TagHistoryService _svc;
    private readonly TagPipeline _pipeline;
    private readonly AppLogger _log;
    private bool _busyReadingTag = false;  // Previne múltiplas leituras simultâneas

    [ObservableProperty] private string epc = "";
    [ObservableProperty] private TagCurrent? current;
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private string statusText = "—";

    public ObservableCollection<TagMovement> Movimentos { get; } = new();

    public IAsyncRelayCommand Consultar { get; }
    public IAsyncRelayCommand LerDoLeitor { get; }

    public ConsultaTagViewModel(TagHistoryService svc, TagPipeline pipeline, AppLogger log)
    {
        _svc = svc;
        _pipeline = pipeline;
        _log = log;

        Consultar = new AsyncRelayCommand(async () => await ConsultarAsync());

        // Consulta por leitura única (InventorySingle)
        LerDoLeitor = new AsyncRelayCommand(async () =>
        {
            // Previne múltiplas consultas simultâneas
            if (_busyReadingTag)
            {
                _log.Warn("⚠️ Leitura já em andamento. Aguarde...");
                return;
            }

            _busyReadingTag = true;
            try
            {
                StatusText = "Lendo tag...";
                _log.Info("🔎 Iniciando leitura de tag do hardware...");
                
                _log.Info("DEBUG: Chamando ConsultarTagAsync...");
                var epc = await _pipeline.ConsultarTagAsync(TimeSpan.FromSeconds(5)); // Sem ConfigureAwait
                _log.Info($"DEBUG: ConsultarTagAsync retornou: {epc ?? "null"}");
                
                if (!string.IsNullOrWhiteSpace(epc))
                {
                    _log.Info($"✅ Tag lida com sucesso: {epc}");
                    Epc = epc;
                    
                    _log.Info("DEBUG: Chamando ConsultarAsync...");
                    await ConsultarAsync(); // Sem ConfigureAwait
                    _log.Info("DEBUG: ConsultarAsync concluído");
                }
                else
                {
                    StatusText = "Nenhuma tag lida";
                    _log.Warn("⚠️ Nenhuma tag foi lida pelo hardware no timeout de 5s");
                    _log.Info("💡 Dica: Aproxime uma tag RFID do leitor e tente novamente");
                }
                
                _log.Info("DEBUG: LerDoLeitor concluído com sucesso");
            }
            catch (OperationCanceledException cancelEx)
            {
                StatusText = "Leitura cancelada";
                _log.Warn($"⚠️ Leitura cancelada: {cancelEx.Message}");
            }
            catch (Exception ex)
            {
                StatusText = "Erro na leitura";
                _log.Error($"❌ Erro ao ler tag do hardware: {ex.Message}", ex);
                _log.Error($"Stack: {ex.StackTrace}");
            }
            finally
            {
                _log.Info("DEBUG: Finally block - liberando busy flag");
                _busyReadingTag = false;
            }
        }, () => !_busyReadingTag); // Adiciona canExecute
    }

    private async Task ConsultarAsync()
    {
        if (string.IsNullOrWhiteSpace(Epc)) return;

        IsLoading = true;
        StatusText = "Consultando...";
        
        // Limpar Movimentos na thread da UI
#pragma warning disable CS4014
        Dispatcher.CurrentDispatcher.BeginInvoke(() => Movimentos.Clear());

        try
        {
            _log.Info($"Iniciando consulta para EPC: {Epc}");
            var dto = await _svc.GetAsync(Epc, limit: 200);
            
            _log.Info($"DTO retornado - Current: {(dto.Current != null ? "EXISTS" : "NULL")}");
            if (dto.Current != null)
            {
                _log.Info($"Current.Sku={dto.Current.Sku}, Lote={dto.Current.Lote}, Status={dto.Current.Status}, Desc={dto.Current.Descricao}");
            }
            _log.Info($"Movimentos: {dto.Movimentos.Count} registros");
            
            // Atualizar UI na thread do Dispatcher
            Dispatcher.CurrentDispatcher.BeginInvoke(() =>
            {
                if (dto.Current != null)
                {
                    Current = dto.Current;
                    StatusText = $"{dto.Current.Status ?? "OK"} - {dto.Movimentos.Count} evento(s)";
                    _log.Info($"Current atualizado na UI");
                }
                else
                {
                    Current = null;
                    StatusText = "Tag não encontrada no sistema";
                    _log.Warn($"Current é NULL - tag não encontrada");
                }
                
                foreach (var m in dto.Movimentos) 
                    Movimentos.Add(m);
            });
        }
        catch (Exception ex)
        {
            StatusText = $"Erro: {ex.Message}";
            _log.Error($"Falha consulta tag: {ex.Message}", ex);
        }
        finally
        {
            IsLoading = false;
        }
#pragma warning restore CS4014
    }
}
