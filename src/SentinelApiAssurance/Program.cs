using SentinelApiAssurance.Configuration;
using SentinelApiAssurance.Execution;
using SentinelApiAssurance.Models;
using SentinelApiAssurance.Reporting;
using SentinelApiAssurance.Safety;
using SentinelApiAssurance.Services;
using SentinelApiAssurance.Utilities;

namespace SentinelApiAssurance;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var cli = CliOptions.Parse(args);
        var logger = new FileLogger(Path.Combine(AppContext.BaseDirectory, "Logs"));
        logger.Info("Sentinel API Assurance started.");

        try
        {
            var appSettingsPath = cli.ConfigPath ?? Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            var config = ConfigLoader.LoadAppConfig(appSettingsPath);

            var environmentName = cli.EnvironmentName ?? config.DefaultEnvironment;
            var environment = config.Environments.FirstOrDefault(x =>
                string.Equals(x.Name, environmentName, StringComparison.OrdinalIgnoreCase));

            if (environment is null)
            {
                Console.WriteLine($"Environment not found: {environmentName}");
                Console.WriteLine($"Defined environments: {string.Join(", ", config.Environments.Select(x => x.Name))}");
                return 2;
            }

            var suitePath = cli.SuitePath ?? Path.Combine(AppContext.BaseDirectory, config.DefaultSuitePath);
            var suite = cli.LegacyCallsPath is not null
                ? LegacyCallImporter.Import(cli.LegacyCallsPath, config, environment)
                : ConfigLoader.LoadTestSuite(suitePath);

            var reportDirectory = Path.Combine(AppContext.BaseDirectory, config.ReportDirectory);
            Directory.CreateDirectory(reportDirectory);

            var safetyPolicy = new OperationSafetyPolicy(config);

            if (cli.DryRun)
            {
                PrintDryRunSummary(suite, environment, safetyPolicy);
                return 0;
            }

            using var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds)
            };

            var executors = new ITestExecutor[]
            {
                new SoapTestExecutor(httpClient, logger),
                new RestTestExecutor(httpClient, logger)
            };

            var runner = new TestRunner(config, environment, suite, executors, safetyPolicy, logger);
            var runResult = await runner.RunAsync();

            var htmlReport = HtmlReportWriter.Write(reportDirectory, runResult);
            var jsonReport = JsonReportWriter.Write(reportDirectory, runResult);
            var csvReport = CsvReportWriter.Write(reportDirectory, runResult);

            Console.WriteLine();
            Console.WriteLine("Sentinel API Assurance reports:");
            Console.WriteLine($"HTML : {htmlReport}");
            Console.WriteLine($"JSON : {jsonReport}");
            Console.WriteLine($"CSV  : {csvReport}");
            Console.WriteLine();

            Console.WriteLine($"Total: {runResult.TotalCount}");
            Console.WriteLine($"Passed: {runResult.PassedCount}");
            Console.WriteLine($"Failed: {runResult.FailedCount}");
            Console.WriteLine($"Skipped: {runResult.SkippedCount}");
            Console.WriteLine($"Success rate: %{runResult.CoverageRate}");

            logger.Info("Sentinel API Assurance completed.");
            return runResult.FailedCount > 0 ? 1 : 0;
        }
        catch (Exception ex)
        {
            logger.Error("Fatal error", ex);
            Console.WriteLine($"Error: {ex.Message}");
            return 99;
        }
    }

    private static void PrintDryRunSummary(TestSuite suite, EnvironmentConfig environment, OperationSafetyPolicy safetyPolicy)
    {
        var activeCases = suite.Cases.Where(x => x.Active).ToList();
        var blockedCases = activeCases.Where(x => safetyPolicy.ShouldBlock(x, out _)).ToList();
        var missingServices = activeCases
            .Where(x => !environment.Services.ContainsKey(x.Service))
            .Select(x => x.Service)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
        var missingTemplates = activeCases
            .Where(x => !string.IsNullOrWhiteSpace(x.RequestBodyFile))
            .Where(x => !File.Exists(ResolveCaseFile(x.RequestBodyFile!)))
            .ToList();

        Console.WriteLine("Sentinel API Assurance dry-run");
        Console.WriteLine($"Suite       : {suite.Name}");
        Console.WriteLine($"Environment : {environment.Name}");
        Console.WriteLine($"Cases       : {activeCases.Count} active / {suite.Cases.Count} total");
        Console.WriteLine($"Services    : {activeCases.Select(x => x.Service).Distinct(StringComparer.OrdinalIgnoreCase).Count()}");
        Console.WriteLine($"Safety skip : {blockedCases.Count}");
        Console.WriteLine($"Missing svc : {missingServices.Count}");
        Console.WriteLine($"Missing req : {missingTemplates.Count}");
        Console.WriteLine();

        foreach (var group in activeCases.GroupBy(x => x.Service).OrderBy(x => x.Key))
        {
            var serviceStatus = environment.Services.ContainsKey(group.Key) ? "registered" : "missing";
            var blockedCount = group.Count(x => safetyPolicy.ShouldBlock(x, out _));
            Console.WriteLine($"{group.Key,-45} {group.Count(),3} cases | {serviceStatus} | stateful blocked: {blockedCount}");
        }

        if (missingServices.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Missing services:");
            foreach (var service in missingServices)
                Console.WriteLine($"- {service}");
        }

        if (missingTemplates.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Missing request templates:");
            foreach (var testCase in missingTemplates.Take(25))
                Console.WriteLine($"- {testCase.RequestBodyFile}");

            if (missingTemplates.Count > 25)
                Console.WriteLine($"... {missingTemplates.Count - 25} more");
        }
    }

    private static string ResolveCaseFile(string path)
    {
        return Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path);
    }
}
