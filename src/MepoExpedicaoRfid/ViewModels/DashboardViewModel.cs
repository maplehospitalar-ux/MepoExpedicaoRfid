using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using MepoExpedicaoRfid.Services;

namespace MepoExpedicaoRfid.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly StatusViewModel _status;
    private readonly TagPipeline _pipeline;
    private readonly RealtimeService _realtime;
    private readonly AppLogger _log;

    [ObservableProperty] private int totalTags;
    [ObservableProperty] private string readsPerSecond = "—";
    [ObservableProperty] private string[] lastTags = Array.Empty<string>();
    [ObservableProperty] private string realtimeStatus = "Desconectado";

    private int _lastCount;
    private DateTime _lastTick = DateTime.UtcNow;

    public DashboardViewModel(StatusViewModel status, TagPipeline pipeline, RealtimeService realtime, AppLogger log)
    {
        _status = status;
        _pipeline = pipeline;
        _realtime = realtime;
        _log = log;

        _pipeline.SnapshotUpdated += (_, __) => OnSnapshot();
        _realtime.OnConnected += (_, __) => UpdateRealtimeStatus("Conectado");
        _realtime.OnDisconnected += (_, __) => UpdateRealtimeStatus("Desconectado");
        UpdateRealtimeStatus(_realtime.IsConnected ? "Conectado" : "Desconectado");
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
    }

    private void UpdateRealtimeStatus(string status)
    {
        _ = Application.Current?.Dispatcher.InvokeAsync(() =>
        {
            RealtimeStatus = status;
        });
    }
}
