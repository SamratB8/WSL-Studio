using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed record WslDashboardOverview(
    DistributionName? DefaultDistribution,
    int RunningDistributionCount,
    int StoppedDistributionCount,
    int TotalDistributionCount,
    WslStatusInfo? StatusInfo,
    WslVersionInfo? VersionInfo);
