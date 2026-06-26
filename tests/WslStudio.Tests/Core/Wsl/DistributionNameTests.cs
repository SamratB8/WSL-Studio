using WslStudio.Core.Wsl;

namespace WslStudio.Tests.Core.Wsl;

public sealed class DistributionNameTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Ubuntu\nPreview")]
    public void TryCreate_RejectsInvalidNames(string value)
    {
        bool result = DistributionName.TryCreate(value, out DistributionName? distributionName);

        Assert.False(result);
        Assert.Null(distributionName);
    }

    [Fact]
    public void TryCreate_TrimsValidName()
    {
        bool result = DistributionName.TryCreate(" Ubuntu Preview ", out DistributionName? distributionName);

        Assert.True(result);
        Assert.NotNull(distributionName);
        Assert.Equal("Ubuntu Preview", distributionName.Value);
    }
}
