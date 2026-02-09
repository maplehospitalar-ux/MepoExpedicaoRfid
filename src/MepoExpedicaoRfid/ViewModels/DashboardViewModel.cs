using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using MepoExpedicaoRfid.Services;

namespace MepoExpedicaoRfid.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly StatusViewModel _status;
    private readonly TagPipeline _pipeline;
    private readonly RealtimeService _realtime;
    private readonly FilaService _fila;
    private readonly SessionStateManager _session;
    private readonly PrintService _printer;
    private readonly AppLogger _log;

    [ObservableProperty] private int totalTags;
    [ObservableProperty] private string readsPerSecond = "—";
    [ObservableProperty] private string[] lastTags = Array.Empty<string>();
    [ObservableProperty] private string realtimeStatus = "Desconectado";

    [ObservableProperty] private int pendentes;
    [ObservableProperty] private int emSeparacao;
    [ObservableProperty] private int finalizados;

    [ObservableProperty] private string sessaoAtual = "—";
    [ObservableProperty] private string impressora = "—";

    private int _lastCount;
    private DateTime _lastTick = DateTime.UtcNow;

    public DashboardViewModel(StatusViewModel status, TagPipeline pipeline, RealtimeService realtime, FilaService fila, SessionStateManager session, PrintService printer, AppLogger log)
    {
        _status = status;
        _pipeline = pipeline;
        _realtime = realtime;
        _fila = fila;
        _session = session;
        _printer = printer;
        _log = log;

        _pipeline.SnapshotUpdated += (_, __) => OnSnapshot();
        _realtime.OnConnected += (_, __) => UpdateRealtimeStatus("Conectado");
        _realtime.OnDisconnected += (_, __) => UpdateRealtimeStatus("Desconectado");

        // Atualiza resumo da fila periodicamente (sem depender do realtime)
        _ = Task.Run(async () => await RefreshFilaLoop());

        UpdateRealtimeStatus(_realtime.IsConnected ? "Conectado" : "Desconectado");
        UpdateSessao();
        UpdatePrinter();
        OnSnapshot();
    }

    private void OnSnapshot()
    {
        TotalTags = _pipeline.TotalUniqueTags;
        LastTags = _pipeline.RecentTags.ToArray();

        var now = DateTime.UtcNow;
        var delta = TotalTags - _lastCount;
        var seconds = Math.Max(0.2, (now - _lastTick).TotalSeconds);
        ReadsPerSecond = $"{(delta / seconds):0.0} tags/s";

        _lastCount = TotalTags;
        _lastTick = now;

        UpdateSessao();
    }

    private async Task RefreshFilaLoop()
    {
        while (true)
        {
            try
            {
                await Task.Delay(5000);
                await _fila.RefreshAsync();

                Pendentes = _fila.Pendentes.Count;
                EmSeparacao = _fila.EmSeparacao.Count;
                Finalizados = _fila.Finalizados.Count;
            }
            catch { }
        }
    }

    private void UpdateSessao()
    {
        try
        {
            var s = _session.CurrentSession;
            if (s is null || string.IsNullOrWhiteSpace(s.SessionId))
            {
                SessaoAtual = "—";
                return;
            }

            var pedido = s.VendaNumero ?? "";
            var origem = s.Origem ?? "";
            var st = s.Status.ToString();
            SessaoAtual = $"{origem} {pedido} ({st})";
        }
        catch { SessaoAtual = "—"; }
    }

    private void UpdatePrinter()
    {
        try
        {
            Impressora = _printer.GetPreferredPrinterName() ?? "default";
        }
        catch { Impressora = "—"; }
    }

    private void UpdateRealtimeStatus(string status)
    {
        _ = Application.Current?.Dispatcher.InvokeAsync(() =>
        {
            RealtimeStatus = status;
        });
    }
}
