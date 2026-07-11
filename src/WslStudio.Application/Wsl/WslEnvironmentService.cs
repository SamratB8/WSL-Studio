namespace WslStudio.Application.Wsl;

/// <summary>
/// Builds the consolidated WSL environment overview by reusing the existing dashboard overview
/// service. The dashboard service already runs the official read-only WSL commands
/// (<c>wsl --list --verbose</c>, <c>wsl --status</c>, <c>wsl --version</c>) once each and parses
/// their output, so this service performs no additional WSL queries — it only derives the two
/// environment-specific facts (Docker Desktop presence and WSLg availability) from data already
/// gathered.
/// </summary>
public sealed class WslEnvironmentService(IWslDashboardOverviewService dashboardOverviewService)
    : IWslEnvironmentService
{
    private const string DockerDesktopDistributionName = "docker-desktop";

    public async Task<WslEnvironmentOverviewResult> GetEnvironmentAsync(CancellationToken cancellationToken)
    {
        WslDashboardOverviewResult overviewResult =
            await dashboardOverviewService.GetOverviewAsync(cancellationToken);

        if (!overviewResult.Succeeded || overviewResult.Overview is null)
        {
            return WslEnvironmentOverviewResult.Failure(overviewResult.UserSafeMessage);
        }

        WslDashboardOverview overview = overviewResult.Overview;

        bool dockerDesktopDetected = overview.Distributions.Any(distribution =>
            string.Equals(distribution.Name.Value, DockerDesktopDistributionName, StringComparison.OrdinalIgnoreCase));

        bool wslgAvailable = !string.IsNullOrWhiteSpace(overview.VersionInfo?.WslgVersion);

        return WslEnvironmentOverviewResult.Success(
            new WslEnvironmentOverview(overview, dockerDesktopDetected, wslgAvailable));
    }
}
