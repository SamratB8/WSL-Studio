using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed class WslVersionParser : IWslVersionParser
{
    public WslVersionParseResult Parse(string output)
    {
        if (string.IsNullOrWhiteSpace(output))
        {
            return WslVersionParseResult.Success(new WslVersionInfo(null, null, null, null));
        }

        Dictionary<string, string> values = WslKeyValueParser.Parse(output);

        values.TryGetValue("WSL version", out string? wslVersion);
        values.TryGetValue("Kernel version", out string? kernelVersion);
        values.TryGetValue("WSLg version", out string? wslgVersion);
        values.TryGetValue("Windows version", out string? windowsVersion);

        return WslVersionParseResult.Success(new WslVersionInfo(
            wslVersion,
            kernelVersion,
            wslgVersion,
            windowsVersion));
    }
}
