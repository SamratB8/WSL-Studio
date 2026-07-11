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
        Assert.Equal("2.9.3.0", result.VersionInfo.WslVersion);
        Assert.Equal("6.18.35.2-1", result.VersionInfo.KernelVersion);
        Assert.Equal("1.0.79", result.VersionInfo.WslgVersion);
        Assert.Equal("10.0.26300.8758", result.VersionInfo.WindowsVersion);
        Assert.Equal("1.611.1-81528511", result.VersionInfo.Direct3DVersion);
        Assert.Equal("10.0.26100.1-240331-1435.ge-release", result.VersionInfo.DxCoreVersion);
        Assert.Equal("1.2.7214", result.VersionInfo.MsrdcVersion);
    }

    [Fact]
    public void Parse_LeavesGraphicsFieldsNull_WhenNotReported()
    {
        WslVersionParseResult result = _parser.Parse(WslStatusAndVersionFixtures.VersionOutputWithoutGraphics);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.VersionInfo);
        Assert.Equal("2.9.3.0", result.VersionInfo.WslVersion);
        Assert.Null(result.VersionInfo.Direct3DVersion);
        Assert.Null(result.VersionInfo.DxCoreVersion);
        Assert.Null(result.VersionInfo.MsrdcVersion);
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
