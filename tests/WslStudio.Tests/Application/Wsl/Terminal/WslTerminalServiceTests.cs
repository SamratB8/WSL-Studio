using WslStudio.Application.Wsl.Terminal;
using WslStudio.Core.Wsl;

namespace WslStudio.Tests.Application.Wsl.Terminal;

public sealed class WslTerminalServiceTests
{
    private const string WindowsTerminal = "wt.exe";
    private const string Wsl = "wsl.exe";

    [Fact]
    public async Task LaunchDefaultAsync_PrefersWindowsTerminal_WhenAvailable()
    {
        FakeTerminalProcessLauncher launcher = new(WindowsTerminal, Wsl);
        WslTerminalService service = new(launcher);

        WslTerminalLaunchResult result = await service.LaunchDefaultAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(TerminalLaunchMethod.WindowsTerminal, result.Method);
        (string fileName, IReadOnlyList<string> arguments) = Assert.Single(launcher.Calls);
        Assert.Equal(WindowsTerminal, fileName);
        Assert.Equal([Wsl], arguments);
    }

    [Fact]
    public async Task LaunchDefaultAsync_FallsBackToWsl_WhenWindowsTerminalUnavailable()
    {
        FakeTerminalProcessLauncher launcher = new(Wsl);
        WslTerminalService service = new(launcher);

        WslTerminalLaunchResult result = await service.LaunchDefaultAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(TerminalLaunchMethod.Wsl, result.Method);
        (string fileName, IReadOnlyList<string> arguments) = Assert.Single(launcher.Calls);
        Assert.Equal(Wsl, fileName);
        Assert.Empty(arguments);
    }

