namespace WslStudio.Application.Wsl;

/// <summary>
/// A consolidated, read-only snapshot of the local WSL environment.
/// </summary>
/// <remarks>
/// This composes the existing <see cref="WslDashboardOverview"/> (distribution counts, default
/// distribution, parsed status and version information) and adds only the two environment-specific
/// facts that the dashboard does not derive: whether the optional Docker Desktop WSL distribution is
/// present and whether WSLg is available. It deliberately reuses the dashboard overview rather than
/// re-deriving the same data so there is a single source of truth and no duplicate WSL queries.
/// </remarks>
public sealed record WslEnvironmentOverview(
    WslDashboardOverview Overview,
    bool DockerDesktopDetected,
    bool WslgAvailable);
