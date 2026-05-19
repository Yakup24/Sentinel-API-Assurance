using System.Text;
using SentinelApiAssurance.Models;

namespace SentinelApiAssurance.Reporting;

public static class CsvReportWriter
{
    public static string Write(string reportDirectory, RunResult run)
    {
        var path = Path.Combine(reportDirectory, $"Summary_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        var sb = new StringBuilder();

        sb.AppendLine("Status;Service;Operation;Protocol;HttpStatus;DurationMs;RequestFile;Message");

        foreach (var r in run.Results)
        {
            sb.AppendLine(string.Join(";",
                Escape(r.Status.ToString()),
                Escape(r.Service),
                Escape(r.Operation),
                Escape(r.Protocol),
                Escape(r.HttpStatus?.ToString() ?? ""),
                Escape(r.DurationMs.ToString()),
                Escape(r.RequestFile),
                Escape(r.Message)
            ));
        }

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        return path;
    }

    private static string Escape(string value)
    {
        value = value.Replace("\"", "\"\"");
        return $"\"{value}\"";
    }
}