    [Fact]
    public async Task LaunchDistributionAsync_UsesWindowsTerminal_WhenAvailable()
    {
        FakeTerminalProcessLauncher launcher = new(WindowsTerminal, Wsl);
        WslTerminalService service = new(launcher);

        WslTerminalLaunchResult result =
            await service.LaunchDistributionAsync(Name("Ubuntu"), CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(TerminalLaunchMethod.WindowsTerminal, result.Method);
        (string fileName, IReadOnlyList<string> arguments) = Assert.Single(launcher.Calls);
        Assert.Equal(WindowsTerminal, fileName);
        Assert.Equal([Wsl, "--distribution", "Ubuntu"], arguments);
    }

    [Fact]
    public async Task LaunchDistributionAsync_FallsBackToWsl_WhenWindowsTerminalUnavailable()
    {
        FakeTerminalProcessLauncher launcher = new(Wsl);
        WslTerminalService service = new(launcher);

        WslTerminalLaunchResult result =
            await service.LaunchDistributionAsync(Name("Ubuntu"), CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(TerminalLaunchMethod.Wsl, result.Method);
        (string fileName, IReadOnlyList<string> arguments) = Assert.Single(launcher.Calls);
        Assert.Equal(Wsl, fileName);
        Assert.Equal(["--distribution", "Ubuntu"], arguments);
    }

    [Fact]
    public async Task LaunchDistributionAsync_PassesNameWithSpaces_AsSingleArgument()
    {
        FakeTerminalProcessLauncher launcher = new(WindowsTerminal, Wsl);
        WslTerminalService service = new(launcher);

        await service.LaunchDistributionAsync(Name("Ubuntu Preview Edition"), CancellationToken.None);

        (_, IReadOnlyList<string> arguments) = Assert.Single(launcher.Calls);
        Assert.Equal([Wsl, "--distribution", "Ubuntu Preview Edition"], arguments);

        // The name must not be split or manually quoted; quoting is ProcessStartInfo.ArgumentList's job.
        string distributionArgument = arguments[^1];
        Assert.DoesNotContain('"', distributionArgument);
        Assert.Equal("Ubuntu Preview Edition", distributionArgument);
    }

    [Fact]
    public async Task LaunchDistributionAsync_FailsWithoutLaunching_WhenBothExecutablesUnavailable()
    {
        FakeTerminalProcessLauncher launcher = new();
        WslTerminalService service = new(launcher);

        WslTerminalLaunchResult result =
            await service.LaunchDistributionAsync(Name("Ubuntu"), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(TerminalLaunchMethod.None, result.Method);
        Assert.Contains("Windows Terminal", result.UserSafeMessage);
        Assert.Empty(launcher.Calls);
    }

    [Fact]
    public async Task LaunchDistributionAsync_FailsWithoutLaunching_WhenCancellationRequested()
    {
        FakeTerminalProcessLauncher launcher = new(WindowsTerminal, Wsl);
        WslTerminalService service = new(launcher);
        using CancellationTokenSource cancellation = new();
        await cancellation.CancelAsync();

        WslTerminalLaunchResult result =
            await service.LaunchDistributionAsync(Name("Ubuntu"), cancellation.Token);

        Assert.False(result.Succeeded);
        Assert.Contains("canceled", result.UserSafeMessage, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(launcher.Calls);
    }

    [Fact]
    public async Task LaunchDistributionAsync_FallsBackToWsl_WhenWindowsTerminalFailsToStart()
    {
        FakeTerminalProcessLauncher launcher = new(WindowsTerminal, Wsl);
        launcher.Results[WindowsTerminal] = TerminalProcessLaunchResult.Failure("wt.exe could not be started.");
        WslTerminalService service = new(launcher);

        WslTerminalLaunchResult result =
            await service.LaunchDistributionAsync(Name("Ubuntu"), CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(TerminalLaunchMethod.Wsl, result.Method);
        Assert.Equal(2, launcher.Calls.Count);
        Assert.Equal(Wsl, launcher.Calls[^1].FileName);
    }

    [Fact]
    public async Task LaunchDistributionAsync_ReturnsStructuredFailure_WhenLaunchFails()
    {
        FakeTerminalProcessLauncher launcher = new(Wsl);
        launcher.Results[Wsl] = TerminalProcessLaunchResult.Failure("wsl.exe could not be found or started.");
        WslTerminalService service = new(launcher);

        WslTerminalLaunchResult result =
            await service.LaunchDistributionAsync(Name("Ubuntu"), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("wsl.exe could not be found or started.", result.UserSafeMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void InvalidDistributionNames_CannotBeConstructed(string? value)
    {
        // Invalid names are rejected by the value object, so they can never reach the launcher.
        Assert.False(DistributionName.TryCreate(value, out DistributionName? name));
        Assert.Null(name);
    }

    [Fact]
    public async Task LaunchDistributionAsync_UsesOnlyOfficialExecutablesAndDiscreteArguments()
    {
        FakeTerminalProcessLauncher launcher = new(WindowsTerminal, Wsl);
        WslTerminalService service = new(launcher);

        await service.LaunchDistributionAsync(Name("Ubuntu"), CancellationToken.None);

        (string fileName, IReadOnlyList<string> arguments) = Assert.Single(launcher.Calls);
        Assert.Contains(fileName, new[] { WindowsTerminal, Wsl });

        foreach (string argument in arguments)
        {
            Assert.DoesNotContain("cmd.exe", argument, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("powershell", argument, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("&&", argument, StringComparison.Ordinal);
            Assert.DoesNotContain("|", argument, StringComparison.Ordinal);
            Assert.DoesNotContain("\"", argument, StringComparison.Ordinal);
        }
    }

    [Fact]
    public async Task LaunchDefaultAsync_ReturnsWithoutWaitingForTerminalExit()
    {
        FakeTerminalProcessLauncher launcher = new(WindowsTerminal, Wsl);
        WslTerminalService service = new(launcher);

        Task<WslTerminalLaunchResult> task = service.LaunchDefaultAsync(CancellationToken.None);

        // Already completed before it is awaited: the launch never waits for the terminal to exit.
        Assert.True(task.IsCompleted);

        WslTerminalLaunchResult result = await task;
        Assert.True(result.Succeeded);
    }

    private static DistributionName Name(string value)
    {
        Assert.True(DistributionName.TryCreate(value, out DistributionName? name));
        return name!;
    }

    private sealed class FakeTerminalProcessLauncher(params string[] availableExecutables) : ITerminalProcessLauncher
    {
        private readonly HashSet<string> _available = new(availableExecutables, StringComparer.OrdinalIgnoreCase);

        public List<(string FileName, IReadOnlyList<string> Arguments)> Calls { get; } = [];

        public Dictionary<string, TerminalProcessLaunchResult> Results { get; } =
            new(StringComparer.OrdinalIgnoreCase);

        public bool IsExecutableAvailable(string executableFileName) => _available.Contains(executableFileName);

        public TerminalProcessLaunchResult Start(string executableFileName, IReadOnlyList<string> arguments)
        {
            Calls.Add((executableFileName, arguments));

            return Results.TryGetValue(executableFileName, out TerminalProcessLaunchResult? result)
                ? result
                : TerminalProcessLaunchResult.Success();
        }
    }
}
