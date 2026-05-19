using System.Diagnostics;
using System.Text;
using SentinelApiAssurance.Models;
using SentinelApiAssurance.Utilities;

namespace SentinelApiAssurance.Services;

public sealed class RestTestExecutor : ITestExecutor
{
    private readonly HttpClient _httpClient;
    private readonly FileLogger _logger;

    public RestTestExecutor(HttpClient httpClient, FileLogger logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public bool CanExecute(TestCase testCase)
        => string.Equals(testCase.Protocol, "REST", StringComparison.OrdinalIgnoreCase);

    public async Task<RawTestResponse> ExecuteAsync(EnvironmentConfig environment, ServiceConfig service, TestCase testCase, AppConfig config)
    {
        var endpoint = service.BuildUrl(environment.BaseUrl);
        var url = string.IsNullOrWhiteSpace(testCase.Path)
            ? endpoint
            : $"{endpoint.TrimEnd('/')}/{testCase.Path.TrimStart('/')}";

        Exception? lastException = null;

        for (var attempt = 0; attempt <= config.RetryCount; attempt++)
        {
            try
            {
                var sw = Stopwatch.StartNew();

                using var request = new HttpRequestMessage(new HttpMethod(testCase.HttpMethod), url);

                if (!string.IsNullOrWhiteSpace(testCase.RequestBodyFile))
                {
                    var bodyPath = Path.Combine(AppContext.BaseDirectory, testCase.RequestBodyFile);
                    if (File.Exists(bodyPath))
                    {
                        var body = TemplateRenderer.Render(await File.ReadAllTextAsync(bodyPath), config.TestData);
                        request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                    }
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
                _logger.Warn($"REST attempt failed. Test={testCase.Id}, Attempt={attempt + 1}, Error={ex.Message}");

                if (attempt < config.RetryCount)
                    await Task.Delay(config.RetryDelayMs);
            }
        }

        return new RawTestResponse
        {
            Message = lastException?.Message ?? "Unknown REST execution error."
        };
    }

    private static void AddHeaders(HttpRequestMessage request, Dictionary<string, string> headers)
    {
        foreach (var header in headers)
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
    }
}
