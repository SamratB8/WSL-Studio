using WslStudio.Application.Commands;

namespace WslStudio.Tests.Application.Commands;

public sealed class CommandResultTests
{
    [Fact]
    public void Success_WithZeroExitCode_ReturnsSuccessfulResult()
    {
        CommandResult result = CommandResult.Success(
            "wsl.exe",
            ["--status"],
            "Default Version: 2",
            string.Empty,
            exitCode: 0,
            TimeSpan.FromMilliseconds(25));

        Assert.True(result.Succeeded);
        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        Assert.Empty(result.UserSafeErrorMessage);
    }

    [Fact]
    public void Timeout_ReturnsUserSafeTimeoutMessage()
    {
        CommandResult result = CommandResult.Timeout(
            "wsl.exe",
            ["--list", "--verbose"],
            TimeSpan.FromSeconds(10));

        Assert.False(result.Succeeded);
        Assert.True(result.TimedOut);
        Assert.Null(result.ExitCode);
        Assert.Equal("The WSL command timed out.", result.UserSafeErrorMessage);
    }
}
