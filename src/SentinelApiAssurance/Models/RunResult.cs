namespace SentinelApiAssurance.Models;

public sealed class RunResult
{
    public string SuiteName { get; set; } = "";
    public string EnvironmentName { get; set; } = "";
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public List<TestResult> Results { get; set; } = new();

    public int TotalCount => Results.Count;
    public int PassedCount => Results.Count(x => x.Status == TestStatus.Passed);
    public int FailedCount => Results.Count(x => x.Status == TestStatus.Failed);
    public int SkippedCount => Results.Count(x => x.Status == TestStatus.Skipped);
    public double CoverageRate => TotalCount == 0 ? 0 : Math.Round((PassedCount * 100.0) / TotalCount, 2);
}

public sealed class TestResult
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Service { get; set; } = "";
    public string Operation { get; set; } = "";
    public string Protocol { get; set; } = "";
    public TestStatus Status { get; set; }
    public int? HttpStatus { get; set; }
    public long DurationMs { get; set; }
    public string RequestFile { get; set; } = "";
    public string Message { get; set; } = "";
    public List<AssertionResult> Assertions { get; set; } = new();
}

public sealed class AssertionResult
{
    public string Type { get; set; } = "";
    public bool Passed { get; set; }
    public string Message { get; set; } = "";
}

public enum TestStatus
{
    Passed,
    Failed,
    Skipped
}
