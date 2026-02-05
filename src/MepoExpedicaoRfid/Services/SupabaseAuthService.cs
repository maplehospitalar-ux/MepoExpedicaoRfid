using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// Serviço de autenticação Supabase com refresh automático
/// </summary>
public sealed class SupabaseAuthService
{
    private readonly string _baseUrl;
    private readonly string _anonKey;
    private readonly string _email;
    private readonly string _password;
    private readonly AppLogger _log;
    private readonly HttpClient _http = new();

    private string? _accessToken;
    private string? _refreshToken;
    private DateTime _expiresAt = DateTime.MinValue;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _expiresAt;
    public string? AccessToken => _accessToken;

    public SupabaseAuthService(string baseUrl, string anonKey, string email, string password, AppLogger log)
    {
        _baseUrl = baseUrl;
        _anonKey = anonKey;
        _email = email;
        _password = password;
        _log = log;

        _http.DefaultRequestHeaders.Add("apikey", _anonKey);
    }

    public async Task<bool> SignInAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (IsAuthenticated)
                return true;

            var payload = new
            {
                email = _email,
                password = _password
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync($"{_baseUrl}/auth/v1/token?grant_type=password", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _log.Error($"[Auth] Login failed: {error}");
                return false;
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponse>(json);

            if (result == null || string.IsNullOrEmpty(result.access_token))
            {
                _log.Error("[Auth] Invalid auth response");
                return false;
            }

            _accessToken = result.access_token;
            _refreshToken = result.refresh_token;
            _expiresAt = DateTime.UtcNow.AddSeconds(result.expires_in - 60); // 60s buffer

            _log.Info($"[Auth] Login successful, expires at {_expiresAt:HH:mm:ss}");
            return true;
        }
        catch (Exception ex)
        {
            _log.Error($"[Auth] Exception: {ex.Message}");
            return false;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (string.IsNullOrEmpty(_refreshToken))
            {
                _log.Warn("[Auth] No refresh token available");
                return await SignInAsync();
            }

            var payload = new { refresh_token = _refreshToken };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync($"{_baseUrl}/auth/v1/token?grant_type=refresh_token", content);

            if (!response.IsSuccessStatusCode)
            {
                _log.Warn("[Auth] Refresh failed, re-authenticating");
                return await SignInAsync();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponse>(json);

            if (result == null || string.IsNullOrEmpty(result.access_token))
                return await SignInAsync();

            _accessToken = result.access_token;
            _refreshToken = result.refresh_token;
            _expiresAt = DateTime.UtcNow.AddSeconds(result.expires_in - 60);

            _log.Info("[Auth] Token refreshed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _log.Error($"[Auth] Refresh exception: {ex.Message}");
            return await SignInAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<string?> GetValidTokenAsync()
    {
        if (!IsAuthenticated)
        {
            if (!await SignInAsync())
                return null;
        }
        else if (DateTime.UtcNow > _expiresAt.AddMinutes(-5))
        {
            await RefreshTokenAsync();
        }

        return _accessToken;
    }

    private sealed class AuthResponse
    {
        public string access_token { get; set; } = "";
        public string refresh_token { get; set; } = "";
        public int expires_in { get; set; }
    }
}
