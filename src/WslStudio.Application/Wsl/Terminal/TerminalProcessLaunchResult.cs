namespace WslStudio.Application.Wsl.Terminal;

/// <summary>
/// Result of starting an interactive process at the infrastructure boundary.
/// </summary>
public sealed record TerminalProcessLaunchResult(bool Succeeded, string UserSafeMessage)
{
    public static TerminalProcessLaunchResult Success() => new(Succeeded: true, string.Empty);

    public static TerminalProcessLaunchResult Failure(string userSafeMessage) => new(Succeeded: false, userSafeMessage);
}
