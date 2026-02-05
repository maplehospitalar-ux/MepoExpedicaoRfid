using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// Serviço de heartbeat RPC para manter dispositivo ativo
/// PARTE E: Usa HttpClient singleton
/// PARTE H: Usa PeriodicTimer para melhor controle e sem acúmulo de Tasks
/// </summary>
public sealed class HeartbeatService : IDisposable
{
    // PARTE E: HttpClient singleton
    private static readonly HttpClient _httpClient = new HttpClient();
    
    private readonly SupabaseAuthService _auth;
    private readonly string _baseUrl;
    private readonly string _deviceId;
    private readonly int _intervalMs;
    private readonly AppLogger _log;

    // PARTE H: Usa PeriodicTimer ao invés de Task.Run + Task.Delay (evita acúmulo)
    private PeriodicTimer? _heartbeatTimer;
    private CancellationTokenSource? _cts;
    private Task? _heartbeatTask;
    private bool _disposed;

    public bool IsRunning => _heartbeatTask != null && !_heartbeatTask.IsCompleted;

    public HeartbeatService(SupabaseAuthService auth, string baseUrl, string deviceId, int intervalMs, AppLogger log)
    {
        _auth = auth;
        _baseUrl = baseUrl;
        _deviceId = deviceId;
        _intervalMs = intervalMs;
        _log = log;
    }

    public void Start()
    {
        if (IsRunning)
        {
            _log.Warn("[Heartbeat] Already running");
            return;
        }

        _cts = new CancellationTokenSource();
        
        // PARTE H: Usa PeriodicTimer para garantir execução em intervalo fixo
        _heartbeatTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(_intervalMs));
        
        _heartbeatTask = Task.Run(() => HeartbeatLoop(_cts.Token), _cts.Token);
        _log.Info($"[Heartbeat] Started (interval: {_intervalMs}ms)");
    }

    public void Stop()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            _log.Info("[Heartbeat] Stopped");
        }
    }

    // PARTE H: Loop com PeriodicTimer ao invés de Task.Delay
    private async Task HeartbeatLoop(CancellationToken ct)
    {
        if (_heartbeatTimer == null)
            return;

        try
        {
            while (!ct.IsCancellationRequested)
            {
                // Aguarda próximo tick do timer
                if (!await _heartbeatTimer.WaitForNextTickAsync(ct).ConfigureAwait(false))
                    break;

                try
                {
                    var token = await _auth.GetValidTokenAsync();
                    if (string.IsNullOrEmpty(token))
                    {
                        _log.Debug("[Heartbeat] No valid token, skipping");
                        continue;
                    }

                    var payload = new { p_device_id = _deviceId };

                    var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/rest/v1/rpc/rfid_device_heartbeat");
                    request.Headers.Add("apikey", Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY") ?? string.Empty);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                    // PARTE E: Usa HttpClient singleton
                    var response = await _httpClient.SendAsync(request, ct).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        _log.Debug($"[Heartbeat] Sent successfully for {_deviceId}");
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                        _log.Debug($"[Heartbeat] Failed: {response.StatusCode}");
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _log.Debug($"[Heartbeat] Exception: {ex.Message}");
                }
            }
        }
        finally
        {
            _log.Debug("[Heartbeat] Loop finalizado");
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        Stop();
        _cts?.Dispose();
        _heartbeatTimer?.Dispose();
        _disposed = true;
    }
}
