using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl.Terminal;

/// <summary>
/// Chooses the terminal executable and builds its argument list.
/// </summary>
/// <remarks>
/// Windows Terminal is preferred when <c>wt.exe</c> can be resolved, launching
/// <c>wt.exe wsl.exe --distribution &lt;name&gt;</c>. Otherwise the launch falls back to
/// <c>wsl.exe --distribution &lt;name&gt;</c>, or plain <c>wsl.exe</c> for the default distribution.
/// Arguments are always supplied as discrete values so no quoting or shell interpolation is required,
/// which keeps distribution names containing spaces safe.
/// </remarks>
public sealed class WslTerminalService(ITerminalProcessLauncher launcher) : IWslTerminalService
{
    internal const string WindowsTerminalExecutable = "wt.exe";
    internal const string WslExecutable = "wsl.exe";
    internal const string DistributionArgument = "--distribution";

    private const string CanceledMessage = "Opening the terminal was canceled.";
    private const string NoExecutableMessage =
        "No supported terminal was found. Windows Terminal (wt.exe) and wsl.exe could not be located.";
    private const string MissingDistributionMessage =
        "Select a distribution before opening a terminal.";

    public Task<WslTerminalLaunchResult> LaunchDefaultAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Launch(distribution: null, cancellationToken));

    public Task<WslTerminalLaunchResult> LaunchDistributionAsync(
        DistributionName distribution,
        CancellationToken cancellationToken)
    {
        if (distribution is null || string.IsNullOrWhiteSpace(distribution.Value))
        {
            return Task.FromResult(WslTerminalLaunchResult.Failure(MissingDistributionMessage));
        }

        return Task.FromResult(Launch(distribution, cancellationToken));
    }

    private WslTerminalLaunchResult Launch(DistributionName? distribution, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return WslTerminalLaunchResult.Failure(CanceledMessage);
        }

        if (launcher.IsExecutableAvailable(WindowsTerminalExecutable))
        {
            TerminalProcessLaunchResult windowsTerminalResult =
                launcher.Start(WindowsTerminalExecutable, BuildWindowsTerminalArguments(distribution));

            if (windowsTerminalResult.Succeeded)
            {
                return WslTerminalLaunchResult.Success(TerminalLaunchMethod.WindowsTerminal);
            }
        }

        if (!launcher.IsExecutableAvailable(WslExecutable))
        {
            return WslTerminalLaunchResult.Failure(NoExecutableMessage);
        }

        TerminalProcessLaunchResult wslResult = launcher.Start(WslExecutable, BuildWslArguments(distribution));

        return wslResult.Succeeded
            ? WslTerminalLaunchResult.Success(TerminalLaunchMethod.Wsl)
            : WslTerminalLaunchResult.Failure(wslResult.UserSafeMessage);
    }

    private static IReadOnlyList<string> BuildWindowsTerminalArguments(DistributionName? distribution) =>
        distribution is null
            ? [WslExecutable]
            : [WslExecutable, DistributionArgument, distribution.Value];

    private static IReadOnlyList<string> BuildWslArguments(DistributionName? distribution) =>
        distribution is null
            ? []
            : [DistributionArgument, distribution.Value];
}
