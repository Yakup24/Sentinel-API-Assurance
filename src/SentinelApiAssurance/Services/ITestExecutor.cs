using SentinelApiAssurance.Models;

namespace SentinelApiAssurance.Services;

public interface ITestExecutor
{
    bool CanExecute(TestCase testCase);
    Task<RawTestResponse> ExecuteAsync(EnvironmentConfig environment, ServiceConfig service, TestCase testCase, AppConfig config);
}

public sealed class RawTestResponse
{
    public int? HttpStatus { get; set; }
    public string Body { get; set; } = "";
    public long DurationMs { get; set; }
    public string Message { get; set; } = "";
}
