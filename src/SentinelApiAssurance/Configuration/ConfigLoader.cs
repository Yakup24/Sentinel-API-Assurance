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
        if (config is null)
            throw new InvalidOperationException("appsettings.json could not be read.");

        ValidateAppConfig(config);
        return config;
    }

    public static TestSuite LoadTestSuite(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Test suite not found.", path);

        var suite = JsonSerializer.Deserialize<TestSuite>(File.ReadAllText(path), JsonOptions);
        if (suite is null)
            throw new InvalidOperationException("Test suite could not be read.");

        ValidateTestSuite(suite);
        return suite;
    }

    private static void ValidateAppConfig(AppConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.DefaultEnvironment))
            throw new InvalidOperationException("DefaultEnvironment is required.");

        if (config.Environments.Count == 0)
            throw new InvalidOperationException("At least one environment must be configured.");

        if (!config.Environments.Any(x => string.Equals(x.Name, config.DefaultEnvironment, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"DefaultEnvironment '{config.DefaultEnvironment}' is not defined in Environments.");

        foreach (var environment in config.Environments)
        {
            if (string.IsNullOrWhiteSpace(environment.Name))
                throw new InvalidOperationException("Environment name is required.");

            if (string.IsNullOrWhiteSpace(environment.BaseUrl))
                throw new InvalidOperationException($"BaseUrl is required for environment '{environment.Name}'.");

            foreach (var (serviceName, service) in environment.Services)
            {
                if (string.IsNullOrWhiteSpace(serviceName))
                    throw new InvalidOperationException($"A service key is empty in environment '{environment.Name}'.");

                if (string.IsNullOrWhiteSpace(service.Endpoint))
                    throw new InvalidOperationException($"Endpoint is required for service '{serviceName}' in environment '{environment.Name}'.");
            }
        }
    }

    private static void ValidateTestSuite(TestSuite suite)
    {
        if (string.IsNullOrWhiteSpace(suite.Name))
            throw new InvalidOperationException("Test suite Name is required.");

        foreach (var testCase in suite.Cases)
        {
            if (string.IsNullOrWhiteSpace(testCase.Id))
                throw new InvalidOperationException("Each test case must define an Id.");

            if (string.IsNullOrWhiteSpace(testCase.Protocol))
                throw new InvalidOperationException($"Test case '{testCase.Id}' must define a Protocol.");

            if (string.IsNullOrWhiteSpace(testCase.Service))
                throw new InvalidOperationException($"Test case '{testCase.Id}' must define a Service.");

            if (string.IsNullOrWhiteSpace(testCase.Operation))
                throw new InvalidOperationException($"Test case '{testCase.Id}' must define an Operation.");
        }
    }
}
