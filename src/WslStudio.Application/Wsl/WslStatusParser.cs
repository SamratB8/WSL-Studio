using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed class WslStatusParser : IWslStatusParser
{
    public WslStatusParseResult Parse(string output)
    {
        if (string.IsNullOrWhiteSpace(output))
        {
            return WslStatusParseResult.Success(new WslStatusInfo(null, null, null));
        }

        Dictionary<string, string> values = WslKeyValueParser.Parse(output);

        DistributionName? defaultDistribution = null;
        if (values.TryGetValue("Default Distribution", out string? defaultDistributionValue) &&
            !DistributionName.TryCreate(defaultDistributionValue, out defaultDistribution))
        {
            return WslStatusParseResult.Failure("WSL returned an invalid default distribution name.");
        }

        int? defaultVersion = null;
        if (values.TryGetValue("Default Version", out string? defaultVersionValue) &&
            int.TryParse(defaultVersionValue, out int parsedDefaultVersion))
        {
            defaultVersion = parsedDefaultVersion;
        }

        values.TryGetValue("Kernel version", out string? kernelVersion);

        return WslStatusParseResult.Success(new WslStatusInfo(
            defaultDistribution,
            defaultVersion,
            kernelVersion));
    }
}
