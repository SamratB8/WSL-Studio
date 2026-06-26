namespace WslStudio.Core.Wsl;

public sealed record WslVersionInfo(
    string? WslVersion,
    string? KernelVersion,
    string? WslgVersion,
    string? WindowsVersion);
