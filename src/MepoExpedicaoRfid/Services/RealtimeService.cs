using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// Serviço de comunicação Realtime via WebSocket com Supabase
/// </summary>
public sealed class RealtimeService : IDisposable
{
    private readonly string _wsUrl;
    private readonly string _anonKey;
    private readonly string _topic;
    private readonly string _deviceId;
    private readonly AppLogger _log;

    private ClientWebSocket? _ws;
    private CancellationTokenSource? _cts;
    private bool _disposed;
    private int _refCounter = 1;

    public bool IsConnected => _ws?.State == WebSocketState.Open;

    // Eventos
    public event EventHandler<JsonDocument>? OnBroadcastReceived;
    public event EventHandler<JsonElement>? OnReaderStartReceived;
    public event EventHandler<JsonElement>? OnReaderStopReceived;
    public event EventHandler<JsonElement>? OnReaderPauseReceived;
    public event EventHandler<JsonElement>? OnSessionCancelReceived;
    public event EventHandler<JsonElement>? OnNovoPedidoFila;
    public event EventHandler? OnConnected;
    public event EventHandler? OnDisconnected;

    public RealtimeService(string wsUrl, string anonKey, string topic, string deviceId, AppLogger log)
    {
        _wsUrl = wsUrl;
        _anonKey = anonKey;
        _topic = topic;
        _deviceId = deviceId;
        _log = log;
    }

