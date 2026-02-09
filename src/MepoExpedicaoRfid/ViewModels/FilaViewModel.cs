using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MepoExpedicaoRfid.Models;
using MepoExpedicaoRfid.Services;

namespace MepoExpedicaoRfid.ViewModels;

public partial class FilaViewModel : ObservableObject
{
    private readonly FilaService _fila;
    private readonly RealtimeService _realtime;
    private readonly NavigationViewModel _nav;
    private readonly SaidaViewModel _saida;
    private readonly AppLogger _log;

    public event EventHandler<JsonElement>? OnPedidoParaImpressao;

    public ObservableCollection<FilaItem> Pendentes => _fila.Pendentes;
    public ObservableCollection<FilaItem> EmSeparacao => _fila.EmSeparacao;
    public ObservableCollection<FilaItem> Finalizados => _fila.Finalizados;

    [ObservableProperty] private FilaItem? selected;

    public IAsyncRelayCommand Refresh { get; }
    public IAsyncRelayCommand AbrirPedido { get; }

    public FilaViewModel(FilaService fila, RealtimeService realtime, NavigationViewModel nav, SaidaViewModel saida, AppLogger log)
    {
        _fila = fila;
        _realtime = realtime;
        _nav = nav;
        _saida = saida;
        _log = log;

        Refresh = new AsyncRelayCommand(async () => 
        {
            _log.Info("🔄 Atualizando fila...");
            await _fila.RefreshAsync();
            _log.Info("✅ Fila atualizada");
        });

        AbrirPedido = new AsyncRelayCommand(async () =>
        {
            if (Selected is null)
            {
                _log.Warn("Nenhum pedido selecionado");
                return;
            }

            _log.Info($"▶️ Abrindo pedido da fila: {Selected.NumeroPedido} ({Selected.Origem})");

            // Carrega pedido na tela de Saída (cria sessão ao abrir, conforme regra)
            var ok = await _saida.OpenFromFilaAsync(Selected).ConfigureAwait(true);
            if (!ok) return;

            _nav.Saida?.Execute(null);
        });

        // Realtime: Subscribe a eventos de atualização da fila
        _realtime.OnConnected += (_, _) =>
        {
            _log.Info("✅ Realtime conectado - sincronizando fila");
            _ = Refresh.ExecuteAsync(null);
        };

        _realtime.OnNovoPedidoFila += async (_, payload) =>
        {
            _log.Info("Novo pedido na fila recebido via Realtime (broadcast)");

            if (payload.TryGetProperty("print_data", out var printData))
            {
                var numero = printData.TryGetProperty("numero", out var n) ? n.GetString() : "?";
                var cliente = printData.TryGetProperty("cliente_nome", out var c) ? c.GetString() : "?";
                _log.Info($"Pedido {numero} - {cliente} -> Impressão Elgin I9");

                OnPedidoParaImpressao?.Invoke(this, printData.Clone());
            }

            await _fila.RefreshAsync();
            _log.Info("Fila atualizada");
        };

        // Postgres changes (documentos_comerciais status_expedicao='preparando')
        _realtime.OnFilaDbChanged += async (_, payload) =>
        {
            _log.Info("Fila mudou (postgres_changes) - atualizando...");
            await _fila.RefreshAsync();
        };

        // Initial load
        _ = Refresh.ExecuteAsync(null);
    }
}
