using SentinelApiAssurance.Models;

namespace SentinelApiAssurance.Safety;

public sealed class OperationSafetyPolicy
{
    private readonly AppConfig _config;
    private static readonly string[] ReadOnlyPrefixes =
    {
        "get", "is", "read", "check", "search", "list", "load", "query", "find", "compare"
    };

    public OperationSafetyPolicy(AppConfig config)
    {
        _config = config;
    }

    public bool ShouldBlock(TestCase testCase, out string reason)
    {
        reason = "";

        if (!_config.BlockDangerousOperationsWithoutExplicitApproval)
            return false;

        var operation = testCase.Operation ?? "";
        if (ReadOnlyPrefixes.Any(prefix => operation.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
            return false;

        var isDangerous = _config.DangerousOperationKeywords.Any(keyword =>
            operation.Contains(keyword, StringComparison.OrdinalIgnoreCase));

        if (!isDangerous)
            return false;

        if (testCase.AllowStateChangingOperation)
            return false;

        reason = "State-changing operation blocked. Set AllowStateChangingOperation=true only with approved test data.";
        return true;
    }
}
