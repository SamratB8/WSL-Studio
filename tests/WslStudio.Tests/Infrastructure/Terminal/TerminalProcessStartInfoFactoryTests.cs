using System.Diagnostics;
using WslStudio.Infrastructure.Terminal;

namespace WslStudio.Tests.Infrastructure.Terminal;

public sealed class TerminalProcessStartInfoFactoryTests
{
    [Fact]
    public void Create_DoesNotRedirectAnyStandardStream()
    {
        ProcessStartInfo startInfo = TerminalProcessStartInfoFactory.Create("wsl.exe", ["--distribution", "Ubuntu"]);

        Assert.False(startInfo.RedirectStandardInput);
        Assert.False(startInfo.RedirectStandardOutput);
        Assert.False(startInfo.RedirectStandardError);
    }

    [Fact]
    public void Create_StartsExecutableDirectlyWithAVisibleWindow()
    {
        ProcessStartInfo startInfo = TerminalProcessStartInfoFactory.Create("wt.exe", ["wsl.exe"]);

        // UseShellExecute = false starts the executable directly: no cmd.exe, no PowerShell, no shell.
        Assert.False(startInfo.UseShellExecute);
        Assert.False(startInfo.CreateNoWindow);
        Assert.Equal("wt.exe", startInfo.FileName);
    }

    [Fact]
    public void Create_PassesArgumentsThroughArgumentList_NotAConcatenatedString()
    {
        ProcessStartInfo startInfo = TerminalProcessStartInfoFactory.Create(
            "wt.exe",
            ["wsl.exe", "--distribution", "Ubuntu Preview Edition"]);

        Assert.Equal(["wsl.exe", "--distribution", "Ubuntu Preview Edition"], startInfo.ArgumentList);

        // Arguments must remain empty so no concatenated/interpolated command string is ever built.
        Assert.True(string.IsNullOrEmpty(startInfo.Arguments));
    }

    [Fact]
    public void Create_KeepsNamesWithSpacesAsASingleArgument()
    {
        ProcessStartInfo startInfo = TerminalProcessStartInfoFactory.Create(
            "wsl.exe",
            ["--distribution", "Ubuntu Preview Edition"]);

        Assert.Equal(2, startInfo.ArgumentList.Count);
        Assert.Equal("Ubuntu Preview Edition", startInfo.ArgumentList[1]);
        Assert.DoesNotContain('"', startInfo.ArgumentList[1]);
    }

    [Fact]
    public void Create_SupportsAnEmptyArgumentList()
    {
        ProcessStartInfo startInfo = TerminalProcessStartInfoFactory.Create("wsl.exe", []);

        Assert.Equal("wsl.exe", startInfo.FileName);
        Assert.Empty(startInfo.ArgumentList);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_RejectsMissingExecutableName(string executableFileName)
    {
        Assert.Throws<ArgumentException>(() => TerminalProcessStartInfoFactory.Create(executableFileName, []));
    }
}
