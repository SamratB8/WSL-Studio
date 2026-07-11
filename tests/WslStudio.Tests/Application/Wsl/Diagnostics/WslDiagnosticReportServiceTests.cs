using WslStudio.Application.Wsl;
using WslStudio.Application.Wsl.Diagnostics;
using WslStudio.Core.Wsl;

namespace WslStudio.Tests.Application.Wsl.Diagnostics;

public sealed class WslDiagnosticReportServiceTests
{
    private static readonly DateTimeOffset FixedTime = new(2026, 7, 11, 14, 30, 0, TimeSpan.Zero);

    [Fact]
    public async Task GenerateReportAsync_ComposesEnvironmentAndHealth()
    {
        WslDiagnosticReportService service = CreateService(SampleEnvironment(), SampleHealth());

        WslDiagnosticReportResult result = await service.GenerateReportAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Report);

        WslDiagnosticReport report = result.Report;
        Assert.Equal("1.0", report.SchemaVersion);
        Assert.Equal(FixedTime, report.GeneratedAtUtc);

        Assert.Equal("10.0.26300.8758", report.Environment.WindowsVersion);
        Assert.Equal("2.9.3.0", report.Environment.WslVersion);
        Assert.Equal("6.18.35.2-1", report.Environment.KernelVersion);
        Assert.Equal("1.0.79", report.Environment.WslgVersion);
        Assert.Equal(2, report.Environment.DefaultWslVersion);
        Assert.Equal("1.611.1-81528511", report.Environment.Direct3DVersion);
        Assert.Equal("10.0.26100.1-240331-1435.ge-release", report.Environment.DxCoreVersion);
        Assert.Equal("1.2.7214", report.Environment.MsrdcVersion);
        Assert.Equal(2, report.Environment.InstalledDistributionCount);
        Assert.Equal(1, report.Environment.RunningDistributionCount);
        Assert.Equal("Ubuntu", report.Environment.DefaultDistribution);
        Assert.True(report.Environment.DockerDesktopDetected);
        Assert.True(report.Environment.WslgAvailable);

        Assert.Equal(2, report.Distributions.Count);
        Assert.Contains(report.Distributions, d => d is { Name: "Ubuntu", State: "Running", IsDefault: true });
        Assert.Contains(report.Distributions, d => d is { Name: "docker-desktop", State: "Stopped", IsDefault: false });

        Assert.Equal(1, report.HealthSummary.HealthyCount);
        Assert.Equal(1, report.HealthSummary.WarningCount);
        Assert.Equal(1, report.HealthSummary.ErrorCount);
        Assert.Equal(1, report.HealthSummary.NotCheckedCount);
        Assert.Equal(4, report.HealthChecks.Count);

