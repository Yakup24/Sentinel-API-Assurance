namespace SentinelApiAssurance.Models;

public sealed class TestSuite
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Owner { get; set; } = "";
    public List<TestCase> Cases { get; set; } = new();
}

public sealed class TestCase
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public bool Active { get; set; } = true;
    public string Protocol { get; set; } = "SOAP"; // SOAP veya REST
    public string Service { get; set; } = "";
    public string Operation { get; set; } = "";
    public string HttpMethod { get; set; } = "POST";
    public string? Path { get; set; }
    public string? RequestBodyFile { get; set; }
    public int ExpectedHttpStatus { get; set; } = 200;
    public bool AllowStateChangingOperation { get; set; } = false;
    public Dictionary<string, string> Headers { get; set; } = new();
    public List<AssertionDefinition> Assertions { get; set; } = new();
}

public sealed class AssertionDefinition
{
    public string Type { get; set; } = "";
    public string? Value { get; set; }
    public string? ElementName { get; set; }
    public string? Expected { get; set; }
    public int? MaxDurationMs { get; set; }
}
