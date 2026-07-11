namespace WslStudio.Application.Wsl;

/// <summary>
/// Ordering applied to a distribution list. All orderings use the distribution name as a
/// final, case-insensitive tie-breaker so results are deterministic.
/// </summary>
public enum DistributionSortKey
{
    /// <summary>The default distribution first, then by name.</summary>
    DefaultFirst,

    /// <summary>By distribution name, ascending.</summary>
    Name,

    /// <summary>By running state (running first), then by name.</summary>
    State,

    /// <summary>By WSL version ascending, with unknown versions last, then by name.</summary>
    Version
}
