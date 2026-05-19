using System.Text.RegularExpressions;

namespace SentinelApiAssurance.Utilities;

public static partial class TemplateRenderer
{
    public static string Render(string template, Dictionary<string, string> data)
    {
        return TokenPattern().Replace(template, match =>
        {
            var token = match.Groups["token"].Value.Trim();

            if (token.StartsWith("ENV:", StringComparison.OrdinalIgnoreCase))
            {
                var environmentVariable = token[4..].Trim();
                return Environment.GetEnvironmentVariable(environmentVariable) ?? match.Value;
            }

            return data.TryGetValue(token, out var value) ? value : match.Value;
        });
    }

    public static Dictionary<string, string> RenderHeaders(Dictionary<string, string> headers, Dictionary<string, string> data)
    {
        return headers.ToDictionary(x => x.Key, x => Render(x.Value, data));
    }

    [GeneratedRegex(@"\{\{(?<token>[^}]+)\}\}", RegexOptions.Compiled)]
    private static partial Regex TokenPattern();
}
