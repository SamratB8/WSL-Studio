namespace WslStudio.Application.Wsl.Terminal;

/// <summary>
/// Identifies which officially supported executable was used to open the interactive terminal.
/// </summary>
public enum TerminalLaunchMethod
{
    /// <summary>No terminal was launched.</summary>
    None,

    /// <summary>Launched through Windows Terminal (<c>wt.exe</c>).</summary>
    WindowsTerminal,

    /// <summary>Launched directly through <c>wsl.exe</c>.</summary>
    Wsl
}
