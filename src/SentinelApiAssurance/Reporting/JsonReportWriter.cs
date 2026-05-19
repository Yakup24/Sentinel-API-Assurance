using System.Text.Json;
using SentinelApiAssurance.Models;

namespace SentinelApiAssurance.Reporting;

public static class JsonReportWriter
{
    public static string Write(string reportDirectory, RunResult run)
    {
        var path = Path.Combine(reportDirectory, $"Summary_{DateTime.Now:yyyyMMdd_HHmmss}.json");

        File.WriteAllText(path, JsonSerializer.Serialize(run, new JsonSerializerOptions
        {
            WriteIndented = true
        }));

        return path;
    }
}
