namespace WslStudio.Application.Wsl;

/// <summary>
/// Read-only filter applied to a distribution list by running state.
/// </summary>
public enum DistributionStateFilter
{
    All,
    Running,
    Stopped,

    /// <summary>
    /// Any state that is neither running nor stopped (for example installing, converting, or unknown).
    /// </summary>
    Other
}
