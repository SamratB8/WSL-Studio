using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed class WslDashboardOverviewService(
    IWslDistributionDiscoveryService distributionDiscoveryService,
    IWslStatusParser statusParser,
    IWslVersionParser versionParser) : IWslDashboardOverviewService
{
    public async Task<WslDashboardOverviewResult> GetOverviewAsync(CancellationToken cancellationToken)
    {
        WslDistributionDiscoveryResult distributionsResult =
            await distributionDiscoveryService.GetDistributionsAsync(cancellationToken);

        if (!distributionsResult.Succeeded)
        {
            return WslDashboardOverviewResult.Failure(distributionsResult.UserSafeMessage);
        }

        WslEnvironmentResult environmentResult =
            await distributionDiscoveryService.GetEnvironmentInfoAsync(cancellationToken);

        WslStatusInfo? statusInfo = null;
        WslVersionInfo? versionInfo = null;

        if (environmentResult.Succeeded && environmentResult.EnvironmentInfo is not null)
        {
            WslStatusParseResult statusParseResult =
                statusParser.Parse(environmentResult.EnvironmentInfo.StatusOutput ?? string.Empty);
            WslVersionParseResult versionParseResult =
                versionParser.Parse(environmentResult.EnvironmentInfo.VersionOutput ?? string.Empty);

            statusInfo = statusParseResult.Succeeded ? statusParseResult.StatusInfo : null;
            versionInfo = versionParseResult.Succeeded ? versionParseResult.VersionInfo : null;
        }

        IReadOnlyList<WslDistribution> distributions = distributionsResult.Distributions;
        DistributionName? defaultDistribution =
            distributions.FirstOrDefault(distribution => distribution.IsDefault)?.Name
            ?? statusInfo?.DefaultDistribution;

        WslDashboardOverview overview = new(
            defaultDistribution,
            RunningDistributionCount: distributions.Count(distribution => distribution.State == WslDistributionState.Running),
            StoppedDistributionCount: distributions.Count(distribution => distribution.State == WslDistributionState.Stopped),
            TotalDistributionCount: distributions.Count,
            statusInfo,
            versionInfo,
            distributions);

        return WslDashboardOverviewResult.Success(overview);
    }
}
