using WslStudio.Application.Wsl;
using WslStudio.Core.Wsl;

namespace WslStudio.Tests.Application.Wsl;

public sealed class WslEnvironmentServiceTests
{
    [Fact]
    public async Task GetEnvironmentAsync_ReturnsConsolidatedEnvironment_WhenWslDataIsAvailable()
    {
        WslEnvironmentService service = CreateService(
            WslDistributionDiscoveryResult.Success([
                Dist("Ubuntu", WslDistributionState.Running, isDefault: true),
                Dist("docker-desktop", WslDistributionState.Stopped)
            ]),
            Environment(WslStatusAndVersionFixtures.StatusOutput, WslStatusAndVersionFixtures.VersionOutput));

        WslEnvironmentOverviewResult result = await service.GetEnvironmentAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Environment);

        WslEnvironmentOverview environment = result.Environment;
        Assert.Equal(2, environment.Overview.TotalDistributionCount);
        Assert.Equal(1, environment.Overview.RunningDistributionCount);
        Assert.Equal("Ubuntu", environment.Overview.DefaultDistribution?.Value);
        Assert.Equal(2, environment.Overview.StatusInfo?.DefaultVersion);

        WslVersionInfo? version = environment.Overview.VersionInfo;
        Assert.NotNull(version);
        Assert.Equal("2.9.3.0", version.WslVersion);
        Assert.Equal("6.18.35.2-1", version.KernelVersion);
        Assert.Equal("1.0.79", version.WslgVersion);
        Assert.Equal("10.0.26300.8758", version.WindowsVersion);
        Assert.Equal("1.611.1-81528511", version.Direct3DVersion);
        Assert.Equal("10.0.26100.1-240331-1435.ge-release", version.DxCoreVersion);
        Assert.Equal("1.2.7214", version.MsrdcVersion);

        Assert.True(environment.DockerDesktopDetected);
        Assert.True(environment.WslgAvailable);
    }

    [Fact]
    public async Task GetEnvironmentAsync_ReturnsFailure_WhenDistributionDiscoveryFails()
    {
        WslEnvironmentService service = CreateService(
            WslDistributionDiscoveryResult.Failure("WSL is not installed or wsl.exe could not be found."),
            WslEnvironmentResult.Failure("Environment information should not be requested."));

        WslEnvironmentOverviewResult result = await service.GetEnvironmentAsync(CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Null(result.Environment);
        Assert.Equal("WSL is not installed or wsl.exe could not be found.", result.UserSafeMessage);
    }

    [Fact]
    public async Task GetEnvironmentAsync_OmitsOptionalInfo_WhenEnvironmentInformationIsUnavailable()
    {
        WslEnvironmentService service = CreateService(
            WslDistributionDiscoveryResult.Success([
                Dist("Ubuntu", WslDistributionState.Running, isDefault: true)
            ]),
            WslEnvironmentResult.Failure("WSL version information is unavailable."));

        WslEnvironmentOverviewResult result = await service.GetEnvironmentAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Environment);
        Assert.Null(result.Environment.Overview.VersionInfo);
        Assert.Null(result.Environment.Overview.StatusInfo);
        Assert.False(result.Environment.WslgAvailable);
        Assert.Equal(1, result.Environment.Overview.TotalDistributionCount);
    }

    [Fact]
    public async Task GetEnvironmentAsync_ReportsGraphicsUnavailable_WhenVersionOutputOmitsGraphics()
    {
        WslEnvironmentService service = CreateService(
            WslDistributionDiscoveryResult.Success([
                Dist("Ubuntu", WslDistributionState.Running, isDefault: true)
            ]),
            Environment(WslStatusAndVersionFixtures.StatusOutput, WslStatusAndVersionFixtures.VersionOutputWithoutGraphics));

        WslEnvironmentOverviewResult result = await service.GetEnvironmentAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        WslVersionInfo? version = result.Environment?.Overview.VersionInfo;
        Assert.NotNull(version);
        Assert.Equal("2.9.3.0", version.WslVersion);
        Assert.Null(version.Direct3DVersion);
        Assert.Null(version.DxCoreVersion);
        Assert.Null(version.MsrdcVersion);
        Assert.True(result.Environment!.WslgAvailable);
    }

    [Fact]
    public async Task GetEnvironmentAsync_DoesNotDetectDocker_WhenDockerDistributionIsAbsent()
    {
        WslEnvironmentService service = CreateService(
            WslDistributionDiscoveryResult.Success([
                Dist("Ubuntu", WslDistributionState.Running, isDefault: true),
                Dist("Debian", WslDistributionState.Stopped)
            ]),
            Environment(WslStatusAndVersionFixtures.StatusOutput, WslStatusAndVersionFixtures.VersionOutput));

        WslEnvironmentOverviewResult result = await service.GetEnvironmentAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.False(result.Environment!.DockerDesktopDetected);
    }

    [Fact]
    public async Task GetEnvironmentAsync_DetectsDocker_RegardlessOfCasing()
    {
        WslEnvironmentService service = CreateService(
            WslDistributionDiscoveryResult.Success([
                Dist("Ubuntu", WslDistributionState.Running, isDefault: true),
                Dist("Docker-Desktop", WslDistributionState.Stopped)
            ]),
            Environment(WslStatusAndVersionFixtures.StatusOutput, WslStatusAndVersionFixtures.VersionOutput));

        WslEnvironmentOverviewResult result = await service.GetEnvironmentAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.True(result.Environment!.DockerDesktopDetected);
    }

    [Fact]
    public async Task GetEnvironmentAsync_ReportsWslgUnavailable_WhenWslgComponentIsMissing()
    {
        const string versionWithoutWslg = """
        WSL version: 2.9.3.0
        Kernel version: 6.18.35.2-1
        Windows version: 10.0.26300.8758
        """;

        WslEnvironmentService service = CreateService(
            WslDistributionDiscoveryResult.Success([
                Dist("Ubuntu", WslDistributionState.Running, isDefault: true)
            ]),
            Environment(WslStatusAndVersionFixtures.StatusOutput, versionWithoutWslg));

        WslEnvironmentOverviewResult result = await service.GetEnvironmentAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.False(result.Environment!.WslgAvailable);
    }

    private static WslEnvironmentService CreateService(
        WslDistributionDiscoveryResult distributionsResult,
        WslEnvironmentResult environmentResult)
    {
        FakeDistributionDiscoveryService discoveryService = new(distributionsResult, environmentResult);
        WslDashboardOverviewService dashboardOverviewService = new(
            discoveryService,
            new WslStatusParser(),
            new WslVersionParser());

        return new WslEnvironmentService(dashboardOverviewService);
    }

    private static WslEnvironmentResult Environment(string statusOutput, string versionOutput)
    {
        return WslEnvironmentResult.Success(new WslEnvironmentInfo(statusOutput, versionOutput, UserSafeMessage: null));
    }

    private static WslDistribution Dist(string name, WslDistributionState state, bool isDefault = false)
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
