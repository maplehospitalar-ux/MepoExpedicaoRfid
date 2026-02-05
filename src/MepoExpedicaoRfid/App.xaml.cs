using System.Windows;
using System.IO;
using Serilog;
using System.Linq;

namespace MepoExpedicaoRfid;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Captura exceções não tratadas GLOBALMENTE (incluindo P/Invoke crashes)
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            var errorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash_dump.txt");
            File.WriteAllText(errorPath, $"[FATAL EXCEPTION - {DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n{ex}\n\nIsTerminating: {args.IsTerminating}");
            FallbackLog($"CRASH DETECTADO: {ex?.Message}");
            FallbackLog($"Stack: {ex?.StackTrace}");
        };
        
        // Captura exceções não observadas em Tasks
        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            var errorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "task_exception.txt");
            File.WriteAllText(errorPath, $"[TASK EXCEPTION - {DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n{args.Exception}");
            FallbackLog($"TASK EXCEPTION: {args.Exception?.Message}");
            args.SetObserved(); // Evita que aplicação feche
        };

        FallbackLog("=== INICIANDO MEPO Desktop-Expedição C# ===");
        FallbackLog($"Diretório atual: {Environment.CurrentDirectory}");
        FallbackLog($"Diretório base: {AppDomain.CurrentDomain.BaseDirectory}");

        if (e.Args.Any(arg => string.Equals(arg, "--smoketest", StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("[MAIN] Detected --smoketest flag");
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            
            // Usa Dispatcher.InvokeAsync para garantir que execute no contexto WPF
            Dispatcher.InvokeAsync(async () =>
            {
                Console.WriteLine("[MAIN] Invoking RunSmokeTestAsync");
                await RunSmokeTestAsync();
            });
            
            return;
        }

        try
        {
            // Configurar Serilog
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "mepo_desktop.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("=== INICIANDO MEPO Desktop-Expedição C# ===");
            Log.Information("Diretório atual: {CurrentDirectory}", Environment.CurrentDirectory);
            Log.Information("Diretório base: {BaseDirectory}", AppDomain.CurrentDomain.BaseDirectory);
        }
        catch (Exception ex)
        {
            FallbackLog($"Falha ao iniciar Serilog: {ex}");
        }

        try
        {
            base.OnStartup(e);
            Log.Information("Aplicação iniciada com sucesso");
            FallbackLog("Aplicação iniciada com sucesso");
            
            // Inicializar MainViewModel via Bootstrapper (async)
            Log.Information("Inicializando MainViewModel...");
            FallbackLog("Inicializando MainViewModel via Bootstrapper...");
            
            // ✅ Usa Dispatcher para executar código async no contexto WPF
            Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    var mainVm = await Bootstrapper.BuildMainViewModel();
                    Log.Information("MainViewModel inicializado com sucesso");
                    FallbackLog("MainViewModel inicializado com sucesso");
                    
                    // Criar MainWindow e atribuir DataContext ANTES de mostrar
                    var mainWindow = new MainWindow
                    {
                        DataContext = mainVm
                    };
                    
                    // Mostrar a janela
                    mainWindow.Show();
                    Log.Information("MainWindow mostrada com sucesso");
                    FallbackLog("MainWindow mostrada com sucesso");
                }
                catch (Exception bootstrapEx)
                {
                    Log.Error(bootstrapEx, "ERRO NO BOOTSTRAPPER");
                    FallbackLog($"ERRO NO BOOTSTRAPPER: {bootstrapEx}");
                    FallbackLog($"Inner Exception: {bootstrapEx.InnerException}");
                    FallbackLog($"Stack Trace: {bootstrapEx.StackTrace}");
                    MessageBox.Show($"Erro ao iniciar aplicação: {bootstrapEx.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown(1);
                }
            });
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Erro fatal ao iniciar aplicação");
            FallbackLog($"Erro fatal ao iniciar aplicação: {ex}");
            FallbackLog($"Stack Trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                FallbackLog($"InnerException: {ex.InnerException}");
                FallbackLog($"InnerEx StackTrace: {ex.InnerException.StackTrace}");
            }
            Shutdown(1);
        }
    }

    private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error(e.Exception, "Exceção não tratada");
        FallbackLog($"Exceção não tratada: {e.Exception}");
        e.Handled = true;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Aplicação finalizada");
        FallbackLog("Aplicação finalizada");
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private async Task RunSmokeTestAsync()
    {
        // Captura exceções ANTES do Serilog
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var errorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash_dump.txt");
            File.WriteAllText(errorPath, $"[FATAL EXCEPTION]\n{e.ExceptionObject}\n\nIsTerminating: {e.IsTerminating}");
            Console.WriteLine($"[FATAL] Exception written to {errorPath}");
        };

        try
        {
            Console.WriteLine("[SMOKE TEST] Starting smoke test runner...");
            var code = await SmokeTestRunner.RunAsync(CancellationToken.None);
            FallbackLog($"Smoke test finalizado com código: {code}");
        }
        catch (Exception ex)
        {
            var errorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "smoketest_error.txt");
            File.WriteAllText(errorPath, $"[SMOKE TEST ERROR]\n{ex}");
            Console.WriteLine($"[ERROR] Details written to {errorPath}");
            FallbackLog($"Smoke test falhou: {ex}");
        }
        finally
        {
            Shutdown(0);
        }
    }

    private static void FallbackLog(string message)
    {
        try
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup_log.txt");
            File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
        }
        catch
        {
            // ignora
        }
    }
}