        Assert.Equal(
            ["wsl.exe --list --verbose", "wsl.exe --status", "wsl.exe --version"],
            report.Sources);
    }

    [Fact]
    public async Task GenerateReportAsync_PropagatesEnvironmentFailure()
    {
        WslDiagnosticReportService service = CreateService(
            WslEnvironmentOverviewResult.Failure("WSL is not installed or wsl.exe could not be found."),
            SampleHealth());

        WslDiagnosticReportResult result = await service.GenerateReportAsync(CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Null(result.Report);
        Assert.Equal("WSL is not installed or wsl.exe could not be found.", result.UserSafeMessage);
    }

    [Fact]
    public async Task GenerateReportAsync_ExcludesRawHealthCheckDetailsAndMetadata()
    {
        WslHealthCheck sensitiveCheck = new(
            "Sample check",
            WslHealthCheckCategory.Wsl,
            WslHealthCheckStatus.Healthy,
            "Safe summary.",
            "SENSITIVE-DETAILS-XYZ",
            "Safe recommendation.",
            new Uri("https://example.com/SENSITIVE-LINK"),
            new Dictionary<string, string> { ["key"] = "SENSITIVE-META-XYZ" });

        WslDiagnosticReportService service = CreateService(
            SampleEnvironment(),
            WslHealthCenterResult.Success([sensitiveCheck]));

        WslDiagnosticReportResult result = await service.GenerateReportAsync(CancellationToken.None);

        Assert.True(result.Succeeded);
        DiagnosticHealthCheckInfo mapped = Assert.Single(result.Report!.HealthChecks);
        Assert.Equal("Safe summary.", mapped.Summary);
        Assert.Equal("Safe recommendation.", mapped.Recommendation);

        foreach (DiagnosticReportFormat format in Enum.GetValues<DiagnosticReportFormat>())
        {
            string text = service.CreateContent(result.Report, format).Text;
            Assert.DoesNotContain("SENSITIVE-DETAILS-XYZ", text);
            Assert.DoesNotContain("SENSITIVE-META-XYZ", text);
            Assert.DoesNotContain("SENSITIVE-LINK", text);
        }
    }

    [Theory]
    [InlineData(DiagnosticReportFormat.Markdown, "wsl-studio-diagnostic-report-20260711-1430.md", ".md")]
    [InlineData(DiagnosticReportFormat.Text, "wsl-studio-diagnostic-report-20260711-1430.txt", ".txt")]
    [InlineData(DiagnosticReportFormat.Json, "wsl-studio-diagnostic-report-20260711-1430.json", ".json")]
    public async Task CreateContent_ProducesFormatSpecificFileNameAndExtension(
        DiagnosticReportFormat format, string expectedFileName, string expectedExtension)
    {
        WslDiagnosticReportService service = CreateService(SampleEnvironment(), SampleHealth());
        WslDiagnosticReport report = (await service.GenerateReportAsync(CancellationToken.None)).Report!;

        DiagnosticReportContent content = service.CreateContent(report, format);

        Assert.Equal(expectedFileName, content.FileName);
        Assert.Equal(expectedExtension, content.FileExtension);
        Assert.False(string.IsNullOrWhiteSpace(content.Text));
    }

    private static WslDiagnosticReportService CreateService(
        WslEnvironmentOverviewResult environmentResult,
        WslHealthCenterResult healthResult)
    {
        IDiagnosticReportFormatter[] formatters =
        [
            new MarkdownDiagnosticReportFormatter(),
            new TextDiagnosticReportFormatter(),
            new JsonDiagnosticReportFormatter()
        ];

        return new WslDiagnosticReportService(
            new FakeEnvironmentService(environmentResult),
            new FakeHealthService(healthResult),
            formatters,
            new FixedTimeProvider(FixedTime));
    }

    private static WslEnvironmentOverviewResult SampleEnvironment()
    {
        WslVersionInfo version = new(
            "2.9.3.0", "6.18.35.2-1", "1.0.79", "10.0.26300.8758",
            "1.611.1-81528511", "10.0.26100.1-240331-1435.ge-release", "1.2.7214");
        WslStatusInfo status = new(Name("Ubuntu"), 2, "6.18.35.2-1");

        WslDashboardOverview overview = new(
            DefaultDistribution: Name("Ubuntu"),
            RunningDistributionCount: 1,
            StoppedDistributionCount: 1,
            TotalDistributionCount: 2,
            StatusInfo: status,
            VersionInfo: version,
            Distributions:
            [
                Dist("Ubuntu", WslDistributionState.Running, isDefault: true),
                Dist("docker-desktop", WslDistributionState.Stopped)
            ]);

        return WslEnvironmentOverviewResult.Success(
            new WslEnvironmentOverview(overview, DockerDesktopDetected: true, WslgAvailable: true));
    }

    private static WslHealthCenterResult SampleHealth()
    {
        return WslHealthCenterResult.Success(
        [
            Check("Healthy check", WslHealthCheckStatus.Healthy),
            Check("Warning check", WslHealthCheckStatus.Warning),
            Check("Error check", WslHealthCheckStatus.Error),
            Check("Unknown check", WslHealthCheckStatus.Unknown)
        ]);
    }

    private static WslHealthCheck Check(string name, WslHealthCheckStatus status) =>
        new(name, WslHealthCheckCategory.Wsl, status, $"{name} summary.", $"{name} details.", $"{name} recommendation.");

    private static WslDistribution Dist(string name, WslDistributionState state, bool isDefault = false) =>
        new(Name(name), state, Version: 2, isDefault);

    private static DistributionName Name(string value)
    {
        Assert.True(DistributionName.TryCreate(value, out DistributionName? name));
        return name!;
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private sealed class FakeEnvironmentService(WslEnvironmentOverviewResult result) : IWslEnvironmentService
    {
        public Task<WslEnvironmentOverviewResult> GetEnvironmentAsync(CancellationToken cancellationToken) =>
            Task.FromResult(result);
    }

    private sealed class FakeHealthService(WslHealthCenterResult result) : IWslHealthCenterService
    {
        public Task<WslHealthCenterResult> GetHealthAsync(CancellationToken cancellationToken) =>
            Task.FromResult(result);
    }
}
