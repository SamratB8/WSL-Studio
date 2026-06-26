namespace WslStudio.Application.Wsl;

public sealed record WslHealthCenterSummary(
    int HealthyCount,
    int WarningCount,
    int ErrorCount,
    int UnknownCount,
    string OverallStatus,
    string Recommendation);
