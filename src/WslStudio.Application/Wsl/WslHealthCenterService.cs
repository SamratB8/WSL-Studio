using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed class WslHealthCenterService(
    IWslDistributionDiscoveryService distributionDiscoveryService,
    IWslStatusParser statusParser,
    IWslVersionParser versionParser) : IWslHealthCenterService
{
    public async Task<WslHealthCenterResult> GetHealthAsync(CancellationToken cancellationToken)
    {
        List<WslHealthCheck> checks = [];

        WslDistributionDiscoveryResult distributionsResult =
            await distributionDiscoveryService.GetDistributionsAsync(cancellationToken);

        if (!distributionsResult.Succeeded)
        {
            checks.Add(CreateCheck(
                "WSL executable available",
                WslHealthCheckCategory.Wsl,
                WslHealthCheckStatus.Error,
                "WSL could not be queried.",
                distributionsResult.UserSafeMessage,
                "Confirm WSL is installed and available, then refresh Health Center."));
            checks.Add(CreateCheck(
                "No command failures during discovery",
                WslHealthCheckCategory.System,
                WslHealthCheckStatus.Error,
                "Distribution discovery failed.",
                distributionsResult.UserSafeMessage,
                "Review the error and confirm WSL is installed and available."));

            AddReadOnlyUnknownChecks(checks);
            return WslHealthCenterResult.Success(checks);
        }

        IReadOnlyList<WslDistribution> distributions = distributionsResult.Distributions;
        checks.Add(CreateCheck(
            "WSL executable available",
            WslHealthCheckCategory.Wsl,
            WslHealthCheckStatus.Healthy,
            "WSL responded to read-only discovery.",
            "`wsl.exe --list --verbose` completed successfully.",
            "No action is needed."));
        checks.Add(CreateCheck(
            "No command failures during discovery",
            WslHealthCheckCategory.System,
            WslHealthCheckStatus.Healthy,
            "Distribution discovery completed.",
            "WSL Studio retrieved distribution data without modifying WSL state.",
            "No action is needed."));

        AddDistributionChecks(checks, distributions);
        AddDockerCheck(checks, distributions);

        WslEnvironmentResult environmentResult =
            await distributionDiscoveryService.GetEnvironmentInfoAsync(cancellationToken);

        if (!environmentResult.Succeeded || environmentResult.EnvironmentInfo is null)
        {
            AddEnvironmentUnavailableChecks(checks, environmentResult.UserSafeMessage);
            AddReadOnlyWindowsUnknownChecks(checks);
            return WslHealthCenterResult.Success(checks);
        }

        WslStatusParseResult statusResult =
            statusParser.Parse(environmentResult.EnvironmentInfo.StatusOutput ?? string.Empty);
        WslVersionParseResult versionResult =
            versionParser.Parse(environmentResult.EnvironmentInfo.VersionOutput ?? string.Empty);

        WslStatusInfo? statusInfo = statusResult.Succeeded ? statusResult.StatusInfo : null;
        WslVersionInfo? versionInfo = versionResult.Succeeded ? versionResult.VersionInfo : null;

        AddStatusChecks(checks, statusInfo);
        AddVersionChecks(checks, versionInfo);
        AddReadOnlyWindowsUnknownChecks(checks);

        return WslHealthCenterResult.Success(checks);
    }

    private static void AddDistributionChecks(List<WslHealthCheck> checks, IReadOnlyList<WslDistribution> distributions)
    {
        checks.Add(CreateCheck(
            "At least one distribution installed",
            WslHealthCheckCategory.Distributions,
            distributions.Count > 0 ? WslHealthCheckStatus.Healthy : WslHealthCheckStatus.Warning,
            distributions.Count > 0 ? "WSL distributions were found." : "No WSL distributions were found.",
            distributions.Count > 0
                ? $"{distributions.Count} distribution(s) were reported by WSL."
                : "WSL is available, but no installed distributions were reported.",
            distributions.Count > 0
                ? "No action is needed."
                : "Install a distribution before using distribution management features.",
            metadata: new Dictionary<string, string>
            {
                ["DistributionCount"] = distributions.Count.ToString()
            }));

        WslDistribution? defaultDistribution = distributions.FirstOrDefault(distribution => distribution.IsDefault);
        checks.Add(CreateCheck(
            "Default distribution configured",
            WslHealthCheckCategory.Distributions,
            defaultDistribution is not null ? WslHealthCheckStatus.Healthy : WslHealthCheckStatus.Warning,
            defaultDistribution is not null ? "A default distribution is configured." : "No default distribution was reported.",
            defaultDistribution is not null
                ? $"{defaultDistribution.Name.Value} is marked as the default distribution."
                : "WSL did not mark any listed distribution as default.",
            defaultDistribution is not null
                ? "No action is needed."
                : "Set a default distribution with official WSL tooling if one is required."));
    }

    private static void AddDockerCheck(List<WslHealthCheck> checks, IReadOnlyList<WslDistribution> distributions)
    {
        bool dockerDesktopDetected = distributions.Any(distribution =>
            string.Equals(distribution.Name.Value, "docker-desktop", StringComparison.OrdinalIgnoreCase));

        checks.Add(CreateCheck(
            "Docker Desktop WSL distribution",
            WslHealthCheckCategory.Docker,
            dockerDesktopDetected ? WslHealthCheckStatus.Healthy : WslHealthCheckStatus.Unknown,
            dockerDesktopDetected ? "Docker Desktop WSL integration was detected." : "Docker Desktop WSL integration was not detected.",
            dockerDesktopDetected
                ? "`docker-desktop` appears in the WSL distribution list."
                : "This check only detects the optional `docker-desktop` distribution when it appears in WSL.",
            dockerDesktopDetected
                ? "No action is needed."
                : "No action is required unless Docker Desktop integration is expected."));
    }

    private static void AddStatusChecks(List<WslHealthCheck> checks, WslStatusInfo? statusInfo)
    {
        checks.Add(CreateCheck(
            "Default WSL version available",
            WslHealthCheckCategory.Wsl,
            statusInfo?.DefaultVersion is not null ? WslHealthCheckStatus.Healthy : WslHealthCheckStatus.Unknown,
            statusInfo?.DefaultVersion is not null ? "Default WSL version is available." : "Default WSL version was not reported.",
            statusInfo?.DefaultVersion is int defaultVersion
                ? $"Default Version: {defaultVersion}"
                : "`wsl.exe --status` did not report a default version.",
            statusInfo?.DefaultVersion is not null
                ? "No action is needed."
                : "Use official WSL tooling to verify default version if this value is needed."));
    }

    private static void AddVersionChecks(List<WslHealthCheck> checks, WslVersionInfo? versionInfo)
    {
        checks.Add(CreateCheck(
            "WSL version available",
            WslHealthCheckCategory.Wsl,
            string.IsNullOrWhiteSpace(versionInfo?.WslVersion) ? WslHealthCheckStatus.Unknown : WslHealthCheckStatus.Healthy,
            string.IsNullOrWhiteSpace(versionInfo?.WslVersion) ? "WSL version was not reported." : "WSL version is available.",
            string.IsNullOrWhiteSpace(versionInfo?.WslVersion)
                ? "`wsl.exe --version` did not report a WSL version."
                : $"WSL version: {versionInfo.WslVersion}",
            "Use the version information when reporting compatibility issues."));

        string? kernelVersion = versionInfo?.KernelVersion;
        checks.Add(CreateCheck(
            "Kernel version available",
            WslHealthCheckCategory.Wsl,
            string.IsNullOrWhiteSpace(kernelVersion) ? WslHealthCheckStatus.Unknown : WslHealthCheckStatus.Healthy,
            string.IsNullOrWhiteSpace(kernelVersion) ? "Kernel version was not reported." : "Kernel version is available.",
            string.IsNullOrWhiteSpace(kernelVersion)
                ? "`wsl.exe --version` did not report a kernel version."
                : $"Kernel version: {kernelVersion}",
            "Use the kernel version when troubleshooting WSL compatibility."));

        string? wslgVersion = versionInfo?.WslgVersion;
        checks.Add(CreateCheck(
            "WSLg availability",
            WslHealthCheckCategory.Wslg,
            string.IsNullOrWhiteSpace(wslgVersion) ? WslHealthCheckStatus.Unknown : WslHealthCheckStatus.Healthy,
            string.IsNullOrWhiteSpace(wslgVersion) ? "WSLg was not reported." : "WSLg version is available.",
            string.IsNullOrWhiteSpace(wslgVersion)
                ? "`wsl.exe --version` did not report a WSLg version."
                : $"WSLg version: {wslgVersion}",
            string.IsNullOrWhiteSpace(wslgVersion)
                ? "No action is required unless Linux GUI application support is expected."
                : "No action is needed."));
    }

    private static void AddEnvironmentUnavailableChecks(List<WslHealthCheck> checks, string message)
    {
        string details = string.IsNullOrWhiteSpace(message)
            ? "WSL status and version information could not be retrieved."
            : message;

        checks.Add(CreateCheck(
            "WSL version available",
            WslHealthCheckCategory.Wsl,
            WslHealthCheckStatus.Unknown,
            "WSL version could not be verified.",
            details,
            "Confirm `wsl.exe --version` is supported by the installed WSL version."));
        checks.Add(CreateCheck(
            "Kernel version available",
            WslHealthCheckCategory.Wsl,
            WslHealthCheckStatus.Unknown,
            "Kernel version could not be verified.",
            details,
            "Confirm WSL version information is available."));
        checks.Add(CreateCheck(
            "Default WSL version available",
            WslHealthCheckCategory.Wsl,
            WslHealthCheckStatus.Unknown,
            "Default WSL version could not be verified.",
            details,
            "Confirm `wsl.exe --status` is supported by the installed WSL version."));
        checks.Add(CreateCheck(
            "WSLg availability",
            WslHealthCheckCategory.Wslg,
            WslHealthCheckStatus.Unknown,
            "WSLg availability could not be verified.",
            details,
            "No action is required unless Linux GUI application support is expected."));
    }

    private static void AddReadOnlyUnknownChecks(List<WslHealthCheck> checks)
    {
        AddEnvironmentUnavailableChecks(checks, "WSL discovery did not complete.");
        AddReadOnlyWindowsUnknownChecks(checks);
    }

    private static void AddReadOnlyWindowsUnknownChecks(List<WslHealthCheck> checks)
    {
        checks.Add(CreateCheck(
            "Windows optional features",
            WslHealthCheckCategory.WindowsFeatures,
            WslHealthCheckStatus.Unknown,
            "Windows feature state was not checked.",
            "This version does not query Windows optional feature state.",
            "Use Windows Settings or official Windows tooling if feature state needs to be verified."));
        checks.Add(CreateCheck(
            "Virtualization availability",
            WslHealthCheckCategory.Virtualization,
            WslHealthCheckStatus.Unknown,
            "Virtualization state was not checked.",
            "This version does not query firmware or virtualization settings.",
            "Use Windows Task Manager or official Windows tooling if virtualization state needs to be verified."));
        checks.Add(CreateCheck(
            "Networking diagnostics",
            WslHealthCheckCategory.Networking,
            WslHealthCheckStatus.Unknown,
            "Networking was not checked.",
            "No network checks are performed in this read-only health pass.",
            "Review WSL networking manually if network behavior is not working as expected."));
    }

    private static WslHealthCheck CreateCheck(
        string name,
        WslHealthCheckCategory category,
        WslHealthCheckStatus status,
        string summary,
        string details,
        string recommendation,
        Uri? documentationLink = null,
        IReadOnlyDictionary<string, string>? metadata = null)
    {
        return new WslHealthCheck(
            name,
            category,
            status,
            summary,
            details,
            recommendation,
            documentationLink,
            metadata);
    }
}
