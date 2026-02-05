using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using MepoExpedicaoRfid.Services;

namespace MepoExpedicaoRfid.ViewModels;

public partial class StatusViewModel : ObservableObject
{
    private readonly SupabaseService _supabase;
    private readonly IRfidReader _reader;
    private readonly AppConfig _cfg;
    private readonly AppLogger _log;

    private CancellationTokenSource? _cts;

    [ObservableProperty] private Brush supabaseBrush = Brushes.Orange;
    [ObservableProperty] private Brush readerBrush = Brushes.Orange;
    [ObservableProperty] private string supabaseText = "Supabase: conectando...";
    [ObservableProperty] private string readerText = "RFID: desconectado";
    [ObservableProperty] private string footer = "";

    public string? UserId { get; private set; } // placeholder (se você quiser decodificar JWT, adicione)

    public StatusViewModel(SupabaseService supabase, IRfidReader reader, AppConfig cfg, AppLogger log)
    {
        _supabase = supabase;
        _reader = reader;
        _cfg = cfg;
        _log = log;
    }

    public async Task InitializeAsync()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        _log.Info($"RFID ReaderMode: {_cfg.RFID.ReaderMode}");

        try
        {
            await _supabase.ConnectAsync();
            SupabaseBrush = Brushes.LimeGreen;
            SupabaseText = "Supabase: conectado";
        }
        catch (Exception ex)
        {
            SupabaseBrush = Brushes.IndianRed;
            SupabaseText = "Supabase: offline";
            _log.Error("Falha ao conectar Supabase", ex);
        }

        // ✅ Não tenta conectar se já estiver conectado (Bootstrapper já conectou via TagPipeline)
        if (_reader.IsConnected)
        {
            ReaderBrush = Brushes.LimeGreen;
            ReaderText = $"RFID: conectado ({_reader.Name})";
            return;
        }

        _log.Info("Iniciando conexão RFID com timeout de 8s...");
        var connectTask = _reader.ConnectAsync(_cts.Token);

        // Atualiza UI quando a conexão finalizar (sem bloquear a UI)
        _ = connectTask.ContinueWith(t =>
        {
            if (t.IsCanceled)
            {
                ReaderBrush = Brushes.IndianRed;
                ReaderText = "RFID: cancelado";
            }
            else if (t.IsFaulted)
            {
                ReaderBrush = Brushes.IndianRed;
                ReaderText = "RFID: offline";
                _log.Error("Falha ao conectar RFID", t.Exception?.GetBaseException());
            }
            else
            {
                ReaderBrush = Brushes.LimeGreen;
                ReaderText = $"RFID: conectado ({_reader.Name})";
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());

        // Watchdog de timeout (registra erro mesmo se a chamada nativa travar)
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(8), _cts.Token).ConfigureAwait(false);
                if (!connectTask.IsCompleted)
                {
                    _log.Warn("Conexão RFID não respondeu dentro de 8s.");
                    _log.Error("Timeout ao conectar RFID. Verifique DLL/hardware e conexão USB.");

                    _ = Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        ReaderBrush = Brushes.IndianRed;
                        ReaderText = "RFID: timeout na conexão";
                    });
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
        }, _cts.Token);

        Footer = $"Device: {_cfg.Device.Id}  •  Topic: {_cfg.Realtime.Topic}";

        // Heartbeat loop
        _ = Task.Run(async () =>
        {
            var delay = TimeSpan.FromSeconds(Math.Max(10, _cfg.Device.HeartbeatSeconds));
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    if (_supabase.IsConnected)
                        await _supabase.HeartbeatAsync().ConfigureAwait(false);
                }
                catch { }
                try
                {
                    await Task.Delay(delay, _cts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }, _cts.Token).ConfigureAwait(false);
    }
}
