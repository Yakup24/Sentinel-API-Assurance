using System.Net;
using System.Text;
using SentinelApiAssurance.Models;
using SentinelApiAssurance.Services;
using SentinelApiAssurance.Utilities;

namespace SentinelApiAssurance.Tests;

public sealed class ExecutorTests
{
    [Fact]
    public async Task SoapExecutor_Builds_Envelope_Headers_And_SoapAction()
    {
        using var temp = new TemporaryDirectory();
        var requestFile = Path.Combine(temp.Path, "request.xml");
        await File.WriteAllTextAsync(requestFile, "<ser:GetCustomer><id>{{CustomerId}}</id></ser:GetCustomer>");

        var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("<response><status>OK</status></response>", Encoding.UTF8, "text/xml")
        });
        var executor = new SoapTestExecutor(new HttpClient(handler), new FileLogger(Path.Combine(temp.Path, "logs")));

        var response = await executor.ExecuteAsync(
            new EnvironmentConfig { BaseUrl = "https://api.example.test/services" },
            new ServiceConfig { Endpoint = "CustomerService", SoapVersion = "1.1", SoapActionFormat = "{service}/{operation}" },
            new TestCase
            {
                Id = "SOAP-001",
                Service = "CustomerService",
                Operation = "GetCustomer",
                RequestBodyFile = requestFile
            },
            new AppConfig
            {
                TestData = { ["CustomerId"] = "CUST-001" },
                GlobalHeaders = { ["X-Correlation-Id"] = "{{CustomerId}}" }
            });

        Assert.Equal(200, response.HttpStatus);
        Assert.Equal(HttpMethod.Post, handler.Method);
        Assert.Equal("https://api.example.test/services/CustomerService", handler.RequestUri!.ToString());
        Assert.Contains("<soapenv:Envelope", handler.Content);
        Assert.Contains("<id>CUST-001</id>", handler.Content);
        Assert.Equal("CustomerService/GetCustomer", handler.Headers["SOAPAction"]);
        Assert.Equal("CUST-001", handler.Headers["X-Correlation-Id"]);
    }

    [Fact]
    public async Task RestExecutor_Renders_Path_Body_And_Uses_Configured_Method()
    {
        using var temp = new TemporaryDirectory();
        var requestFile = Path.Combine(temp.Path, "request.json");
        await File.WriteAllTextAsync(requestFile, """{"customerId":"{{CustomerId}}"}""");

        var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"status":"ok"}""", Encoding.UTF8, "application/json")
        });
        var executor = new RestTestExecutor(new HttpClient(handler), new FileLogger(Path.Combine(temp.Path, "logs")));

        var response = await executor.ExecuteAsync(
            new EnvironmentConfig { BaseUrl = "https://api.example.test" },
            new ServiceConfig { Endpoint = "customer-api" },
            new TestCase
            {
                Id = "REST-001",
                Protocol = "REST",
                HttpMethod = "PUT",
                Path = "/customers/{{CustomerId}}",
                RequestBodyFile = requestFile
            },
            new AppConfig { TestData = { ["CustomerId"] = "CUST-001" } });

        Assert.Equal(200, response.HttpStatus);
        Assert.Equal(HttpMethod.Put, handler.Method);
        Assert.Equal("https://api.example.test/customer-api/customers/CUST-001", handler.RequestUri!.ToString());
        Assert.Equal("""{"customerId":"CUST-001"}""", handler.Content);
    }
}
