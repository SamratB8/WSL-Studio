using WslStudio.Application.Wsl;
using WslStudio.Core.Wsl;

namespace WslStudio.Tests.Application.Wsl;

public sealed class WslDistributionDetailsServiceTests
{
    [Fact]
    public async Task GetDetailsAsync_ReturnsSelectedDistributionAndKernelVersion()
    {
        WslDistribution selectedDistribution = CreateDistribution(
            "Ubuntu Development",
            WslDistributionState.Running,
            version: 2,
            isDefault: true);
        FakeDistributionDiscoveryService discoveryService = new(
            WslDistributionDiscoveryResult.Success([
                CreateDistribution("Debian", WslDistributionState.Stopped, version: 2, isDefault: false),
                selectedDistribution
            ]),
            WslEnvironmentResult.Success(new WslEnvironmentInfo(
                WslStatusAndVersionFixtures.StatusOutput,
                WslStatusAndVersionFixtures.VersionOutput,
                UserSafeMessage: null)));
        WslDistributionDetailsService service = new(discoveryService, new WslVersionParser());

        WslDistributionDetailsResult result = await service.GetDetailsAsync(
            CreateName("ubuntu development"),
            CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.False(result.NotFound);
        Assert.NotNull(result.Details);
        Assert.Equal(selectedDistribution, result.Details.Distribution);
        Assert.Equal("6.18.35.2-1", result.Details.KernelVersion);
        Assert.Null(result.Details.Architecture);
        Assert.Null(result.Details.InstallationLocation);
    }

    [Fact]
    public async Task GetDetailsAsync_ReturnsMissing_WhenDistributionIsNoLongerInstalled()
    {
        FakeDistributionDiscoveryService discoveryService = new(
            WslDistributionDiscoveryResult.Success([]),
            WslEnvironmentResult.Failure("Environment information should not be requested."));
        WslDistributionDetailsService service = new(discoveryService, new WslVersionParser());

        WslDistributionDetailsResult result = await service.GetDetailsAsync(
            CreateName("Ubuntu"),
            CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.True(result.NotFound);
        Assert.Null(result.Details);
        Assert.Contains("no longer reported", result.UserSafeMessage);
        Assert.Equal(0, discoveryService.EnvironmentRequestCount);
    }

    [Fact]
    public async Task GetDetailsAsync_ReturnsFailure_WhenDiscoveryFails()
    {
        FakeDistributionDiscoveryService discoveryService = new(
            WslDistributionDiscoveryResult.Failure("WSL distribution discovery failed."),
            WslEnvironmentResult.Failure("Environment information should not be requested."));
        WslDistributionDetailsService service = new(discoveryService, new WslVersionParser());

        WslDistributionDetailsResult result = await service.GetDetailsAsync(
            CreateName("Ubuntu"),
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.False(result.NotFound);
        Assert.Null(result.Details);
        Assert.Equal("WSL distribution discovery failed.", result.UserSafeMessage);
        Assert.Equal(0, discoveryService.EnvironmentRequestCount);
    }

    [Fact]
    public async Task GetDetailsAsync_SucceedsWithoutOptionalEnvironmentInformation()
    {
        FakeDistributionDiscoveryService discoveryService = new(
            WslDistributionDiscoveryResult.Success([
                CreateDistribution("Ubuntu", WslDistributionState.Stopped, version: 2, isDefault: false)
            ]),
            WslEnvironmentResult.Failure("WSL version information is unavailable."));
        WslDistributionDetailsService service = new(discoveryService, new WslVersionParser());

        WslDistributionDetailsResult result = await service.GetDetailsAsync(
            CreateName("Ubuntu"),
            CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Details);
        Assert.Null(result.Details.KernelVersion);
    }

    private static DistributionName CreateName(string value)
    {
        bool created = DistributionName.TryCreate(value, out DistributionName? distributionName);
        Assert.True(created);
        return Assert.IsType<DistributionName>(distributionName);
    }

    private static WslDistribution CreateDistribution(
        string name,
        WslDistributionState state,
        int? version,
        bool isDefault)
    {
        return new WslDistribution(CreateName(name), state, version, isDefault);
    }

    private sealed class FakeDistributionDiscoveryService(
        WslDistributionDiscoveryResult distributionsResult,
        WslEnvironmentResult environmentResult) : IWslDistributionDiscoveryService
    {
        public int EnvironmentRequestCount { get; private set; }

        public Task<WslDistributionDiscoveryResult> GetDistributionsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(distributionsResult);
        }

        public Task<WslEnvironmentResult> GetEnvironmentInfoAsync(CancellationToken cancellationToken)
        {
            EnvironmentRequestCount++;
            return Task.FromResult(environmentResult);
        }
    }
}
