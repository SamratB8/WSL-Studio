namespace WslStudio.Application.Wsl.Terminal;

/// <summary>
/// Structured outcome of an interactive terminal launch. Raw exceptions are never surfaced to
/// ViewModels; failures are reported through <see cref="UserSafeMessage"/>.
/// </summary>
public sealed record WslTerminalLaunchResult(
    bool Succeeded,
    TerminalLaunchMethod Method,
    string UserSafeMessage)
{
    public static WslTerminalLaunchResult Success(TerminalLaunchMethod method) =>
        new(Succeeded: true, method, string.Empty);

    public static WslTerminalLaunchResult Failure(string userSafeMessage) =>
        new(Succeeded: false, TerminalLaunchMethod.None, userSafeMessage);
}
