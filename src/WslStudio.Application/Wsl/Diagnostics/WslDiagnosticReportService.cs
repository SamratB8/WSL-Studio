using System.Globalization;
using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl.Diagnostics;

/// <summary>
/// Composes the existing read-only <see cref="IWslEnvironmentService"/> and
/// <see cref="IWslHealthCenterService"/> into a single, non-sensitive diagnostic report snapshot, and
/// renders it into the requested format using the registered formatters. It runs no WSL commands
/// itself and reuses the existing discovery, parsing, and health logic rather than duplicating it.
/// </summary>
public sealed class WslDiagnosticReportService(
    IWslEnvironmentService environmentService,
    IWslHealthCenterService healthCenterService,
    IEnumerable<IDiagnosticReportFormatter> formatters,
    TimeProvider timeProvider) : IWslDiagnosticReportService
{
    private const string SchemaVersion = "1.0";

    private static readonly IReadOnlyList<string> ReportSources =
    [
        "wsl.exe --list --verbose",
        "wsl.exe --status",
        "wsl.exe --version"
    ];

    public async Task<WslDiagnosticReportResult> GenerateReportAsync(CancellationToken cancellationToken)
    {
        WslEnvironmentOverviewResult environmentResult =
            await environmentService.GetEnvironmentAsync(cancellationToken);

        if (!environmentResult.Succeeded || environmentResult.Environment is null)
        {
            return WslDiagnosticReportResult.Failure(environmentResult.UserSafeMessage);
        }

        WslHealthCenterResult healthResult = await healthCenterService.GetHealthAsync(cancellationToken);

        if (!healthResult.Succeeded)
        {
            return WslDiagnosticReportResult.Failure(healthResult.UserSafeMessage);
        }

        WslDiagnosticReport report = BuildReport(environmentResult.Environment, healthResult);
        return WslDiagnosticReportResult.Success(report);
    }

    public DiagnosticReportContent CreateContent(WslDiagnosticReport report, DiagnosticReportFormat format)
    {
        ArgumentNullException.ThrowIfNull(report);

        IDiagnosticReportFormatter formatter = formatters.FirstOrDefault(candidate => candidate.Format == format)
            ?? throw new ArgumentOutOfRangeException(nameof(format), format, "No formatter is registered for the requested format.");

        string text = formatter.Render(report);
        string timestamp = report.GeneratedAtUtc.UtcDateTime.ToString("yyyyMMdd-HHmm", CultureInfo.InvariantCulture);
        string fileName = $"wsl-studio-diagnostic-report-{timestamp}{formatter.FileExtension}";

        return new DiagnosticReportContent(fileName, formatter.FileExtension, formatter.FileTypeDescription, text);
    }

    private WslDiagnosticReport BuildReport(WslEnvironmentOverview environment, WslHealthCenterResult healthResult)
    {
        WslDashboardOverview overview = environment.Overview;
        WslVersionInfo? version = overview.VersionInfo;
        WslStatusInfo? status = overview.StatusInfo;

        DiagnosticEnvironmentInfo environmentInfo = new(
            WindowsVersion: version?.WindowsVersion,
            WslVersion: version?.WslVersion,
            KernelVersion: version?.KernelVersion ?? status?.KernelVersion,
            WslgVersion: version?.WslgVersion,
            DefaultWslVersion: status?.DefaultVersion,
            InstalledDistributionCount: overview.TotalDistributionCount,
            RunningDistributionCount: overview.RunningDistributionCount,
            DefaultDistribution: overview.DefaultDistribution?.Value,
            DockerDesktopDetected: environment.DockerDesktopDetected,
            WslgAvailable: environment.WslgAvailable,
            Direct3DVersion: version?.Direct3DVersion,
            DxCoreVersion: version?.DxCoreVersion,
            MsrdcVersion: version?.MsrdcVersion);

        List<DiagnosticDistributionInfo> distributions = overview.Distributions
            .Select(distribution => new DiagnosticDistributionInfo(
                distribution.Name.Value,
                distribution.State.ToString(),
                distribution.Version,
                distribution.IsDefault))
            .ToList();

        DiagnosticHealthSummary healthSummary = new(
            healthResult.Summary.OverallStatus,
            healthResult.Summary.HealthyCount,
            healthResult.Summary.WarningCount,
            healthResult.Summary.ErrorCount,
            healthResult.Summary.UnknownCount,
            healthResult.Summary.Recommendation);

        List<DiagnosticHealthCheckInfo> healthChecks = healthResult.Checks
            .Select(check => new DiagnosticHealthCheckInfo(
                check.Name,
                check.Category.ToString(),
                check.Status.ToString(),
                check.Summary,
                check.Recommendation))
            .ToList();

        return new WslDiagnosticReport(
            SchemaVersion,
            timeProvider.GetUtcNow(),
            environmentInfo,
            distributions,
            healthSummary,
            healthChecks,
            ReportSources);
    }
}
