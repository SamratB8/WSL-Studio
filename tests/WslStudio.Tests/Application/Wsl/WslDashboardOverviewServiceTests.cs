using WslStudio.Application.Wsl;
using WslStudio.Core.Wsl;

namespace WslStudio.Tests.Application.Wsl;

public sealed class WslDashboardOverviewServiceTests
{
    [Fact]
    public async Task GetOverviewAsync_ComposesCountsAndEnvironmentInfo()
    {
        FakeDistributionDiscoveryService discoveryService = new(
            WslDistributionDiscoveryResult.Success([
                CreateDistribution("Ubuntu", WslDistributionState.Running, isDefault: true),
                CreateDistribution("Debian", WslDistributionState.Stopped, isDefault: false),
                CreateDistribution("Alpine", WslDistributionState.Stopped, isDefault: false)
            ]),
            WslEnvironmentResult.Success(new WslEnvironmentInfo(
                WslStatusAndVersionFixtures.StatusOutput,
                WslStatusAndVersionFixtures.VersionOutput,
                UserSafeMessage: null)));

        WslDashboardOverviewService service = new(
            discoveryService,
            new WslStatusParser(),
            new WslVersionParser());

        WslDashboardOverviewResult result = await service.GetOverviewAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Overview);
        Assert.Equal("Ubuntu", result.Overview.DefaultDistribution?.Value);
        Assert.Equal(3, result.Overview.TotalDistributionCount);
        Assert.Equal(1, result.Overview.RunningDistributionCount);
        Assert.Equal(2, result.Overview.StoppedDistributionCount);
        Assert.Equal("2.4.13.0", result.Overview.VersionInfo?.WslVersion);
        Assert.Equal(2, result.Overview.StatusInfo?.DefaultVersion);
    }

    [Fact]
    public async Task GetOverviewAsync_ReturnsFailure_WhenDistributionDiscoveryFails()
    {
        FakeDistributionDiscoveryService discoveryService = new(
            WslDistributionDiscoveryResult.Failure("WSL is not installed or wsl.exe could not be found."),
            WslEnvironmentResult.Failure("Not called"));

        WslDashboardOverviewService service = new(
            discoveryService,
            new WslStatusParser(),
            new WslVersionParser());

        WslDashboardOverviewResult result = await service.GetOverviewAsync(CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Null(result.Overview);
        Assert.Equal("WSL is not installed or wsl.exe could not be found.", result.UserSafeMessage);
    }

    [Fact]
    public async Task GetOverviewAsync_SucceedsWithoutEnvironmentInfo()
    {
        FakeDistributionDiscoveryService discoveryService = new(
            WslDistributionDiscoveryResult.Success([
                CreateDistribution("Ubuntu", WslDistributionState.Running, isDefault: true)
            ]),
            WslEnvironmentResult.Failure("WSL version is unavailable."));

        WslDashboardOverviewService service = new(
            discoveryService,
            new WslStatusParser(),
            new WslVersionParser());

        WslDashboardOverviewResult result = await service.GetOverviewAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Overview);
        Assert.Null(result.Overview.StatusInfo);
        Assert.Null(result.Overview.VersionInfo);
        Assert.Equal(1, result.Overview.TotalDistributionCount);
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
