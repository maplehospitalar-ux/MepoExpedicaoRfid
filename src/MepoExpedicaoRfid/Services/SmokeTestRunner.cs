using System.IO;
using MepoExpedicaoRfid.Models;
using MepoExpedicaoRfid.Services;

namespace MepoExpedicaoRfid;

public static class SmokeTestRunner
{
    public static async Task<int> RunAsync(CancellationToken ct)
    {
        Console.WriteLine("[SMOKE TEST] === ENTERING RunAsync ===");
        AppLogger? log = null;
        try
        {
            Console.WriteLine("[SMOKE TEST] Loading config...");
            var cfg = LoadConfig();
            Console.WriteLine($"[SMOKE TEST] Config loaded: DeviceId={cfg.Device?.Id ?? "NULL"}");
            
            Console.WriteLine("[SMOKE TEST] Validating config...");
            cfg.Validate();
            Console.WriteLine("[SMOKE TEST] Config validated");

            Console.WriteLine("[SMOKE TEST] Creating AppLogger...");
            log = new AppLogger(cfg.Logging.Level, cfg.Logging.Path);
            Console.WriteLine("[SMOKE TEST] AppLogger created");
            
            log.Info("=== SMOKE TEST INICIADO ===");

            var auth = new SupabaseAuthService(cfg.Supabase.Url, cfg.Supabase.AnonKey, cfg.Auth.Email, cfg.Auth.Password, log);
            await auth.SignInAsync();

            var supabase = new SupabaseService(cfg.Supabase, cfg.Auth, cfg.Device, log);
            await supabase.ConnectAsync();

            var realtime = new RealtimeService(cfg.Realtime.WebSocketUrl, cfg.Supabase.AnonKey, cfg.Realtime.Topic, cfg.Device.Id, log);
            await realtime.ConnectAsync();

            var offline = new OfflineBufferService(cfg.Offline.SqliteDbPath, cfg.Offline.MaxTags, log);
            var batch = new BatchTagInsertService(supabase, offline, log, cfg.Performance.BatchSize, cfg.Performance.FlushIntervalMs);
            var session = new SessionStateManager(log);

            IRfidReader reader = cfg.RFID.ReaderMode.Equals("R3Dll", StringComparison.OrdinalIgnoreCase)
                ? new R3DllReader(cfg.RFID, log)
                : new SimulatedRfidReader(cfg.RFID, log);

            var pipeline = new TagPipeline(reader, cfg.RFID, log, session, batch, realtime, supabase);
            await pipeline.StartAsync();

            // SAÍDA
            var vendaNumero = $"TESTE-{DateTime.UtcNow:yyyyMMddHHmmss}";
            log.Info($"Criando sessão de saída: origem=OMIE, venda={vendaNumero}");
            var saida = await supabase.CriarSessaoSaidaAsync("OMIE", vendaNumero);
            
            log.Info($"Resposta da sessão de saída: Success={saida.Success}, SessionId={saida.SessionId}, Message={saida.Message}, ErrorMessage={saida.ErrorMessage}");
            
            if (string.IsNullOrWhiteSpace(saida.SessionId))
            {
                log.Error($"SessionId vazio ou nulo na resposta");
                return 1;
            }

            log.Info($"Sessão de saída criada: {saida.SessionId}");

            session.StartSession(new SessionInfo
            {
                SessionId = saida.SessionId,
                Tipo = SessionType.Saida,
                Origem = "OMIE",
                VendaNumero = vendaNumero,
                ReaderId = cfg.Device.Id,
                ClientType = cfg.Device.ClientType
            });

            await realtime.BroadcastReaderStartAsync(saida.SessionId);
            await pipeline.BeginReadingAsync();
            await Task.Delay(2000, ct);
            await pipeline.EndReadingAsync();
            await batch.FlushAsync();
            await offline.SyncWithBackendAsync(batch);
            await supabase.FinalizarSessaoEdgeAsync(saida.SessionId, "saida");
            session.EndSession();
            await realtime.BroadcastReaderStopAsync(saida.SessionId);

            // ENTRADA
            var entrada = await supabase.CriarSessaoEntradaAsync("SKU-TEST", "L2025A", DateTime.UtcNow.Date.AddDays(-30), DateTime.UtcNow.Date.AddYears(1));
            if (!entrada.Success || string.IsNullOrWhiteSpace(entrada.SessionId))
            {
                log.Error($"Falha ao criar sessão de entrada: {entrada.ErrorMessage ?? entrada.Message}");
                return 2;
            }

            session.StartSession(new SessionInfo
            {
                SessionId = entrada.SessionId,
                Tipo = SessionType.Entrada,
                Sku = "SKU-TEST",
                Lote = "L2025A",
                EntradaId = entrada.EntradaId,
                DataFabricacao = DateTime.UtcNow.Date.AddDays(-30),
                DataValidade = DateTime.UtcNow.Date.AddYears(1),
                ReaderId = cfg.Device.Id,
                ClientType = cfg.Device.ClientType
            });

            await realtime.BroadcastReaderStartAsync(entrada.SessionId);
            await pipeline.BeginReadingAsync();
            await Task.Delay(2000, ct);
            await pipeline.EndReadingAsync();
            await batch.FlushAsync();
            await offline.SyncWithBackendAsync(batch);
            await supabase.FinalizarSessaoEdgeAsync(entrada.SessionId, "entrada");
            session.EndSession();
            await realtime.BroadcastReaderStopAsync(entrada.SessionId);

            await pipeline.StopAsync();
            await realtime.DisconnectAsync();

            log.Info("=== SMOKE TEST FINALIZADO ===");
            return 0;
        }
        catch (Exception ex)
        {
            log?.Error("Erro no smoke test", ex);
            return 3;
        }
        finally
        {
            log?.Dispose();
        }
    }

    private static AppConfig LoadConfig()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var configPath = Path.Combine(baseDir, "appsettings.json");
        if (File.Exists(configPath))
            return AppConfig.Load(configPath);

        var currentPath = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
        if (File.Exists(currentPath))
            return AppConfig.Load(currentPath);

        return AppConfig.FromEnvironment();
    }
}
