namespace WslStudio.Application.Wsl.Terminal;

/// <summary>
/// Boundary for starting an <b>interactive</b> process that the user owns.
/// </summary>
/// <remarks>
/// This is intentionally separate from <see cref="Commands.ICommandRunner"/>. The command runner
/// redirects standard output and error, hides the window, and awaits process exit so it can capture
/// read-only output. An interactive terminal requires the opposite: no redirection, a visible window,
/// and no waiting for exit. Implementations must pass arguments individually (never a concatenated
/// command string) and must not involve a shell.
/// </remarks>
public interface ITerminalProcessLauncher
{
    /// <summary>
    /// Returns whether the executable can be resolved without starting any process.
    /// </summary>
    bool IsExecutableAvailable(string executableFileName);

    /// <summary>
    /// Starts the executable and returns immediately without waiting for it to exit.
    /// </summary>
    TerminalProcessLaunchResult Start(string executableFileName, IReadOnlyList<string> arguments);
}
