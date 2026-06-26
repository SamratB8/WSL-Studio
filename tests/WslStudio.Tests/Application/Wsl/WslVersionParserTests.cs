using WslStudio.Application.Wsl;

namespace WslStudio.Tests.Application.Wsl;

public sealed class WslVersionParserTests
{
    private readonly WslVersionParser _parser = new();

    [Fact]
    public void Parse_ReadsVersionFields()
    {
        WslVersionParseResult result = _parser.Parse(WslStatusAndVersionFixtures.VersionOutput);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.VersionInfo);
        Assert.Equal("2.4.13.0", result.VersionInfo.WslVersion);
        Assert.Equal("5.15.167.4-1", result.VersionInfo.KernelVersion);
        Assert.Equal("1.0.65", result.VersionInfo.WslgVersion);
        Assert.Equal("10.0.26100.0", result.VersionInfo.WindowsVersion);
    }

    [Fact]
    public void Parse_AllowsEmptyOutput()
    {
        WslVersionParseResult result = _parser.Parse(string.Empty);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.VersionInfo);
        Assert.Null(result.VersionInfo.WslVersion);
    }
}
