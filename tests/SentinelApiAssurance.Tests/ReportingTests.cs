using SentinelApiAssurance.Models;
using SentinelApiAssurance.Reporting;

namespace SentinelApiAssurance.Tests;

public sealed class ReportingTests
{
    [Fact]
    public void JsonReportWriter_Writes_Report_File_For_Empty_Run()
    {
        using var temp = new TemporaryDirectory();
        var run = new RunResult { SuiteName = "empty-suite", EnvironmentName = "demo" };

        var path = JsonReportWriter.Write(temp.Path, run);

        Assert.True(File.Exists(path));
        Assert.Contains("empty-suite", File.ReadAllText(path));
    }

    [Fact]
    public void CsvReportWriter_Writes_Header_And_Result_Row()
    {
        using var temp = new TemporaryDirectory();
        var run = new RunResult
        {
            SuiteName = "demo-suite",
            EnvironmentName = "demo",
            Results =
            {
                new TestResult
                {
                    Status = TestStatus.Passed,
                    Service = "CustomerService",
                    Operation = "GetCustomer",
                    Protocol = "SOAP",
                    HttpStatus = 200,
                    DurationMs = 25,
                    RequestFile = "request.xml",
                    Message = "Test passed."
                }
            }
        };

        var path = CsvReportWriter.Write(temp.Path, run);
        var content = File.ReadAllText(path);

        Assert.Contains("Status;Service;Operation;Protocol;HttpStatus;DurationMs;RequestFile;Message", content);
        Assert.Contains("\"CustomerService\"", content);
    }
}
