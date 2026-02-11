using System.Collections.ObjectModel;
using System.Media;
using System.Text.Json;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MepoExpedicaoRfid.Models;
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

    public ObservableCollection<AvisoPedido> AvisosPendentes { get; } = new();
    public IRelayCommand<AvisoPedido> DismissAviso { get; }

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
        _realtime.OnNovoPedidoFila += (_, payload) => OnNovoPedidoFila(payload);

        DismissAviso = new RelayCommand<AvisoPedido>(aviso =>
        {
            if (aviso is null) return;
            _ = Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                AvisosPendentes.Remove(aviso);
            });
        });

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

    private void OnNovoPedidoFila(JsonElement payload)
    {
        try
        {
            // payload esperado (já chega do backend): has_aviso, aviso_tipo, observacao, numero, origem, cliente_nome...
            var hasAviso = payload.TryGetProperty("has_aviso", out var ha) && ha.ValueKind == JsonValueKind.True;
            if (!hasAviso) return;

            var avisoTipo = payload.TryGetProperty("aviso_tipo", out var at) ? (at.GetString() ?? "") : "";
            var observacao = payload.TryGetProperty("observacao", out var obs) ? (obs.GetString() ?? "") : "";

            var aviso = new AvisoPedido
            {
                SessionId = payload.TryGetProperty("session_id", out var sid) ? (sid.GetString() ?? "") : "",
                Numero = payload.TryGetProperty("numero", out var num) ? (num.GetString() ?? "") : "",
                Origem = payload.TryGetProperty("origem", out var org) ? (org.GetString() ?? "") : "",
                ClienteNome = payload.TryGetProperty("cliente_nome", out var cn) ? (cn.GetString() ?? "") : "",
                Observacao = observacao,
                AvisoTipo = avisoTipo,
                EnviadoPor = payload.TryGetProperty("enviado_por", out var ep) ? (ep.GetString() ?? "") : "",
                EnviadoAt = payload.TryGetProperty("enviado_at", out var ea) && ea.ValueKind == JsonValueKind.String && DateTime.TryParse(ea.GetString(), out var dt)
                    ? dt
                    : null,
            };

            _ = Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                // evita duplicar pelo mesmo session_id (quando receber replay)
                if (!string.IsNullOrWhiteSpace(aviso.SessionId) && AvisosPendentes.Any(a => a.SessionId == aviso.SessionId))
                    return;

                AvisosPendentes.Add(aviso);

                // FIFO: max 20
                while (AvisosPendentes.Count > 20)
                    AvisosPendentes.RemoveAt(0);

                // Som para SEM NF
                if (aviso.IsSemNf)
                {
                    try { SystemSounds.Exclamation.Play(); } catch { }
                }
            });
        }
        catch (Exception ex)
        {
            _log.Debug($"Falha ao processar novo_pedido_fila (avisos): {ex.Message}");
        }
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
