using WslStudio.Application.Wsl;

namespace WslStudio.Tests.Application.Wsl;

public sealed class WslStatusParserTests
{
    private readonly WslStatusParser _parser = new();

    [Fact]
    public void Parse_ReadsDefaultDistributionAndVersion()
    {
        WslStatusParseResult result = _parser.Parse(WslStatusAndVersionFixtures.StatusOutput);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.StatusInfo);
        Assert.Equal("Ubuntu", result.StatusInfo.DefaultDistribution?.Value);
        Assert.Equal(2, result.StatusInfo.DefaultVersion);
        Assert.Equal("6.18.35.2-1", result.StatusInfo.KernelVersion);
    }

    [Fact]
    public void Parse_AllowsEmptyOutput()
    {
        WslStatusParseResult result = _parser.Parse(string.Empty);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.StatusInfo);
        Assert.Null(result.StatusInfo.DefaultDistribution);
        Assert.Null(result.StatusInfo.DefaultVersion);
    }

    [Fact]
    public void Parse_ReadsDefaultDistributionWithSpaces()
    {
        WslStatusParseResult result = _parser.Parse("Default Distribution: Ubuntu Preview");

        Assert.True(result.Succeeded);
        Assert.NotNull(result.StatusInfo);
        Assert.Equal("Ubuntu Preview", result.StatusInfo.DefaultDistribution?.Value);
    }
}
