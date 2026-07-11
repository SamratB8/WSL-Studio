using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed class WslDistributionDetailsService(
    IWslDistributionDiscoveryService distributionDiscoveryService,
    IWslVersionParser versionParser) : IWslDistributionDetailsService
{
    public async Task<WslDistributionDetailsResult> GetDetailsAsync(
        DistributionName distributionName,
        CancellationToken cancellationToken)
    {
        WslDistributionDiscoveryResult discoveryResult =
            await distributionDiscoveryService.GetDistributionsAsync(cancellationToken);

        if (!discoveryResult.Succeeded)
        {
            return WslDistributionDetailsResult.Failure(discoveryResult.UserSafeMessage);
        }

        WslDistribution? distribution = discoveryResult.Distributions.FirstOrDefault(candidate =>
            string.Equals(candidate.Name.Value, distributionName.Value, StringComparison.OrdinalIgnoreCase));

        if (distribution is null)
        {
            return WslDistributionDetailsResult.Missing(
                $"The distribution '{distributionName.Value}' is no longer reported by WSL.");
        }

        string? kernelVersion = await GetKernelVersionAsync(cancellationToken);

        return WslDistributionDetailsResult.Success(new WslDistributionDetails(
            distribution,
            Architecture: null,
            InstallationLocation: null,
            kernelVersion));
    }

    private async Task<string?> GetKernelVersionAsync(CancellationToken cancellationToken)
    {
        WslEnvironmentResult environmentResult =
            await distributionDiscoveryService.GetEnvironmentInfoAsync(cancellationToken);

        if (!environmentResult.Succeeded || environmentResult.EnvironmentInfo is null)
        {
            return null;
        }

        WslVersionParseResult parseResult = versionParser.Parse(environmentResult.EnvironmentInfo.VersionOutput ?? string.Empty);
        return parseResult.Succeeded ? parseResult.VersionInfo?.KernelVersion : null;
    }
}
