using System.ComponentModel;
using System.Diagnostics;
using WslStudio.Application.Wsl.Terminal;

namespace WslStudio.Infrastructure.Terminal;

/// <summary>
/// Starts interactive terminal processes using <see cref="Process"/>.
/// </summary>
/// <remarks>
/// Executable availability is resolved by probing the directories in <c>PATH</c>; no process is
/// started to perform detection. Started processes are never awaited, so the terminal stays open and
/// under the user's control. Disposing the <see cref="Process"/> wrapper releases local handles only
/// and does not terminate the launched terminal.
/// </remarks>
public sealed class ProcessTerminalLauncher : ITerminalProcessLauncher
{
    public bool IsExecutableAvailable(string executableFileName)
    {
        if (string.IsNullOrWhiteSpace(executableFileName))
        {
            return false;
        }

        string? pathVariable = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(pathVariable))
        {
            return false;
        }

        foreach (string directory in pathVariable.Split(
            Path.PathSeparator,
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            try
            {
                if (File.Exists(Path.Combine(directory, executableFileName)))
                {
                    return true;
                }
            }
            catch (ArgumentException)
            {
                // A malformed PATH entry should never prevent the remaining entries from being checked.
            }
        }

        return false;
    }

    public TerminalProcessLaunchResult Start(string executableFileName, IReadOnlyList<string> arguments)
    {
        try
        {
            using Process? process = Process.Start(
                TerminalProcessStartInfoFactory.Create(executableFileName, arguments));

            return process is null
                ? TerminalProcessLaunchResult.Failure("The terminal process could not be started.")
                : TerminalProcessLaunchResult.Success();
        }
        catch (Win32Exception)
        {
            return TerminalProcessLaunchResult.Failure($"{executableFileName} could not be found or started.");
        }
        catch (InvalidOperationException)
        {
            return TerminalProcessLaunchResult.Failure("The terminal could not be started.");
        }
    }
}
