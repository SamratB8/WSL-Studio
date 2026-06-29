using WslStudio.Application.Wsl;
using WslStudio.Core.Wsl;

namespace WslStudio.Tests.Application.Wsl;

public sealed class WslDistributionQueryTests
{
    [Fact]
    public void Apply_SearchesByName_CaseInsensitiveAndPartial()
    {
        WslDistribution[] distributions =
        [
            Dist("Ubuntu", WslDistributionState.Running),
            Dist("Debian", WslDistributionState.Stopped),
            Dist("docker-desktop", WslDistributionState.Stopped)
        ];

        IReadOnlyList<WslDistribution> result = WslDistributionQuery.Apply(
            distributions, "UBUN", DistributionStateFilter.All, DistributionSortKey.Name);

        Assert.Single(result);
        Assert.Equal("Ubuntu", result[0].Name.Value);
    }

    [Fact]
    public void Apply_FiltersByRunning()
    {
        WslDistribution[] distributions =
        [
            Dist("Ubuntu", WslDistributionState.Running),
            Dist("Debian", WslDistributionState.Stopped),
            Dist("Alpine", WslDistributionState.Running)
        ];

        IReadOnlyList<WslDistribution> result = WslDistributionQuery.Apply(
            distributions, searchText: null, DistributionStateFilter.Running, DistributionSortKey.Name);

        Assert.Equal(2, result.Count);
        Assert.All(result, distribution => Assert.Equal(WslDistributionState.Running, distribution.State));
    }

    [Fact]
    public void Apply_FiltersByStopped()
    {
        WslDistribution[] distributions =
        [
            Dist("Ubuntu", WslDistributionState.Running),
            Dist("Debian", WslDistributionState.Stopped)
        ];

        IReadOnlyList<WslDistribution> result = WslDistributionQuery.Apply(
            distributions, searchText: null, DistributionStateFilter.Stopped, DistributionSortKey.Name);

        Assert.Single(result);
        Assert.Equal("Debian", result[0].Name.Value);
    }

    [Fact]
    public void Apply_FiltersByOther_ExcludesRunningAndStopped()
    {
        WslDistribution[] distributions =
        [
            Dist("Ubuntu", WslDistributionState.Running),
            Dist("Debian", WslDistributionState.Stopped),
            Dist("Installing-One", WslDistributionState.Installing),
            Dist("Mystery", WslDistributionState.Unknown)
        ];

        IReadOnlyList<WslDistribution> result = WslDistributionQuery.Apply(
            distributions, searchText: null, DistributionStateFilter.Other, DistributionSortKey.Name);

        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, distribution =>
            distribution.State is WslDistributionState.Running or WslDistributionState.Stopped);
    }

    [Fact]
    public void Apply_SortsByName_Ascending()
    {
        WslDistribution[] distributions =
        [
            Dist("Ubuntu", WslDistributionState.Stopped),
            Dist("Debian", WslDistributionState.Stopped),
            Dist("Alpine", WslDistributionState.Stopped)
        ];

        IReadOnlyList<WslDistribution> result = WslDistributionQuery.Apply(
            distributions, searchText: null, DistributionStateFilter.All, DistributionSortKey.Name);

        Assert.Equal(["Alpine", "Debian", "Ubuntu"], result.Select(d => d.Name.Value));
    }

    [Fact]
    public void Apply_SortsByState_RunningBeforeStopped()
    {
        WslDistribution[] distributions =
        [
            Dist("Stopped-One", WslDistributionState.Stopped),
            Dist("Running-One", WslDistributionState.Running),
            Dist("Unknown-One", WslDistributionState.Unknown)
        ];

        IReadOnlyList<WslDistribution> result = WslDistributionQuery.Apply(
            distributions, searchText: null, DistributionStateFilter.All, DistributionSortKey.State);

        Assert.Equal(
            [WslDistributionState.Running, WslDistributionState.Stopped, WslDistributionState.Unknown],
            result.Select(d => d.State));
    }

    [Fact]
    public void Apply_SortsByVersion_AscendingWithUnknownLast()
    {
        WslDistribution[] distributions =
        [
            Dist("Alpha", WslDistributionState.Stopped, version: 2),
            Dist("Bravo", WslDistributionState.Stopped, version: 1),
            Dist("Charlie", WslDistributionState.Stopped, version: null)
        ];

        IReadOnlyList<WslDistribution> result = WslDistributionQuery.Apply(
            distributions, searchText: null, DistributionStateFilter.All, DistributionSortKey.Version);

        Assert.Equal([1, 2, null], result.Select(d => d.Version));
    }

    [Fact]
    public void Apply_SortsByDefaultFirst()
    {
        WslDistribution[] distributions =
        [
            Dist("Zeta", WslDistributionState.Stopped, isDefault: true),
            Dist("Alpha", WslDistributionState.Stopped, isDefault: false)
        ];

        IReadOnlyList<WslDistribution> result = WslDistributionQuery.Apply(
            distributions, searchText: null, DistributionStateFilter.All, DistributionSortKey.DefaultFirst);

        Assert.True(result[0].IsDefault);
        Assert.Equal("Zeta", result[0].Name.Value);
    }

    [Fact]
    public void Apply_ReturnsEmpty_WhenSearchHasNoMatch()
    {
        WslDistribution[] distributions =
        [
            Dist("Ubuntu", WslDistributionState.Running),
            Dist("Debian", WslDistributionState.Stopped)
        ];

        IReadOnlyList<WslDistribution> result = WslDistributionQuery.Apply(
            distributions, "no-such-distribution", DistributionStateFilter.All, DistributionSortKey.Name);

        Assert.Empty(result);
    }

    [Fact]
    public void Apply_ReturnsEmpty_WhenFilterExcludesEverything()
    {
        WslDistribution[] distributions =
        [
            Dist("Ubuntu", WslDistributionState.Stopped),
            Dist("Debian", WslDistributionState.Stopped)
        ];

        IReadOnlyList<WslDistribution> result = WslDistributionQuery.Apply(
            distributions, searchText: null, DistributionStateFilter.Running, DistributionSortKey.Name);

        Assert.Empty(result);
    }

    private static WslDistribution Dist(
        string name,
        WslDistributionState state,
        int? version = 2,
        bool isDefault = false)
    {
        bool created = DistributionName.TryCreate(name, out DistributionName? distributionName);
        Assert.True(created);
        Assert.NotNull(distributionName);

        return new WslDistribution(distributionName, state, version, isDefault);
    }
}
