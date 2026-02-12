using System.IO;
using System.Text.Json;
using MepoExpedicaoRfid.Services;
using MepoExpedicaoRfid.ViewModels;

namespace MepoExpedicaoRfid;

public static class Bootstrapper
{
    public static async Task<MainViewModel> BuildMainViewModel()
    {
        try
        {
            Log("Bootstrapper: Iniciando carregamento de configuração");
            var configPath = ResolveConfigPath("appsettings.json");
            AppConfig cfg;
            if (configPath is null)
            {
                Log("Bootstrapper: appsettings.json não encontrado. Usando variáveis de ambiente.");
                cfg = AppConfig.FromEnvironment();
            }
            else
            {
                Log($"Bootstrapper: Procurando configuração em: {configPath}");
                cfg = AppConfig.Load(configPath);
            }

            cfg.Validate();
            Log("Bootstrapper: Configuração carregada com sucesso");

            Log("Bootstrapper: Criando AppLogger");
            var log = new AppLogger(cfg.Logging.Level, cfg.Logging.Path);

            Log("Bootstrapper: Criando SupabaseAuthService");
            var auth = new SupabaseAuthService(
                cfg.Supabase.Url,
                cfg.Supabase.AnonKey,
                cfg.Auth.Email,
                cfg.Auth.Password,
                log);

            Log("Bootstrapper: Iniciando autenticação");
            _ = auth.SignInAsync();

            Log("Bootstrapper: Criando SupabaseService");
            var supabase = new SupabaseService(cfg.Supabase, cfg.Auth, cfg.Device, log);
            
            Log("Bootstrapper: Criando TagCounterService");
            var tagCounter = new TagCounterService(log);

            Log("Bootstrapper: Criando TagInsertService");
            var tagInsert = new TagInsertService(auth, cfg.Supabase.Url, cfg.Supabase.AnonKey, log);

            Log("Bootstrapper: Criando FilaService");
            var fila = new FilaService(supabase, log);
            
            Log("Bootstrapper: Criando TagHistoryService");
            var tags = new TagHistoryService(supabase, log);

            Log("Bootstrapper: Criando HeartbeatService");
            var heartbeat = new HeartbeatService(
                auth,
                cfg.Supabase.Url,
                cfg.Device.Id,
                cfg.Performance.HeartbeatIntervalMs,
                log);

            Log("Bootstrapper: Criando RealtimeService");
            var realtime = new RealtimeService(
                cfg.Realtime.WebSocketUrl,
                cfg.Supabase.AnonKey,
                cfg.Realtime.Topic,
                cfg.Device.Id,
                log);

            Log("Bootstrapper: Criando SessionStateManager");
            var sessionManager = new SessionStateManager(log);

            Log("Bootstrapper: Criando OfflineBufferService");
            var offlineBuffer = new OfflineBufferService(
                cfg.Offline.SqliteDbPath,
                cfg.Offline.MaxTags,
                log);

            Log("Bootstrapper: Criando BatchTagInsertService");
            var batchService = new BatchTagInsertService(
                supabase,
                offlineBuffer,
                log,
                cfg.Performance.BatchSize,
                cfg.Performance.FlushIntervalMs);

            Log("Bootstrapper: Criando RfidReader");
            IRfidReader reader;
            
            if (cfg.RFID.ReaderMode.Equals("R3Dll", StringComparison.OrdinalIgnoreCase))
            {
                Log("Bootstrapper: Modo configurado para R3Dll, carregando...");
                try
                {
                    reader = new R3DllReader(cfg.RFID, log);
                    Log("Bootstrapper: R3DllReader criado com sucesso");
                }
                catch (Exception ex)
                {
                    log.Error($"Bootstrapper: Falha ao criar R3DllReader, fazendo fallback para Simulated. Motivo: {ex.Message}");
                    reader = new SimulatedRfidReader(cfg.RFID, log);
                    Log("Bootstrapper: SimulatedRfidReader ativado como fallback");
                }
            }
            else
            {
                Log("Bootstrapper: Modo Simulated configurado");
                reader = new SimulatedRfidReader(cfg.RFID, log);
            }

            Log("Bootstrapper: Criando TagPipeline");
            var pipeline = new TagPipeline(reader, cfg.RFID, log, sessionManager, batchService, realtime, supabase);

            Log("Bootstrapper: Criando StatusViewModel e navegação");
            var status = new StatusViewModel(supabase, reader, cfg, realtime, log);
            var nav = new NavigationViewModel();
            var printer = new PrintService(log);

            Log("Bootstrapper: Criando MainViewModel");
            _ = realtime.ConnectAsync();
            
            // ✅ CORREÇÃO CRÍTICA: Inicia o TagPipeline para conectar ao reader e processar tags
            Log("Bootstrapper: Iniciando TagPipeline...");
            await pipeline.StartAsync(); // ✅ AGUARDA a conexão completar antes de continuar

            var saidaVm = new SaidaViewModel(supabase, pipeline, tags, nav, cfg, sessionManager, realtime, printer, log);

            var vm = new MainViewModel(nav, status,
                new DashboardViewModel(status, pipeline, realtime, fila, sessionManager, printer, log),
                new FilaViewModel(fila, realtime, nav, saidaVm, printer, log),
                saidaVm,
                new EntradaViewModel(supabase, pipeline, nav, cfg, sessionManager, realtime, log),
                new ConsultaTagViewModel(tags, pipeline, log),
                new ConfigViewModel(cfg, log));

            Log("Bootstrapper: Configurando navegação");
            nav.Configure(vm);

            Log("Bootstrapper: Iniciando serviços de background");
            _ = status.InitializeAsync();
            heartbeat.Start();

            Log("Bootstrapper: MainViewModel retornado com sucesso");
            return vm;
        }
        catch (Exception ex)
        {
            Log($"ERRO FATAL NO BOOTSTRAPPER: {ex.Message}");
            Log($"StackTrace: {ex.StackTrace}");
            throw;
        }
    }

