namespace WslStudio.Application.Commands;

public sealed record CommandRequest(
    string CommandName,
    IReadOnlyList<string> Arguments,
    TimeSpan Timeout);
