using System.IO;
using Serilog;
using Serilog.Events;

namespace MepoExpedicaoRfid.Services;

public sealed class AppLogger : IDisposable
{
    private readonly ILogger _logger;
    private bool _disposed;

    public AppLogger(string level = "Info", string? filePath = null)
    {
        var logLevel = ParseLogLevel(level);
        
        var config = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

        // Usa filePath do config se fornecido, senão padrão
        var logPath = filePath ?? "logs/mepo_desktop.log";
        var dir = Path.GetDirectoryName(logPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        config.WriteTo.File(
            logPath,
            rollingInterval: RollingInterval.Day,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
            retainedFileCountLimit: 30);

        _logger = config.CreateLogger();
        Log.Logger = _logger;
    }

    private static LogEventLevel ParseLogLevel(string level)
    {
        return level?.ToLowerInvariant() switch
        {
            "debug" => LogEventLevel.Debug,
            "info" or "information" => LogEventLevel.Information,
            "warn" or "warning" => LogEventLevel.Warning,
            "error" => LogEventLevel.Error,
            "fatal" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }

    public void Debug(string message) => _logger.Debug(message);
    public void Info(string message) => _logger.Information(message);
    public void Warn(string message) => _logger.Warning(message);
    public void Error(string message, Exception? ex = null) => _logger.Error(ex, message);
    public void Fatal(string message, Exception? ex = null) => _logger.Fatal(ex, message);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Log.CloseAndFlush();
    }
}
