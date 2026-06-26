using WslStudio.Application.Wsl;
using WslStudio.Core.Wsl;

namespace WslStudio.Tests.Application.Wsl;

public sealed class WslDistributionParserTests
{
    private readonly WslDistributionParser _parser = new();

    [Fact]
    public void ParseListVerbose_HandlesDefaultMarkerAndStates()
    {
        WslDistributionParseResult result = _parser.ParseListVerbose(WslListVerboseFixtures.NormalOutput);

        Assert.True(result.Succeeded);
        Assert.Equal(2, result.Distributions.Count);

        WslDistribution defaultDistribution = result.Distributions[0];
        Assert.Equal("Ubuntu", defaultDistribution.Name.Value);
        Assert.True(defaultDistribution.IsDefault);
        Assert.Equal(WslDistributionState.Running, defaultDistribution.State);
        Assert.Equal(2, defaultDistribution.Version);

        WslDistribution stoppedDistribution = result.Distributions[1];
        Assert.Equal("Debian", stoppedDistribution.Name.Value);
        Assert.False(stoppedDistribution.IsDefault);
        Assert.Equal(WslDistributionState.Stopped, stoppedDistribution.State);
        Assert.Equal(2, stoppedDistribution.Version);
    }

    [Fact]
    public void ParseListVerbose_HandlesDistributionNamesWithSpaces()
    {
        WslDistributionParseResult result = _parser.ParseListVerbose(WslListVerboseFixtures.NameWithSpacesOutput);

        Assert.True(result.Succeeded);
        WslDistribution distribution = Assert.Single(result.Distributions);
        Assert.Equal("Ubuntu Preview", distribution.Name.Value);
        Assert.Equal(WslDistributionState.Stopped, distribution.State);
        Assert.Equal(2, distribution.Version);
    }

    [Fact]
    public void ParseListVerbose_HandlesUtf16NullSeparatedOutput()
    {
        WslDistributionParseResult result = _parser.ParseListVerbose(WslListVerboseFixtures.NullSeparatedOutput);

        Assert.True(result.Succeeded);
        WslDistribution distribution = Assert.Single(result.Distributions);
        Assert.Equal("Ubuntu", distribution.Name.Value);
    }

    [Fact]
    public void ParseListVerbose_ReturnsFailure_ForUnsupportedFormat()
    {
        WslDistributionParseResult result = _parser.ParseListVerbose("unexpected output");

        Assert.False(result.Succeeded);
        Assert.Empty(result.Distributions);
        Assert.Equal("WSL returned distribution output in an unsupported format.", result.UserSafeMessage);
    }
}
