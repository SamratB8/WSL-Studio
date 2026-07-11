namespace WslStudio.Tests.Application.Wsl;

internal static class WslStatusAndVersionFixtures
{
    public const string StatusOutput = """
    Default Distribution: Ubuntu
    Default Version: 2
    Kernel version: 6.18.35.2-1
    """;

    public const string VersionOutput = """
    WSL version: 2.9.3.0
    Kernel version: 6.18.35.2-1
    WSLg version: 1.0.79
    MSRDC version: 1.2.7214
    Direct3D version: 1.611.1-81528511
    DXCore version: 10.0.26100.1-240331-1435.ge-release
    Windows version: 10.0.26300.8758
    """;

    /// <summary>
    /// A <c>wsl --version</c> output that omits the optional graphics components (Direct3D, DXCore,
    /// MSRDC). Used to verify those fields are reported as unavailable rather than fabricated.
    /// </summary>
    public const string VersionOutputWithoutGraphics = """
    WSL version: 2.9.3.0
    Kernel version: 6.18.35.2-1
    WSLg version: 1.0.79
    Windows version: 10.0.26300.8758
    """;
}
