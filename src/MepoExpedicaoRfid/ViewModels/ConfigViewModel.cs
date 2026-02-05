using CommunityToolkit.Mvvm.ComponentModel;
using MepoExpedicaoRfid.Services;

namespace MepoExpedicaoRfid.ViewModels;

public partial class ConfigViewModel : ObservableObject
{
    private readonly AppConfig _cfg;
    private readonly AppLogger _log;

    public string SupabaseUrl => _cfg.Supabase.Url;
    public string DeviceId => _cfg.Device.Id;

    [ObservableProperty] private int power;
    [ObservableProperty] private int debounceMs;
    [ObservableProperty] private int batchFlushMs;
    [ObservableProperty] private int batchSize;
    [ObservableProperty] private int uiUpdateMs;
    [ObservableProperty] private string readerMode;

    public ConfigViewModel(AppConfig cfg, AppLogger log)
    {
        _cfg = cfg;
        _log = log;

        Power = cfg.RFID.Power;
        DebounceMs = cfg.RFID.DebounceMs;
        BatchFlushMs = cfg.RFID.BatchFlushMs;
        BatchSize = cfg.RFID.BatchSize;
        UiUpdateMs = cfg.RFID.UiUpdateMs;
        ReaderMode = cfg.RFID.ReaderMode;
    }
}