    /// <summary>
    /// Conecta ao WebSocket e entra no channel
    /// </summary>
    public async Task ConnectAsync()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(RealtimeService));

        if (_ws != null && _ws.State == WebSocketState.Open)
        {
            _log.Warn("WebSocket já conectado");
            return;
        }

        try
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            _ws = new ClientWebSocket();
            var uri = new Uri($"{_wsUrl}?apikey={_anonKey}&vsn=1.0.0");

            await _ws.ConnectAsync(uri, _cts.Token);
            _log.Info($"✅ WebSocket conectado: {_wsUrl}");

            // Join channel
            await JoinChannelAsync();

            // Broadcast status online
            await BroadcastReaderStatusAsync("online");

            // Inicia loops
            _ = Task.Run(() => ReceiveLoop(_cts.Token), _cts.Token);
            _ = Task.Run(() => HeartbeatLoop(_cts.Token), _cts.Token);

            OnConnected?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _log.Error("Falha ao conectar WebSocket", ex);
            throw;
        }
    }

    /// <summary>
    /// Desconecta do WebSocket
    /// </summary>
    public async Task DisconnectAsync()
    {
        if (_ws == null || _ws.State != WebSocketState.Open)
            return;

        try
        {
            // Broadcast status offline
            await BroadcastReaderStatusAsync("offline");

            // Leave channel
            await LeaveChannelAsync();

            _cts?.Cancel();
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnect", CancellationToken.None);

            _log.Info("⛔ WebSocket desconectado");
            OnDisconnected?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _log.Error("Erro ao desconectar WebSocket", ex);
        }
    }

    private async Task JoinChannelAsync()
    {
        var joinMsg = new
        {
            topic = _topic,
            @event = "phx_join",
            payload = new { config = new { broadcast = new { self = false, ack = true } } },
            @ref = _refCounter++.ToString()
        };

        await SendAsync(joinMsg);
        _log.Info($"📡 Join enviado: {_topic}");
    }

    private async Task LeaveChannelAsync()
    {
        var leaveMsg = new
        {
            topic = _topic,
            @event = "phx_leave",
            payload = new { },
            @ref = _refCounter++.ToString()
        };

        await SendAsync(leaveMsg);
    }

    /// <summary>
    /// Broadcast do status do leitor
    /// </summary>
    public async Task BroadcastReaderStatusAsync(string status)
    {
        var msg = new
        {
            topic = _topic,
            @event = "broadcast",
            payload = new
            {
                type = "broadcast",
                @event = "reader_status",
                reader_id = _deviceId,
                status,
                timestamp = DateTime.UtcNow.ToString("o")
            },
            @ref = _refCounter++.ToString()
        };

        await SendAsync(msg);
    }

    /// <summary>
    /// Broadcast de tag lida
    /// </summary>
    public async Task BroadcastTagReadAsync(string epc, string? sku, string? lote, string sessionId, double? rssi = null)
    {
        var msg = new
        {
            topic = _topic,
            @event = "broadcast",
            payload = new
            {
                type = "broadcast",
                @event = "tag_read",
                reader_id = _deviceId,
                epc,
                sku,
                lote,
                session_id = sessionId,
                rssi,
                timestamp = DateTime.UtcNow.ToString("o")
            },
            @ref = _refCounter++.ToString()
        };

        await SendAsync(msg);
    }

    /// <summary>
    /// Broadcast de início de leitura
    /// </summary>
    public async Task BroadcastReaderStartAsync(string sessionId)
    {
        var msg = new
        {
            topic = _topic,
            @event = "broadcast",
            payload = new
            {
                type = "broadcast",
                @event = "reader_start",
                reader_id = _deviceId,
                session_id = sessionId,
                timestamp = DateTime.UtcNow.ToString("o")
            },
            @ref = _refCounter++.ToString()
        };

        await SendAsync(msg);
    }

    /// <summary>
    /// Broadcast de parada de leitura
    /// </summary>
    public async Task BroadcastReaderStopAsync(string sessionId)
    {
        var msg = new
        {
            topic = _topic,
            @event = "broadcast",
            payload = new
            {
                type = "broadcast",
                @event = "reader_stop",
                reader_id = _deviceId,
                session_id = sessionId,
                timestamp = DateTime.UtcNow.ToString("o")
            },
            @ref = _refCounter++.ToString()
        };

        await SendAsync(msg);
    }

    private async Task SendAsync(object message)
    {
        if (_ws == null || _ws.State != WebSocketState.Open)
        {
            _log.Warn("WebSocket não conectado. Mensagem não enviada.");
            return;
        }

        try
        {
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _log.Error("Erro ao enviar mensagem WebSocket", ex);
        }
    }

    private async Task ReceiveLoop(CancellationToken ct)
    {
        var buffer = new byte[8192];

        try
        {
            while (!ct.IsCancellationRequested && _ws != null && _ws.State == WebSocketState.Open)
            {
                var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _log.Warn("WebSocket fechado pelo servidor");
                    OnDisconnected?.Invoke(this, EventArgs.Empty);
                    break;
                }

                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                ProcessReceivedMessage(json);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal
        }
        catch (Exception ex)
        {
            _log.Error("Erro no loop de recebimento WebSocket", ex);
        }
    }

    private void ProcessReceivedMessage(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("event", out var eventProp))
                return;

            var eventName = eventProp.GetString();

            // Ignora replies e heartbeats
            if (eventName is "phx_reply" or "phx_error")
                return;

            // Processa broadcasts
            if (eventName == "broadcast")
            {
                if (root.TryGetProperty("payload", out var payloadProp))
                {
                    // Extrai evento interno
                    var innerEvent = payloadProp.TryGetProperty("event", out var evt)
                        ? evt.GetString()
                        : null;
                    var innerPayload = payloadProp;
                    if (payloadProp.TryGetProperty("payload", out var payloadInner))
                    {
                        innerPayload = payloadInner;
                    }

                    _log.Info($"📨 Broadcast recebido: {innerEvent}");

                    // Emite eventos específicos
                    switch (innerEvent)
                    {
                        case "reader_start":
                            OnReaderStartReceived?.Invoke(this, innerPayload.Clone());
                            break;
                        case "reader_stop":
                            OnReaderStopReceived?.Invoke(this, innerPayload.Clone());
                            break;
                        case "reader_pause":
                            OnReaderPauseReceived?.Invoke(this, innerPayload.Clone());
                            break;
                        case "session_cancel":
                            OnSessionCancelReceived?.Invoke(this, innerPayload.Clone());
                            break;
                        case "novo_pedido_fila":
                            OnNovoPedidoFila?.Invoke(this, innerPayload.Clone());
                            break;
                    }
                }
                
                // Evento genérico (para debug/logging)
                OnBroadcastReceived?.Invoke(this, doc);
            }
        }
        catch (Exception ex)
        {
            _log.Error($"Erro ao processar mensagem: {json}", ex);
        }
    }

    private async Task HeartbeatLoop(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(30000, ct);

                if (_ws?.State == WebSocketState.Open)
                {
                    var hbMsg = new
                    {
                        topic = "phoenix",
                        @event = "heartbeat",
                        payload = new { },
                        @ref = $"heartbeat_{_refCounter++}"
                    };

                    await SendAsync(hbMsg);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Normal
        }
        catch (Exception ex)
        {
            _log.Error("Erro no loop de heartbeat", ex);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        DisconnectAsync().Wait();

        _cts?.Cancel();
        _cts?.Dispose();
        _ws?.Dispose();
    }
}
