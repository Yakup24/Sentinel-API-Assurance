using System.Text.Json;
using SentinelApiAssurance.Configuration;

namespace SentinelApiAssurance.Tests;

public sealed class ConfigLoaderTests
{
    [Fact]
    public void LoadAppConfig_Throws_When_Default_Environment_Is_Not_Defined()
    {
        using var temp = new TemporaryDirectory();
        var configPath = Path.Combine(temp.Path, "appsettings.json");
        File.WriteAllText(configPath, """
        {
          "DefaultEnvironment": "missing",
          "Environments": []
        }
        """);

        var ex = Assert.Throws<InvalidOperationException>(() => ConfigLoader.LoadAppConfig(configPath));

        Assert.Contains("At least one environment", ex.Message);
    }

    [Fact]
    public void LoadTestSuite_Throws_When_Case_Service_Is_Missing()
    {
        using var temp = new TemporaryDirectory();
        var suitePath = Path.Combine(temp.Path, "suite.json");
        File.WriteAllText(suitePath, """
        {
          "Name": "invalid-suite",
          "Cases": [
            {
              "Id": "CASE-001",
              "Protocol": "SOAP",
              "Operation": "GetCustomer"
            }
          ]
        }
        """);

        var ex = Assert.Throws<InvalidOperationException>(() => ConfigLoader.LoadTestSuite(suitePath));

        Assert.Contains("must define a Service", ex.Message);
    }

    [Fact]
    public void LoadAppConfig_Reads_Valid_Config()
    {
        using var temp = new TemporaryDirectory();
        var configPath = Path.Combine(temp.Path, "appsettings.json");
        File.WriteAllText(configPath, JsonSerializer.Serialize(new
        {
            DefaultEnvironment = "demo",
            Environments = new[]
            {
                new
                {
                    Name = "demo",
                    BaseUrl = "https://api.example.test",
                    Services = new Dictionary<string, object>
                    {
                        ["CustomerService"] = new { Endpoint = "CustomerService" }
                    }
                }
            }
        }));

        var config = ConfigLoader.LoadAppConfig(configPath);

        Assert.Equal("demo", config.DefaultEnvironment);
        Assert.True(config.Environments.Single().Services.ContainsKey("CustomerService"));
    }
}
