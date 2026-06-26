using System.Text.RegularExpressions;
using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed partial class WslDistributionParser : IWslDistributionParser
{
    public WslDistributionParseResult ParseListVerbose(string output)
    {
        if (string.IsNullOrWhiteSpace(output))
        {
            return WslDistributionParseResult.Success([]);
        }

        List<WslDistribution> distributions = [];

        foreach (string rawLine in NormalizeOutput(output).Split('\n'))
        {
            string line = rawLine.TrimEnd('\r');

            if (string.IsNullOrWhiteSpace(line) || IsHeaderLine(line))
            {
                continue;
            }

            Match match = DistributionLineRegex().Match(line);
            if (!match.Success)
            {
                return WslDistributionParseResult.Failure("WSL returned distribution output in an unsupported format.");
            }

            string nameValue = match.Groups["name"].Value.Trim();
            if (!DistributionName.TryCreate(nameValue, out DistributionName? name) || name is null)
            {
                return WslDistributionParseResult.Failure("WSL returned an invalid distribution name.");
            }

            WslDistributionState state = ParseState(match.Groups["state"].Value);
            int? version = int.TryParse(match.Groups["version"].Value, out int parsedVersion)
                ? parsedVersion
                : null;

            distributions.Add(new WslDistribution(
                name,
                state,
                version,
                IsDefault: !string.IsNullOrWhiteSpace(match.Groups["default"].Value)));
        }

        return WslDistributionParseResult.Success(distributions);
    }

    private static bool IsHeaderLine(string line)
    {
        return line.TrimStart().StartsWith("NAME", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeOutput(string output)
    {
        return output.Replace("\0", string.Empty, StringComparison.Ordinal);
    }

    private static WslDistributionState ParseState(string value)
    {
        return value.Trim().ToUpperInvariant() switch
        {
            "RUNNING" => WslDistributionState.Running,
            "STOPPED" => WslDistributionState.Stopped,
            "INSTALLING" => WslDistributionState.Installing,
            "CONVERTING" => WslDistributionState.Converting,
            _ => WslDistributionState.Unknown
        };
    }

    [GeneratedRegex(@"^\s*(?<default>\*)?\s*(?<name>.+?)\s{2,}(?<state>\S+)\s{2,}(?<version>\d+)\s*$")]
    private static partial Regex DistributionLineRegex();
}
