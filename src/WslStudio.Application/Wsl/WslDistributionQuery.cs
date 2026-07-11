using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

/// <summary>
/// Applies read-only search, state filtering, and sorting to an already-discovered set of
/// distributions. This is a pure projection over data that was returned by official WSL
/// commands: it never runs commands, performs I/O, or changes WSL state.
/// </summary>
public static class WslDistributionQuery
{
    public static IReadOnlyList<WslDistribution> Apply(
        IEnumerable<WslDistribution> distributions,
        string? searchText,
        DistributionStateFilter stateFilter,
        DistributionSortKey sortKey)
    {
        ArgumentNullException.ThrowIfNull(distributions);

        IEnumerable<WslDistribution> query = distributions;

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            string term = searchText.Trim();
            query = query.Where(distribution =>
                distribution.Name.Value.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        query = stateFilter switch
        {
            DistributionStateFilter.Running =>
                query.Where(distribution => distribution.State == WslDistributionState.Running),
            DistributionStateFilter.Stopped =>
                query.Where(distribution => distribution.State == WslDistributionState.Stopped),
            DistributionStateFilter.Other =>
                query.Where(distribution =>
                    distribution.State != WslDistributionState.Running &&
                    distribution.State != WslDistributionState.Stopped),
            _ => query
        };

        IOrderedEnumerable<WslDistribution> ordered = sortKey switch
        {
            DistributionSortKey.Name =>
                query.OrderBy(distribution => distribution.Name.Value, StringComparer.OrdinalIgnoreCase),
            DistributionSortKey.State =>
                query
                    .OrderBy(distribution => StateRank(distribution.State))
                    .ThenBy(distribution => distribution.Name.Value, StringComparer.OrdinalIgnoreCase),
            DistributionSortKey.Version =>
                query
                    .OrderBy(distribution => distribution.Version is null)
                    .ThenBy(distribution => distribution.Version ?? int.MaxValue)
                    .ThenBy(distribution => distribution.Name.Value, StringComparer.OrdinalIgnoreCase),
            _ =>
                query
                    .OrderByDescending(distribution => distribution.IsDefault)
                    .ThenBy(distribution => distribution.Name.Value, StringComparer.OrdinalIgnoreCase)
        };

        return ordered.ToList();
    }

    /// <summary>
    /// Orders states by activity so that the most relevant distributions appear first.
    /// </summary>
    private static int StateRank(WslDistributionState state) => state switch
    {
        WslDistributionState.Running => 0,
        WslDistributionState.Stopped => 1,
        WslDistributionState.Installing => 2,
        WslDistributionState.Converting => 3,
        _ => 4
    };
}
