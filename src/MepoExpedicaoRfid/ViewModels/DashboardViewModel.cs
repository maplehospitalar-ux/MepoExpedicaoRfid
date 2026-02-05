using CommunityToolkit.Mvvm.ComponentModel;
using MepoExpedicaoRfid.Services;

namespace MepoExpedicaoRfid.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly StatusViewModel _status;
    private readonly TagPipeline _pipeline;
    private readonly AppLogger _log;

    [ObservableProperty] private int totalTags;
    [ObservableProperty] private string readsPerSecond = "—";
    [ObservableProperty] private string[] lastTags = Array.Empty<string>();

    private int _lastCount;
    private DateTime _lastTick = DateTime.UtcNow;

    public DashboardViewModel(StatusViewModel status, TagPipeline pipeline, AppLogger log)
    {
        _status = status;
        _pipeline = pipeline;
        _log = log;

        _pipeline.SnapshotUpdated += (_, __) => OnSnapshot();
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
}
