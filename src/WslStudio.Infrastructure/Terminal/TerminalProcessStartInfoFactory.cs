using System.Diagnostics;

namespace WslStudio.Infrastructure.Terminal;

/// <summary>
/// Builds the <see cref="ProcessStartInfo"/> used to open an interactive terminal.
/// </summary>
/// <remarks>
/// This factory encodes the safety policy for interactive launches so it can be verified by tests:
/// no shell is used, arguments are added individually through
/// <see cref="ProcessStartInfo.ArgumentList"/> rather than a concatenated command string, and no
/// standard stream is redirected so the terminal remains fully interactive.
/// </remarks>
public static class TerminalProcessStartInfoFactory
{
    public static ProcessStartInfo Create(string executableFileName, IReadOnlyList<string> arguments)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executableFileName);
        ArgumentNullException.ThrowIfNull(arguments);

        ProcessStartInfo startInfo = new()
        {
            FileName = executableFileName,

            // Start the executable directly. No cmd.exe, no PowerShell, no shell interpolation.
            UseShellExecute = false,

            // The terminal window must remain visible and interactive.
            CreateNoWindow = false,
            RedirectStandardInput = false,
            RedirectStandardOutput = false,
            RedirectStandardError = false
        };

        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        return startInfo;
    }
}
