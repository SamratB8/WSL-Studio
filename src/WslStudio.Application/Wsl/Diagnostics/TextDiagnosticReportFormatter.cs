using System.Globalization;
using System.Text;

namespace WslStudio.Application.Wsl.Diagnostics;

/// <summary>
/// Renders the diagnostic report as plain text that stays readable in Notepad and terminals.
/// </summary>
public sealed class TextDiagnosticReportFormatter : IDiagnosticReportFormatter
{
    private const string NotReported = "Not reported";

    public DiagnosticReportFormat Format => DiagnosticReportFormat.Text;

    public string FileExtension => ".txt";

    public string FileTypeDescription => "Plain text document";

    public string Render(WslDiagnosticReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        DiagnosticEnvironmentInfo environment = report.Environment;
        StringBuilder builder = new();

        builder.AppendLine("WSL Studio Diagnostic Report");
        builder.AppendLine("============================");
        builder.AppendLine();

        AppendHeading(builder, "Report Information");
        builder.AppendLine($"Schema version    : {report.SchemaVersion}");
        builder.AppendLine($"Generated (UTC)   : {FormatTimestamp(report.GeneratedAtUtc)}");
        builder.AppendLine($"Windows version   : {Value(environment.WindowsVersion)}");
        builder.AppendLine($"WSL version       : {Value(environment.WslVersion)}");
        builder.AppendLine($"Kernel version    : {Value(environment.KernelVersion)}");
        builder.AppendLine($"WSLg version      : {Value(environment.WslgVersion)}");
        builder.AppendLine($"Default WSL version: {Value(environment.DefaultWslVersion)}");
        builder.AppendLine();

        AppendHeading(builder, "WSL Environment");
        builder.AppendLine($"Installed distributions: {environment.InstalledDistributionCount}");
        builder.AppendLine($"Running distributions  : {environment.RunningDistributionCount}");
        builder.AppendLine($"Default distribution   : {Value(environment.DefaultDistribution)}");
        builder.AppendLine($"Docker Desktop detected: {YesNo(environment.DockerDesktopDetected)}");
        builder.AppendLine($"WSLg available         : {YesNo(environment.WslgAvailable)}");
        builder.AppendLine($"Direct3D version       : {Value(environment.Direct3DVersion)}");
        builder.AppendLine($"DXCore version         : {Value(environment.DxCoreVersion)}");
        builder.AppendLine($"MSRDC version          : {Value(environment.MsrdcVersion)}");
        builder.AppendLine();

        AppendHeading(builder, "Distributions");
        if (report.Distributions.Count == 0)
        {
            builder.AppendLine("No distributions were reported by WSL.");
        }
        else
        {
            foreach (DiagnosticDistributionInfo distribution in report.Distributions)
            {
                builder.AppendLine(
                    $"- {distribution.Name} | State: {distribution.State} | WSL {Value(distribution.WslVersion)} | Default: {YesNo(distribution.IsDefault)}");
            }
        }

        builder.AppendLine();

        AppendHeading(builder, "Health Summary");
        builder.AppendLine($"Overall status: {report.HealthSummary.OverallStatus}");
        builder.AppendLine($"Healthy: {report.HealthSummary.HealthyCount}   " +
            $"Warning: {report.HealthSummary.WarningCount}   " +
            $"Error: {report.HealthSummary.ErrorCount}   " +
            $"Not checked: {report.HealthSummary.NotCheckedCount}");
        builder.AppendLine($"Recommendation: {report.HealthSummary.Recommendation}");
        builder.AppendLine();

        AppendHeading(builder, "Health Checks");
        if (report.HealthChecks.Count == 0)
        {
            builder.AppendLine("No health checks were produced.");
        }
        else
        {
            foreach (DiagnosticHealthCheckInfo check in report.HealthChecks)
            {
                builder.AppendLine($"[{check.Status}] {check.Name} ({check.Category})");
                builder.AppendLine($"    Summary       : {check.Summary}");
                builder.AppendLine($"    Recommendation: {check.Recommendation}");
            }
        }

        builder.AppendLine();

        AppendHeading(builder, "Data Sources");
        foreach (string source in report.Sources)
        {
            builder.AppendLine($"- {source}");
        }

        builder.AppendLine();

        AppendHeading(builder, "Privacy Notice");
        builder.AppendLine(DiagnosticReportText.PrivacyNotice);

        return builder.ToString();
    }

    private static void AppendHeading(StringBuilder builder, string heading)
    {
        builder.AppendLine(heading);
        builder.AppendLine(new string('-', heading.Length));
    }

    private static string FormatTimestamp(DateTimeOffset value) =>
        value.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss 'UTC'", CultureInfo.InvariantCulture);

    private static string Value(string? value) => string.IsNullOrWhiteSpace(value) ? NotReported : value;

    private static string Value(int? value) => value?.ToString(CultureInfo.InvariantCulture) ?? NotReported;

    private static string YesNo(bool value) => value ? "Yes" : "No";
}
