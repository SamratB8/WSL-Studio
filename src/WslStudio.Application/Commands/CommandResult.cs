namespace WslStudio.Application.Commands;

public sealed record CommandResult(
    string CommandName,
    IReadOnlyList<string> Arguments,
    string StandardOutput,
    string StandardError,
    int? ExitCode,
    TimeSpan Duration,
    bool Succeeded,
    bool TimedOut,
    string UserSafeErrorMessage)
{
    public static CommandResult Success(
        string commandName,
        IReadOnlyList<string> arguments,
        string standardOutput,
        string standardError,
        int exitCode,
        TimeSpan duration)
    {
        return new CommandResult(
            commandName,
            arguments,
            standardOutput,
            standardError,
            exitCode,
            duration,
            Succeeded: exitCode == 0,
            TimedOut: false,
            UserSafeErrorMessage: exitCode == 0
                ? string.Empty
                : "WSL returned an error while running the requested command.");
    }

    public static CommandResult Timeout(
        string commandName,
        IReadOnlyList<string> arguments,
        TimeSpan duration)
    {
        return new CommandResult(
            commandName,
            arguments,
            string.Empty,
            string.Empty,
            ExitCode: null,
            duration,
            Succeeded: false,
            TimedOut: true,
            UserSafeErrorMessage: "The WSL command timed out.");
    }

    public static CommandResult Failure(
        string commandName,
        IReadOnlyList<string> arguments,
        string userSafeErrorMessage,
        TimeSpan duration,
        string standardError = "")
    {
        return new CommandResult(
            commandName,
            arguments,
            string.Empty,
            standardError,
            ExitCode: null,
            duration,
            Succeeded: false,
            TimedOut: false,
            userSafeErrorMessage);
    }
}
