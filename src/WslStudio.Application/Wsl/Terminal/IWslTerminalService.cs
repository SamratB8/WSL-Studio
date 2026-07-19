using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl.Terminal;

/// <summary>
/// Opens an interactive WSL terminal window using officially supported executables. This starts a
/// user-owned terminal process; it does not run commands inside a distribution and does not change
/// WSL configuration.
/// </summary>
public interface IWslTerminalService
{
    Task<WslTerminalLaunchResult> LaunchDefaultAsync(CancellationToken cancellationToken);

    Task<WslTerminalLaunchResult> LaunchDistributionAsync(DistributionName distribution, CancellationToken cancellationToken);
}
