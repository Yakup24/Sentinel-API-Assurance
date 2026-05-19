using SentinelApiAssurance.Models;
using SentinelApiAssurance.Safety;
using SentinelApiAssurance.Services;
using SentinelApiAssurance.Utilities;

namespace SentinelApiAssurance.Execution;

public sealed class TestRunner
{
    private readonly AppConfig _config;
    private readonly EnvironmentConfig _environment;
    private readonly TestSuite _suite;
    private readonly ITestExecutor[] _executors;
    private readonly OperationSafetyPolicy _safetyPolicy;
    private readonly FileLogger _logger;

    public TestRunner(
        AppConfig config,
        EnvironmentConfig environment,
        TestSuite suite,
        ITestExecutor[] executors,
        OperationSafetyPolicy safetyPolicy,
        FileLogger logger)
    {
        _config = config;
        _environment = environment;
        _suite = suite;
        _executors = executors;
        _safetyPolicy = safetyPolicy;
        _logger = logger;
    }

    public async Task<RunResult> RunAsync()
    {
        var run = new RunResult
        {
            SuiteName = _suite.Name,
            EnvironmentName = _environment.Name,
            StartedAt = DateTime.Now
        };

        foreach (var testCase in _suite.Cases.Where(x => x.Active))
        {
            var result = await RunSingleAsync(testCase);
            run.Results.Add(result);

            Console.WriteLine($"{result.Status,-7} | {result.Service}.{result.Operation} | HTTP:{result.HttpStatus?.ToString() ?? "-"} | {result.DurationMs} ms | {result.Message}");
        }

        run.FinishedAt = DateTime.Now;
        return run;
    }

    private async Task<TestResult> RunSingleAsync(TestCase testCase)
    {
        var result = new TestResult
        {
            Id = testCase.Id,
            Title = testCase.Title,
            Service = testCase.Service,
            Operation = testCase.Operation,
            Protocol = testCase.Protocol,
            RequestFile = testCase.RequestBodyFile ?? ""
        };

        if (_safetyPolicy.ShouldBlock(testCase, out var blockReason))
        {
            result.Status = TestStatus.Skipped;
            result.Message = blockReason;
            return result;
        }

        if (!_environment.Services.TryGetValue(testCase.Service, out var service))
        {
            result.Status = TestStatus.Failed;
            result.Message = $"Service is not defined in the selected environment: {testCase.Service}";
            return result;
        }

        var executor = _executors.FirstOrDefault(x => x.CanExecute(testCase));
        if (executor is null)
        {
            result.Status = TestStatus.Failed;
            result.Message = $"Unsupported protocol: {testCase.Protocol}";
            return result;
        }

        _logger.Info($"Running test: {testCase.Id}");

        var raw = await executor.ExecuteAsync(_environment, service, testCase, _config);

        result.HttpStatus = raw.HttpStatus;
        result.DurationMs = raw.DurationMs;

        var assertions = AssertionEngine.Evaluate(testCase, raw);
        result.Assertions = assertions;

        var httpOk = raw.HttpStatus == testCase.ExpectedHttpStatus;
        var assertionsOk = assertions.All(x => x.Passed);

        result.Status = httpOk && assertionsOk ? TestStatus.Passed : TestStatus.Failed;

        if (!httpOk)
        {
            result.Message = $"Expected HTTP {testCase.ExpectedHttpStatus}, actual HTTP {raw.HttpStatus?.ToString() ?? "-"}";
        }
        else if (!assertionsOk)
        {
            result.Message = assertions.First(x => !x.Passed).Message;
        }
        else
        {
            result.Message = "Test passed.";
        }

        return result;
    }
}
