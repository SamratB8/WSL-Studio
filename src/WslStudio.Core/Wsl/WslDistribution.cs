namespace WslStudio.Core.Wsl;

public sealed record WslDistribution(
    DistributionName Name,
    WslDistributionState State,
    int? Version,
    bool IsDefault);
