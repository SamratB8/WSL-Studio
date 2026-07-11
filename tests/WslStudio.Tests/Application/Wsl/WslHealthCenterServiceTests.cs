using WslStudio.Application.Wsl;
using WslStudio.Core.Wsl;

namespace WslStudio.Tests.Application.Wsl;

public sealed class WslHealthCenterServiceTests
{
    [Fact]
    public async Task GetHealthAsync_ReportsHealthyCoreChecks_WhenWslDataIsAvailable()
    {
        FakeDistributionDiscoveryService discoveryService = new(
            WslDistributionDiscoveryResult.Success([
                CreateDistribution("Ubuntu", WslDistributionState.Running, isDefault: true),
                CreateDistribution("docker-desktop", WslDistributionState.Stopped, isDefault: false)
            ]),
            WslEnvironmentResult.Success(new WslEnvironmentInfo(
                WslStatusAndVersionFixtures.StatusOutput,
                WslStatusAndVersionFixtures.VersionOutput,
                UserSafeMessage: null)));

        WslHealthCenterService service = CreateService(discoveryService);

        WslHealthCenterResult result = await service.GetHealthAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Contains(result.Checks, check =>
            check.Name == "WSL executable available" &&
            check.Status == WslHealthCheckStatus.Healthy);
        Assert.Contains(result.Checks, check =>
            check.Name == "At least one distribution installed" &&
            check.Status == WslHealthCheckStatus.Healthy);
        Assert.Contains(result.Checks, check =>
            check.Name == "Docker Desktop WSL distribution" &&
            check.Status == WslHealthCheckStatus.Healthy);
        Assert.True(result.Summary.HealthyCount > 0);
    }

    [Fact]
    public async Task GetHealthAsync_ReportsWarnings_WhenNoDistributionIsInstalled()
    {
        FakeDistributionDiscoveryService discoveryService = new(
            WslDistributionDiscoveryResult.Success([]),
            WslEnvironmentResult.Success(new WslEnvironmentInfo(
                WslStatusAndVersionFixtures.StatusOutput,
                WslStatusAndVersionFixtures.VersionOutput,
                UserSafeMessage: null)));

        WslHealthCenterService service = CreateService(discoveryService);

        WslHealthCenterResult result = await service.GetHealthAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Contains(result.Checks, check =>
            check.Name == "At least one distribution installed" &&
            check.Status == WslHealthCheckStatus.Warning);
        Assert.Contains(result.Checks, check =>
            check.Name == "Default distribution configured" &&
            check.Status == WslHealthCheckStatus.Warning);
        Assert.Equal("Review recommended", result.Summary.OverallStatus);
    }

    [Fact]
    public async Task GetHealthAsync_ReportsErrorChecks_WhenDistributionDiscoveryFails()
    {
        FakeDistributionDiscoveryService discoveryService = new(
            WslDistributionDiscoveryResult.Failure("WSL is not installed or wsl.exe could not be found."),
            WslEnvironmentResult.Failure("Not called"));

        WslHealthCenterService service = CreateService(discoveryService);

        WslHealthCenterResult result = await service.GetHealthAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Contains(result.Checks, check =>
            check.Name == "WSL executable available" &&
            check.Status == WslHealthCheckStatus.Error);
        Assert.Contains(result.Checks, check =>
            check.Name == "No command failures during discovery" &&
            check.Status == WslHealthCheckStatus.Error);
        Assert.Equal("Attention needed", result.Summary.OverallStatus);
    }

    [Fact]
    public async Task GetHealthAsync_ReportsUnknownEnvironmentChecks_WhenStatusAndVersionAreUnavailable()
    {
        FakeDistributionDiscoveryService discoveryService = new(
            WslDistributionDiscoveryResult.Success([
                CreateDistribution("Ubuntu", WslDistributionState.Running, isDefault: true)
            ]),
            WslEnvironmentResult.Failure("WSL version is unavailable."));

        WslHealthCenterService service = CreateService(discoveryService);

        WslHealthCenterResult result = await service.GetHealthAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Contains(result.Checks, check =>
            check.Name == "WSL version available" &&
            check.Status == WslHealthCheckStatus.Unknown);
        Assert.Contains(result.Checks, check =>
            check.Name == "Virtualization availability" &&
            check.Status == WslHealthCheckStatus.Unknown);
    }

    [Fact]
    public async Task GetHealthAsync_UsesReadOnlyCompleteSummary_WhenOnlyUnknownChecksRemain()
    {
        FakeDistributionDiscoveryService discoveryService = new(
            WslDistributionDiscoveryResult.Success([
                CreateDistribution("Ubuntu", WslDistributionState.Running, isDefault: true)
            ]),
            WslEnvironmentResult.Success(new WslEnvironmentInfo(
                WslStatusAndVersionFixtures.StatusOutput,
                WslStatusAndVersionFixtures.VersionOutput,
                UserSafeMessage: null)));

        WslHealthCenterService service = CreateService(discoveryService);

        WslHealthCenterResult result = await service.GetHealthAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(0, result.Summary.ErrorCount);
        Assert.Equal(0, result.Summary.WarningCount);
        Assert.True(result.Summary.UnknownCount > 0);
        Assert.Equal("Read-only health check complete", result.Summary.OverallStatus);
        Assert.Contains("not implemented yet", result.Summary.Recommendation, StringComparison.OrdinalIgnoreCase);
    }

    private static WslHealthCenterService CreateService(IWslDistributionDiscoveryService discoveryService)
    {
        return new WslHealthCenterService(
            discoveryService,
            new WslStatusParser(),
            new WslVersionParser());
    }

    private static WslDistribution CreateDistribution(
        string name,
        WslDistributionState state,
        bool isDefault)
    {
        bool created = DistributionName.TryCreate(name, out DistributionName? distributionName);
        Assert.True(created);
        Assert.NotNull(distributionName);

        return new WslDistribution(distributionName, state, Version: 2, isDefault);
    }

    private sealed class FakeDistributionDiscoveryService(
        WslDistributionDiscoveryResult distributionsResult,
        WslEnvironmentResult environmentResult) : IWslDistributionDiscoveryService
    {
        public Task<WslDistributionDiscoveryResult> GetDistributionsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(distributionsResult);
        }

        public Task<WslEnvironmentResult> GetEnvironmentInfoAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(environmentResult);
        }
    }
}
