using System.Net;
using System.Text;

namespace SentinelApiAssurance.Tests;

internal sealed class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

    public TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage>? responseFactory = null)
    {
        _responseFactory = responseFactory ?? (_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("<response><status>OK</status></response>", Encoding.UTF8, "text/xml")
        });
    }

    public HttpMethod? Method { get; private set; }
    public Uri? RequestUri { get; private set; }
    public string? Content { get; private set; }
    public Dictionary<string, string> Headers { get; } = new(StringComparer.OrdinalIgnoreCase);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Method = request.Method;
        RequestUri = request.RequestUri;

        if (request.Content is not null)
            Content = await request.Content.ReadAsStringAsync(cancellationToken);

        foreach (var header in request.Headers)
            Headers[header.Key] = string.Join(",", header.Value);

        return _responseFactory(request);
    }
}
