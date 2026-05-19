using System.Text;

namespace SentinelApiAssurance.Utilities;

public sealed class FileLogger
{
    private readonly string _logFile;

    public FileLogger(string logDirectory)
    {
        Directory.CreateDirectory(logDirectory);
        _logFile = Path.Combine(logDirectory, $"runner_{DateTime.Now:yyyyMMdd}.log");
    }

    public void Info(string message) => Write("INFO", message);

    public void Warn(string message) => Write("WARN", message);

    public void Error(string message, Exception? exception = null)
    {
        var detail = exception is null ? message : $"{message} | {exception}";
        Write("ERROR", detail);
    }

    private void Write(string level, string message)
    {
        var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}\t{level}\t{message}{Environment.NewLine}";
        File.AppendAllText(_logFile, line, Encoding.UTF8);
    }
}
