using System.Diagnostics;
using System.Text;
using SentinelApiAssurance.Models;
using SentinelApiAssurance.Utilities;

namespace SentinelApiAssurance.Services;

public sealed class SoapTestExecutor : ITestExecutor
{
    private readonly HttpClient _httpClient;
    private readonly FileLogger _logger;

    public SoapTestExecutor(HttpClient httpClient, FileLogger logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public bool CanExecute(TestCase testCase)
        => string.Equals(testCase.Protocol, "SOAP", StringComparison.OrdinalIgnoreCase);

    public async Task<RawTestResponse> ExecuteAsync(EnvironmentConfig environment, ServiceConfig service, TestCase testCase, AppConfig config)
    {
        var requestFile = ResolvePath(testCase.RequestBodyFile);
        if (requestFile is null || !File.Exists(requestFile))
        {
            return new RawTestResponse
            {
                Message = $"SOAP request body file not found: {testCase.RequestBodyFile}"
            };
        }

        var body = TemplateRenderer.Render(await File.ReadAllTextAsync(requestFile), config.TestData);
        var envelope = BuildEnvelope(body, service.SoapVersion);
        var endpoint = service.BuildUrl(environment.BaseUrl);

        Exception? lastException = null;

        for (var attempt = 0; attempt <= config.RetryCount; attempt++)
        {
            try
            {
                var sw = Stopwatch.StartNew();

                using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Content = new StringContent(
                    envelope,
                    Encoding.UTF8,
                    service.SoapVersion == "1.2" ? "application/soap+xml" : "text/xml"
                );

                if (service.SoapVersion == "1.1")
                {
                    var soapAction = service.SoapActionFormat
                        .Replace("{service}", testCase.Service)
                        .Replace("{operation}", testCase.Operation);

                    request.Headers.TryAddWithoutValidation("SOAPAction", soapAction);
                }

                AddHeaders(request, TemplateRenderer.RenderHeaders(config.GlobalHeaders, config.TestData));
                AddHeaders(request, TemplateRenderer.RenderHeaders(environment.Headers, config.TestData));
                AddHeaders(request, TemplateRenderer.RenderHeaders(service.Headers, config.TestData));
                AddHeaders(request, TemplateRenderer.RenderHeaders(testCase.Headers, config.TestData));

                using var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                sw.Stop();

                return new RawTestResponse
                {
                    HttpStatus = (int)response.StatusCode,
                    Body = responseBody,
                    DurationMs = sw.ElapsedMilliseconds,
                    Message = response.ReasonPhrase ?? ""
                };
            }
            catch (Exception ex)
            {
                lastException = ex;
                _logger.Warn($"SOAP attempt failed. Test={testCase.Id}, Attempt={attempt + 1}, Error={ex.Message}");

                if (attempt < config.RetryCount)
                    await Task.Delay(config.RetryDelayMs);
            }
        }

        return new RawTestResponse
        {
            Message = lastException?.Message ?? "Unknown SOAP execution error."
        };
    }

    private static string? ResolvePath(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return null;

        if (Path.IsPathRooted(relativePath))
            return relativePath;

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }

    private static void AddHeaders(HttpRequestMessage request, Dictionary<string, string> headers)
    {
        foreach (var header in headers)
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
    }

    private static string BuildEnvelope(string body, string soapVersion)
    {
        var ns = soapVersion == "1.2"
            ? "http://www.w3.org/2003/05/soap-envelope"
            : "http://schemas.xmlsoap.org/soap/envelope/";

        return $"""
        <?xml version="1.0" encoding="utf-8"?>
        <soapenv:Envelope xmlns:soapenv="{ns}">
          <soapenv:Header/>
          <soapenv:Body>
            {body}
          </soapenv:Body>
        </soapenv:Envelope>
        """;
    }
}
