namespace WslStudio.Tests.Application.Wsl;

internal static class WslStatusAndVersionFixtures
{
    public const string StatusOutput = """
    Default Distribution: Ubuntu
    Default Version: 2
    Kernel version: 5.15.167.4-1
    """;

    public const string VersionOutput = """
    WSL version: 2.4.13.0
    Kernel version: 5.15.167.4-1
    WSLg version: 1.0.65
    Windows version: 10.0.26100.0
    """;
}
