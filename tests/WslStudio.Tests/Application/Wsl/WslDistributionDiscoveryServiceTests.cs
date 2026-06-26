using WslStudio.Application.Commands;
using WslStudio.Application.Wsl;

namespace WslStudio.Tests.Application.Wsl;

public sealed class WslDistributionDiscoveryServiceTests
{
    [Fact]
    public async Task GetDistributionsAsync_UsesReadOnlyListVerboseCommand()
    {
        FakeCommandRunner commandRunner = new(CommandResult.Success(
            "wsl.exe",
            ["--list", "--verbose"],
            WslListVerboseFixtures.NormalOutput,
            string.Empty,
            exitCode: 0,
            TimeSpan.FromMilliseconds(10)));

        WslDistributionDiscoveryService service = new(commandRunner, new WslDistributionParser());

        WslDistributionDiscoveryResult result = await service.GetDistributionsAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(["--list", "--verbose"], commandRunner.Requests.Single().Arguments);
        Assert.Equal("wsl.exe", commandRunner.Requests.Single().CommandName);
        Assert.Equal(2, result.Distributions.Count);
    }

    [Fact]
    public async Task GetDistributionsAsync_ReturnsUserSafeFailure_WhenCommandFails()
    {
        FakeCommandRunner commandRunner = new(CommandResult.Failure(
            "wsl.exe",
            ["--list", "--verbose"],
            "WSL is not installed or wsl.exe could not be found.",
            TimeSpan.FromMilliseconds(5)));

        WslDistributionDiscoveryService service = new(commandRunner, new WslDistributionParser());

        WslDistributionDiscoveryResult result = await service.GetDistributionsAsync(CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Empty(result.Distributions);
        Assert.Equal("WSL is not installed or wsl.exe could not be found.", result.UserSafeMessage);
    }

    private sealed class FakeCommandRunner(CommandResult result) : ICommandRunner
    {
        public List<CommandRequest> Requests { get; } = [];

        public Task<CommandResult> RunAsync(CommandRequest request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return Task.FromResult(result);
        }
    }
}