    private static void Log(string message)
    {
        try
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bootstrap_log.txt");
            File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
        }
        catch { /* Ignora erros de log */ }
    }

    private static string? ResolveConfigPath(string fileName)
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var basePath = Path.Combine(baseDir, fileName);
        if (File.Exists(basePath)) return basePath;

        var currentPath = Path.Combine(Environment.CurrentDirectory, fileName);
        if (File.Exists(currentPath)) return currentPath;

        return null;
    }
}

public sealed class AppConfig
{
    public SupabaseConfig Supabase { get; set; } = new();
    public AuthConfig Auth { get; set; } = new();
    public DeviceConfig Device { get; set; } = new();
    public RealtimeConfig Realtime { get; set; } = new();
    public HardwareConfig Hardware { get; set; } = new();
    public RfidConfig RFID { get; set; } = new();
    public PerformanceConfig Performance { get; set; } = new();
    public OfflineConfig Offline { get; set; } = new();
    public LoggingConfig Logging { get; set; } = new();

    public static AppConfig Load(string path)
    {
        var json = File.ReadAllText(path);
        var cfg = JsonSerializer.Deserialize<AppConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new AppConfig();

        cfg.ApplyEnvironmentOverrides();
        cfg.Normalize();
        return cfg;
    }

    public static AppConfig FromEnvironment()
    {
        var cfg = new AppConfig();
        cfg.ApplyEnvironmentOverrides();
        cfg.Normalize();
        return cfg;
    }

    public void ApplyEnvironmentOverrides()
    {
        string? Env(string key) => Environment.GetEnvironmentVariable(key);
        int EnvInt(string key, int fallback)
            => int.TryParse(Env(key), out var v) ? v : fallback;

        Supabase.Url = Env("SUPABASE_URL") ?? Supabase.Url;
        Supabase.AnonKey = Env("SUPABASE_ANON_KEY") ?? Supabase.AnonKey;
        Auth.Email = Env("RFID_USER_EMAIL") ?? Auth.Email;
        Auth.Password = Env("RFID_USER_PASSWORD") ?? Auth.Password;
        Device.Id = Env("DEVICE_ID") ?? Device.Id;
        Device.ClientType = Env("CLIENT_TYPE") ?? Device.ClientType;
        Realtime.Topic = Env("TOPIC") ?? Realtime.Topic;
        Realtime.WebSocketUrl = Env("REALTIME_WS_URL") ?? Realtime.WebSocketUrl;
        Hardware.DllPath = Env("DLL_PATH") ?? Hardware.DllPath;
        Hardware.DebounceMs = EnvInt("DEBOUNCE_MS", Hardware.DebounceMs);
        RFID.ReaderMode = Env("RFID_READERMODE") ?? RFID.ReaderMode;
        Performance.BatchSize = EnvInt("BATCH_SIZE", Performance.BatchSize);
        Performance.FlushIntervalMs = EnvInt("FLUSH_INTERVAL_MS", Performance.FlushIntervalMs);
        Performance.HeartbeatIntervalMs = EnvInt("HEARTBEAT_INTERVAL_MS", Performance.HeartbeatIntervalMs);
        Offline.SqliteDbPath = Env("SQLITE_DB_PATH") ?? Offline.SqliteDbPath;
        Offline.MaxTags = EnvInt("MAX_OFFLINE_TAGS", Offline.MaxTags);
        Logging.Level = Env("LOG_LEVEL") ?? Logging.Level;
        Logging.Path = Env("LOG_PATH") ?? Logging.Path;
    }

