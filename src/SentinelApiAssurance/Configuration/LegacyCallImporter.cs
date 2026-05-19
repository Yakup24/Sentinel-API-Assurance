using System.Xml.Linq;
using SentinelApiAssurance.Models;

namespace SentinelApiAssurance.Configuration;

public static class LegacyCallImporter
{
    public static TestSuite Import(string path, AppConfig config, EnvironmentConfig environment)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Legacy call XML not found.", path);

        var content = File.ReadAllText(path);
        var safeXml = content.TrimStart().StartsWith("<calls", StringComparison.OrdinalIgnoreCase)
            ? content
            : $"<calls>{content}</calls>";

        var doc = XDocument.Parse(safeXml);

        var cases = doc.Descendants("call")
            .Select(x => new TestCase
            {
                Id = $"{(string?)x.Attribute("service")}.{(string?)x.Attribute("operation")}",
                Title = $"{(string?)x.Attribute("service")} / {(string?)x.Attribute("operation")}",
                Active = string.Equals((string?)x.Attribute("active"), "true", StringComparison.OrdinalIgnoreCase),
                Protocol = "SOAP",
                Service = ((string?)x.Attribute("service") ?? "").Trim(),
                Operation = ((string?)x.Attribute("operation") ?? "").Trim(),
                RequestBodyFile = $"Requests/{((string?)x.Attribute("service") ?? "").Trim()}/{((string?)x.Attribute("operation") ?? "").Trim()}.xml",
                ExpectedHttpStatus = 200,
                Assertions = new List<AssertionDefinition>
                {
                    new() { Type = "NoSoapFault" },
                    new() { Type = "MaxDurationMs", MaxDurationMs = config.DefaultMaxDurationMs }
                }
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.Service) && !string.IsNullOrWhiteSpace(x.Operation))
            .DistinctBy(x => $"{x.Service}.{x.Operation}")
            .ToList();

        return new TestSuite
        {
            Name = $"Legacy XML Import - {Path.GetFileName(path)}",
            Description = "Test suite generated from legacy <call> XML inventory.",
            Owner = "QA / Integration Team",
            Cases = cases
        };
    }
}
