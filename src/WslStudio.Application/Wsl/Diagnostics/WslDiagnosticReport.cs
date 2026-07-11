namespace WslStudio.Application.Wsl.Diagnostics;

/// <summary>
/// A safe, read-only snapshot of the local WSL environment and health information, shaped for export.
/// </summary>
/// <remarks>
/// This is a dedicated export DTO composed from existing read-only services. It intentionally contains
/// only non-sensitive fields (versions, counts, names, health status) and excludes usernames, file
/// paths, environment variables, raw command output, and Linux filesystem contents.
/// </remarks>
public sealed record WslDiagnosticReport(
    string SchemaVersion,
    DateTimeOffset GeneratedAtUtc,
    DiagnosticEnvironmentInfo Environment,
    IReadOnlyList<DiagnosticDistributionInfo> Distributions,
    DiagnosticHealthSummary HealthSummary,
    IReadOnlyList<DiagnosticHealthCheckInfo> HealthChecks,
    IReadOnlyList<string> Sources);

/// <summary>Consolidated WSL environment and version information.</summary>
public sealed record DiagnosticEnvironmentInfo(
    string? WindowsVersion,
    string? WslVersion,
    string? KernelVersion,
    string? WslgVersion,
    int? DefaultWslVersion,
    int InstalledDistributionCount,
    int RunningDistributionCount,
    string? DefaultDistribution,
    bool DockerDesktopDetected,
    bool WslgAvailable,
    string? Direct3DVersion,
    string? DxCoreVersion,
    string? MsrdcVersion);

/// <summary>Non-sensitive per-distribution summary.</summary>
public sealed record DiagnosticDistributionInfo(
    string Name,
    string State,
    int? WslVersion,
    bool IsDefault);

/// <summary>Aggregate health counts and overall status.</summary>
public sealed record DiagnosticHealthSummary(
    string OverallStatus,
    int HealthyCount,
    int WarningCount,
    int ErrorCount,
    int NotCheckedCount,
    string Recommendation);

/// <summary>
/// A single health check, limited to safe display fields. Raw <c>Details</c> and technical metadata
/// from the underlying check are intentionally excluded from the exported report.
/// </summary>
public sealed record DiagnosticHealthCheckInfo(
    string Name,
    string Category,
    string Status,
    string Summary,
    string Recommendation);
