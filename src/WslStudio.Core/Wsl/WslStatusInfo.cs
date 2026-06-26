namespace WslStudio.Core.Wsl;

public sealed record WslStatusInfo(
    DistributionName? DefaultDistribution,
    int? DefaultVersion,
    string? KernelVersion);
