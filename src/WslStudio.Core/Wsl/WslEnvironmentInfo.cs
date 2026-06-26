namespace WslStudio.Core.Wsl;

public sealed record WslEnvironmentInfo(
    string? StatusOutput,
    string? VersionOutput,
    string? UserSafeMessage);
