namespace SentinelApiAssurance.Utilities;

public sealed class CliOptions
{
    public string? EnvironmentName { get; init; }
    public string? SuitePath { get; init; }
    public string? ConfigPath { get; init; }
    public string? LegacyCallsPath { get; init; }
    public bool DryRun { get; init; }

    public static CliOptions Parse(string[] args)
    {
        string? GetValue(string name)
        {
            var index = Array.FindIndex(args, x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase));
            if (index < 0 || index + 1 >= args.Length)
                return null;

            return args[index + 1];
        }

        return new CliOptions
        {
            EnvironmentName = GetValue("--env"),
            SuitePath = GetValue("--suite"),
            ConfigPath = GetValue("--config"),
            LegacyCallsPath = GetValue("--calls"),
            DryRun = args.Any(x => string.Equals(x, "--dry-run", StringComparison.OrdinalIgnoreCase))
        };
    }
}
