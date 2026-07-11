using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed record WslDistributionDetails(
    WslDistribution Distribution,
    string? Architecture,
    string? InstallationLocation,
    string? KernelVersion);
