namespace SentinelApiAssurance.Models;

public sealed class AppConfig
{
    public string DefaultEnvironment { get; set; } = "STB";
    public string DefaultSuitePath { get; set; } = "Suites/voltran-enterprise-regression-suite.json";
    public string ReportDirectory { get; set; } = "Reports";
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 1;
    public int RetryDelayMs { get; set; } = 500;
    public int DefaultMaxDurationMs { get; set; } = 5000;
    public bool BlockDangerousOperationsWithoutExplicitApproval { get; set; } = true;
    public List<string> DangerousOperationKeywords { get; set; } = new()
    {
        "submit", "create", "activate", "deactivate", "deactivation",
        "delete", "remove", "cancel", "upsert", "update", "set",
        "unset", "insert", "add", "change", "payment", "order", "callback",
        "inform", "correction"
    };

    public Dictionary<string, string> GlobalHeaders { get; set; } = new();
    public Dictionary<string, string> TestData { get; set; } = new();
    public List<EnvironmentConfig> Environments { get; set; } = new();
}

public sealed class EnvironmentConfig
{
    public string Name { get; set; } = "";
    public string BaseUrl { get; set; } = "";
    public Dictionary<string, ServiceConfig> Services { get; set; } = new();
    public Dictionary<string, string> Headers { get; set; } = new();
}

public sealed class ServiceConfig
{
    public string Endpoint { get; set; } = "";
    public string SoapVersion { get; set; } = "1.1";
    public string SoapActionFormat { get; set; } = "{operation}";
    public Dictionary<string, string> Headers { get; set; } = new();

    public string BuildUrl(string baseUrl)
    {
        if (Endpoint.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            Endpoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return Endpoint;

        return $"{baseUrl.TrimEnd('/')}/{Endpoint.TrimStart('/')}";
    }
}
