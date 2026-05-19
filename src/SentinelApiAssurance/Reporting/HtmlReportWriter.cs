using System.Net;
using System.Text;
using SentinelApiAssurance.Models;

namespace SentinelApiAssurance.Reporting;

public static class HtmlReportWriter
{
    public static string Write(string reportDirectory, RunResult run)
    {
        var path = Path.Combine(reportDirectory, $"Summary_{DateTime.Now:yyyyMMdd_HHmmss}.html");
        var serviceRows = string.Join(Environment.NewLine, run.Results
            .GroupBy(x => x.Service)
            .OrderBy(x => x.Key)
            .Select(group =>
            {
                var total = group.Count();
                var passed = group.Count(x => x.Status == TestStatus.Passed);
                var failed = group.Count(x => x.Status == TestStatus.Failed);
                var skipped = group.Count(x => x.Status == TestStatus.Skipped);

                return $"""
                <tr>
                  <td>{Encode(group.Key)}</td>
                  <td>{total}</td>
                  <td>{passed}</td>
                  <td>{failed}</td>
                  <td>{skipped}</td>
                </tr>
                """;
            }));

        var rows = string.Join(Environment.NewLine, run.Results.Select(r =>
        {
            var assertions = string.Join("<br>", r.Assertions.Select(a =>
                $"{Encode(a.Type)}: {(a.Passed ? "OK" : "NOK")} - {Encode(a.Message)}"));

            return $"""
            <tr class="{r.Status.ToString().ToLowerInvariant()}">
              <td>{Encode(r.Status.ToString())}</td>
              <td>{Encode(r.Service)}</td>
              <td>{Encode(r.Operation)}</td>
              <td>{Encode(r.Protocol)}</td>
              <td>{Encode(r.HttpStatus?.ToString() ?? "-")}</td>
              <td>{r.DurationMs}</td>
              <td>{Encode(r.RequestFile)}</td>
              <td>{Encode(r.Message)}</td>
              <td>{assertions}</td>
            </tr>
            """;
        }));

        var html = $$"""
        <!doctype html>
        <html lang="tr">
        <head>
          <meta charset="utf-8">
          <title>Sentinel API Assurance Report</title>
          <style>
            body {
              font-family: Arial, sans-serif;
              background: #f8fafc;
              color: #0f172a;
              margin: 28px;
            }
            .header {
              background: #111827;
              color: white;
              padding: 22px;
              border-radius: 8px;
              margin-bottom: 18px;
            }
            .header h1 {
              margin: 0 0 8px;
              font-size: 26px;
              letter-spacing: 0;
            }
            .cards {
              display: flex;
              gap: 12px;
              flex-wrap: wrap;
              margin: 18px 0 24px;
            }
            .card {
              background: white;
              border: 1px solid #e5e7eb;
              border-radius: 8px;
              padding: 14px 18px;
              min-width: 140px;
              box-shadow: 0 2px 10px rgba(15,23,42,.06);
            }
            .card b {
              display: block;
              color: #64748b;
              font-size: 13px;
              margin-bottom: 6px;
            }
            .card span {
              font-size: 25px;
              font-weight: 800;
            }
            h2 {
              font-size: 18px;
              margin-top: 28px;
            }
            table {
              width: 100%;
              border-collapse: collapse;
              background: white;
              box-shadow: 0 2px 10px rgba(15,23,42,.06);
              margin-bottom: 24px;
            }
            th, td {
              border: 1px solid #e5e7eb;
              padding: 9px 10px;
              text-align: left;
              vertical-align: top;
              font-size: 13px;
            }
            th {
              background: #1f2937;
              color: white;
            }
            tr.passed td:first-child { color: #15803d; font-weight: 700; }
            tr.failed td:first-child { color: #b91c1c; font-weight: 700; }
            tr.skipped td:first-child { color: #a16207; font-weight: 700; }
          </style>
        </head>
        <body>
          <div class="header">
            <h1>Sentinel API Assurance</h1>
            <div>Suite: {{Encode(run.SuiteName)}} | Environment: {{Encode(run.EnvironmentName)}}</div>
            <div>Started: {{run.StartedAt:yyyy-MM-dd HH:mm:ss}} | Finished: {{run.FinishedAt:yyyy-MM-dd HH:mm:ss}}</div>
          </div>

          <div class="cards">
            <div class="card"><b>Total</b><span>{{run.TotalCount}}</span></div>
            <div class="card"><b>Passed</b><span>{{run.PassedCount}}</span></div>
            <div class="card"><b>Failed</b><span>{{run.FailedCount}}</span></div>
            <div class="card"><b>Skipped</b><span>{{run.SkippedCount}}</span></div>
            <div class="card"><b>Success</b><span>%{{run.CoverageRate}}</span></div>
          </div>

          <h2>Service Summary</h2>
          <table>
            <thead>
              <tr>
                <th>Service</th>
                <th>Total</th>
                <th>Passed</th>
                <th>Failed</th>
                <th>Skipped</th>
              </tr>
            </thead>
            <tbody>
              {{serviceRows}}
            </tbody>
          </table>

          <h2>Case Details</h2>
          <table>
            <thead>
              <tr>
                <th>Status</th>
                <th>Service</th>
                <th>Operation</th>
                <th>Protocol</th>
                <th>HTTP</th>
                <th>Ms</th>
                <th>Request</th>
                <th>Message</th>
                <th>Assertions</th>
              </tr>
            </thead>
            <tbody>
              {{rows}}
            </tbody>
          </table>
        </body>
        </html>
        """;

        File.WriteAllText(path, html, Encoding.UTF8);
        return path;
    }

    private static string Encode(string value) => WebUtility.HtmlEncode(value);
}
