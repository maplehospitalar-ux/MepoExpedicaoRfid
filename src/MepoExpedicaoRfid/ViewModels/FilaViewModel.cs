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
    private readonly PrintService _printer;
    private readonly AppLogger _log;

    // Auto-impressão: garante que um mesmo documento não imprima em loop a cada refresh
    private readonly HashSet<Guid> _autoPrinted = new();

    public event EventHandler<JsonElement>? OnPedidoParaImpressao;

    public ObservableCollection<FilaItem> Pendentes => _fila.Pendentes;
    public ObservableCollection<FilaItem> EmSeparacao => _fila.EmSeparacao;
    public ObservableCollection<FilaItem> Finalizados => _fila.Finalizados;

    [ObservableProperty] private FilaItem? selected;

    public IAsyncRelayCommand Refresh { get; }
    public IAsyncRelayCommand AbrirPedido { get; }

    public FilaViewModel(FilaService fila, RealtimeService realtime, NavigationViewModel nav, SaidaViewModel saida, PrintService printer, AppLogger log)
    {
        _fila = fila;
        _realtime = realtime;
        _nav = nav;
        _saida = saida;
        _printer = printer;
        _log = log;

        Refresh = new AsyncRelayCommand(async () => 
        {
            _log.Info("🔄 Atualizando fila...");
            await _fila.RefreshAsync();
            _log.Info("✅ Fila atualizada");

            // Estratégia mais confiável que depender do postgres_changes:
            // ao atualizar a fila, tenta auto-imprimir itens em status 'preparando'.
            _ = Task.Run(async () =>
            {
                try
                {
                    var candidatos = _fila.Pendentes.Concat(_fila.EmSeparacao)
                        .Where(x => string.Equals((x.Status ?? "").Trim(), "preparando", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    foreach (var it in candidatos)
                    {
                        if (it.Id == Guid.Empty) continue;
                        if (_autoPrinted.Contains(it.Id)) continue;

                        _autoPrinted.Add(it.Id);
                        _log.Info($"[AUTO-PRINT] Pedido na fila (status=preparando) doc_id={it.Id} origem={it.Origem} numero={it.NumeroPedido} -> imprimindo...");

                        var printText = await _fila.BuildPrintTextAsync(it.Id);
                        if (!string.IsNullOrWhiteSpace(printText))
                            _printer.PrintText(printText);
                        else
                            _log.Warn($"[AUTO-PRINT] Texto de impressão vazio para doc_id={it.Id}");
                    }
                }
                catch (Exception ex)
                {
                    _log.Warn($"[AUTO-PRINT] Falha ao auto-imprimir via refresh: {ex.Message}");
                }
            });
        });

        AbrirPedido = new AsyncRelayCommand(async () =>
        {
            if (Selected is null)
            {
                _log.Warn("Nenhum pedido selecionado");
                return;
            }

            _log.Info($"▶️ Abrindo pedido da fila: {Selected.NumeroPedido} ({Selected.Origem})");

            // Auto-impressão imediata ao "importar/abrir" o pedido da fila (fluxo do operador)
            try
            {
                if (Selected.Id != Guid.Empty && !_autoPrinted.Contains(Selected.Id))
                {
                    _autoPrinted.Add(Selected.Id);
                    _log.Info($"[AUTO-PRINT] AbrirPedido -> imprimindo doc_id={Selected.Id}...");
                    var printText = await _fila.BuildPrintTextAsync(Selected.Id).ConfigureAwait(true);
                    if (!string.IsNullOrWhiteSpace(printText))
                        _printer.PrintText(printText);
                    else
                        _log.Warn($"[AUTO-PRINT] Texto de impressão vazio para doc_id={Selected.Id}");
                }
            }
            catch (Exception ex)
            {
                _log.Warn($"[AUTO-PRINT] Falha ao imprimir ao abrir pedido: {ex.Message}");
            }

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

            // Auto impressão quando MEPO envia pedido para fila (status_expedicao=preparando)
            // A payload costuma conter record com id do documento.
            try
            {
                if (payload.TryGetProperty("record", out var rec) && rec.TryGetProperty("id", out var idProp))
                {
                    var idStr = idProp.GetString() ?? idProp.ToString();
                    if (Guid.TryParse(idStr, out var docId))
                    {
                        _log.Info($"🖨️ Pedido entrou na fila (doc_id={docId}) - imprimindo... ");
                        var printText = await _fila.BuildPrintTextAsync(docId);
                        if (!string.IsNullOrWhiteSpace(printText))
                            _printer.PrintText(printText);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Warn($"Falha ao auto-imprimir pedido da fila: {ex.Message}");
            }
        };

        // Initial load
        _ = Refresh.ExecuteAsync(null);
    }
}
