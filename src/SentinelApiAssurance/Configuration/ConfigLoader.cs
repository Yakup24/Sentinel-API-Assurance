using System.Text.Json;
using SentinelApiAssurance.Models;

namespace SentinelApiAssurance.Configuration;

public static class ConfigLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        WriteIndented = true
    };

    public static AppConfig LoadAppConfig(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("appsettings.json not found.", path);

        var config = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(path), JsonOptions);
        return config ?? throw new InvalidOperationException("appsettings.json could not be read.");
    }

    public static TestSuite LoadTestSuite(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Test suite not found.", path);

        var suite = JsonSerializer.Deserialize<TestSuite>(File.ReadAllText(path), JsonOptions);
        return suite ?? throw new InvalidOperationException("Test suite could not be read.");
    }
}
