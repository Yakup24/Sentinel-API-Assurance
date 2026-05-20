using SentinelApiAssurance.Utilities;

namespace SentinelApiAssurance.Tests;

public sealed class TemplateRendererTests
{
    [Fact]
    public void Render_Replaces_Config_Tokens()
    {
        var rendered = TemplateRenderer.Render(
            "<customer>{{CustomerId}}</customer>",
            new Dictionary<string, string> { ["CustomerId"] = "CUST-001" });

        Assert.Equal("<customer>CUST-001</customer>", rendered);
    }

    [Fact]
    public void Render_Replaces_Environment_Variable_Tokens()
    {
        const string variableName = "SENTINEL_TEST_TOKEN";
        Environment.SetEnvironmentVariable(variableName, "demo-token");

        try
        {
            var rendered = TemplateRenderer.Render("<token>{{ENV:SENTINEL_TEST_TOKEN}}</token>", new Dictionary<string, string>());

            Assert.Equal("<token>demo-token</token>", rendered);
        }
        finally
        {
            Environment.SetEnvironmentVariable(variableName, null);
        }
    }

    [Fact]
    public void Render_Leaves_Unknown_Tokens_Intact()
    {
        var rendered = TemplateRenderer.Render("<value>{{Missing}}</value>", new Dictionary<string, string>());

        Assert.Equal("<value>{{Missing}}</value>", rendered);
    }
}