    public void Normalize()
    {
        if (string.IsNullOrWhiteSpace(Auth.Email) && !string.IsNullOrWhiteSpace(Supabase.Email))
            Auth.Email = Supabase.Email;
        if (string.IsNullOrWhiteSpace(Auth.Password) && !string.IsNullOrWhiteSpace(Supabase.Password))
            Auth.Password = Supabase.Password;

        if (string.IsNullOrWhiteSpace(Device.Id) && !string.IsNullOrWhiteSpace(Device.DeviceId))
            Device.Id = Device.DeviceId;
        if (string.IsNullOrWhiteSpace(Device.DeviceId) && !string.IsNullOrWhiteSpace(Device.Id))
            Device.DeviceId = Device.Id;

        if (string.IsNullOrWhiteSpace(Realtime.Topic) && !string.IsNullOrWhiteSpace(Supabase.RealtimeTopic))
            Realtime.Topic = Supabase.RealtimeTopic;
        if (string.IsNullOrWhiteSpace(Supabase.RealtimeTopic) && !string.IsNullOrWhiteSpace(Realtime.Topic))
            Supabase.RealtimeTopic = Realtime.Topic;

        if (string.IsNullOrWhiteSpace(Device.ClientType))
            Device.ClientType = "desktop_csharp";
        if (string.IsNullOrWhiteSpace(Device.Location))
            Device.Location = "Expedição";
    }

    public void Validate()
    {
        var missing = new List<string>();

        if (string.IsNullOrWhiteSpace(Supabase.Url)) missing.Add("Supabase.Url");
        if (string.IsNullOrWhiteSpace(Supabase.AnonKey)) missing.Add("Supabase.AnonKey");
        if (string.IsNullOrWhiteSpace(Auth.Email)) missing.Add("Auth.Email");
        if (string.IsNullOrWhiteSpace(Auth.Password)) missing.Add("Auth.Password");
        if (string.IsNullOrWhiteSpace(Device.Id)) missing.Add("Device.Id");
        if (string.IsNullOrWhiteSpace(Realtime.WebSocketUrl)) missing.Add("Realtime.WebSocketUrl");
        if (string.IsNullOrWhiteSpace(Realtime.Topic)) missing.Add("Realtime.Topic");

        if (missing.Count > 0)
            throw new InvalidOperationException($"Configuração incompleta: {string.Join(", ", missing)}");
    }
}

public sealed class AuthConfig
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public sealed class SupabaseConfig
{
    public string Url { get; set; } = "";
    public string AnonKey { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string RealtimeTopic { get; set; } = "public:leitorR3";
}

public sealed class DeviceConfig
{
    public string Id { get; set; } = "r3-desktop-02";
    public string DeviceId { get; set; } = "r3-desktop-02";
    public string ClientType { get; set; } = "desktop_csharp";
    public string Location { get; set; } = "Expedição";
    public int HeartbeatSeconds { get; set; } = 30;
}

public sealed class RealtimeConfig
{
    public string WebSocketUrl { get; set; } = "";
    public string Topic { get; set; } = "public:leitorR3";
}

public sealed class HardwareConfig
{
    public string DllPath { get; set; } = "";
    public int PowerDbm { get; set; } = 20;
    public int DebounceMs { get; set; } = 300;
}

public sealed class RfidConfig
{
    public string ReaderMode { get; set; } = "Simulated";

    // ===== Reader hardware configuration (optional) =====
    // Se ApplyConfigOnConnect=false (padrão), o app NÃO altera (nem salva) configurações no reader.
    // Isso evita “desconfigurar” o equipamento ao abrir o app.
    public bool ApplyConfigOnConnect { get; set; } = false;

    // Se true, grava configuração em EEPROM (persistente). Use com cuidado.
    // Se false, aplica temporariamente (enquanto o app estiver rodando).
    public bool SaveToEeprom { get; set; } = false;

    // Região em string (case-insensitive): "usa", "europe", "china1", "china2", "korea", "japan".
    // null/"" = não altera.
    public string? Region { get; set; } = null;

    // Potência em dBm (5..30). null = não altera.
    public int? PowerDbm { get; set; } = null;

    // Máscara de antena (bitmask). Ex.: 1 = ANT1, 2 = ANT2, 3 = ANT1+ANT2. null = não altera.
    public int? AntennaMask { get; set; } = null;

    // RFLink mode (depende do firmware). null = não altera.
    public int? RFLink { get; set; } = null;

    // Beep: true/false para ligar/desligar. null = não altera.
    public bool? Beep { get; set; } = null;

    // Modo de leitura EPC/TID/USER. true = setar EPC+TID (0x01). null/false = não altera.
    public bool ApplyEpcTidMode { get; set; } = true;

    // ===== App behavior =====
    public int Power { get; set; } = 25; // (mantido por compatibilidade; preferir PowerDbm)
    public int DebounceMs { get; set; } = 500;
    public int UiUpdateMs { get; set; } = 150;
    public int BatchFlushMs { get; set; } = 300;
    public int BatchSize { get; set; } = 50;
}

public sealed class PerformanceConfig
{
    public int BatchSize { get; set; } = 50;
    public int FlushIntervalMs { get; set; } = 500;
    public int HeartbeatIntervalMs { get; set; } = 30000;
}

public sealed class OfflineConfig
{
    public string SqliteDbPath { get; set; } = "rfid_buffer.db";
    public int MaxTags { get; set; } = 10000;
}

public sealed class LoggingConfig
{
    public string Level { get; set; } = "Info";
    public string Path { get; set; } = "logs/mepo_desktop.log";
}
