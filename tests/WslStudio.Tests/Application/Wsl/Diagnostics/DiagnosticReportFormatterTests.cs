using System.Text.Json;
using WslStudio.Application.Wsl.Diagnostics;

namespace WslStudio.Tests.Application.Wsl.Diagnostics;

public sealed class MarkdownDiagnosticReportFormatterTests
{
    private readonly MarkdownDiagnosticReportFormatter _formatter = new();

    [Fact]
    public void Render_IncludesAllSectionsInOrder()
    {
        string output = _formatter.Render(DiagnosticReportSamples.FullReport());

        string[] sections =
        [
            "# WSL Studio Diagnostic Report",
            "## Report Information",
            "## WSL Environment",
            "## Distributions",
            "## Health Summary",
            "## Health Checks",
            "## Data Sources",
            "## Privacy Notice"
        ];

        int previousIndex = -1;
        foreach (string section in sections)
        {
            int index = output.IndexOf(section, StringComparison.Ordinal);
            Assert.True(index > previousIndex, $"Section '{section}' is missing or out of order.");
            previousIndex = index;
        }
    }

    [Fact]
    public void Render_IncludesDistributionRowsAndDataSources()
    {
        string output = _formatter.Render(DiagnosticReportSamples.FullReport());

        Assert.Contains("| Ubuntu | Running | 2 | Yes |", output);
        Assert.Contains("| docker-desktop | Stopped | 2 | No |", output);
        Assert.Contains("`wsl.exe --version`", output);
        Assert.Contains("Review the contents before sharing", output);
    }

    [Fact]
    public void Render_ShowsNotReported_ForMissingOptionalFields()
    {
        string output = _formatter.Render(DiagnosticReportSamples.MinimalReport());

        Assert.Contains("- Windows version: Not reported", output);
        Assert.Contains("- Direct3D version: Not reported", output);
        Assert.Contains("- Default WSL version: Not reported", output);
    }

    [Fact]
    public void Render_ShowsEmptyMessage_WhenNoDistributions()
    {
        string output = _formatter.Render(DiagnosticReportSamples.MinimalReport());

        Assert.Contains("No distributions were reported by WSL.", output);
    }
}

public sealed class TextDiagnosticReportFormatterTests
{
    private readonly TextDiagnosticReportFormatter _formatter = new();

    [Fact]
    public void Render_ProducesReadableHeadingsAndContent()
    {
        string output = _formatter.Render(DiagnosticReportSamples.FullReport());

        Assert.Contains("WSL Studio Diagnostic Report", output);
        Assert.Contains("Report Information", output);
        Assert.Contains("Health Checks", output);
        Assert.Contains("[Healthy] Healthy check (Wsl)", output);
        Assert.Contains("- wsl.exe --status", output);
        Assert.Contains("Privacy Notice", output);
        Assert.DoesNotContain("|---", output);
    }

    [Fact]
    public void Render_HandlesNoDistributions()
    {
        string output = _formatter.Render(DiagnosticReportSamples.MinimalReport());

        Assert.Contains("No distributions were reported by WSL.", output);
    }
}

public sealed class JsonDiagnosticReportFormatterTests
{
    private readonly JsonDiagnosticReportFormatter _formatter = new();

    [Fact]
    public void Render_ProducesValidIndentedJsonWithExpectedShape()
    {
        string output = _formatter.Render(DiagnosticReportSamples.FullReport());

        using JsonDocument document = JsonDocument.Parse(output);
        JsonElement root = document.RootElement;

        Assert.Equal("1.0", root.GetProperty("schemaVersion").GetString());
        Assert.True(root.TryGetProperty("generatedAtUtc", out _));
        Assert.Equal(JsonValueKind.Object, root.GetProperty("environment").ValueKind);
        Assert.Equal(JsonValueKind.Array, root.GetProperty("distributions").ValueKind);
        Assert.Equal(JsonValueKind.Object, root.GetProperty("healthSummary").ValueKind);
        Assert.Equal(JsonValueKind.Array, root.GetProperty("healthChecks").ValueKind);
        Assert.Equal(JsonValueKind.Array, root.GetProperty("sources").ValueKind);

        // Indented output.
        Assert.Contains("\n  ", output);
        // camelCase, not PascalCase.
        Assert.DoesNotContain("\"SchemaVersion\"", output);
    }

    [Fact]
    public void Render_SerializesDistributionsAndStatusesAsStrings()
    {
        string output = _formatter.Render(DiagnosticReportSamples.FullReport());

        using JsonDocument document = JsonDocument.Parse(output);
        JsonElement distributions = document.RootElement.GetProperty("distributions");
        Assert.Equal(2, distributions.GetArrayLength());
        Assert.Equal("Ubuntu", distributions[0].GetProperty("name").GetString());
        Assert.Equal("Running", distributions[0].GetProperty("state").GetString());

        JsonElement checks = document.RootElement.GetProperty("healthChecks");
        Assert.Equal("Healthy", checks[0].GetProperty("status").GetString());
    }

    [Fact]
    public void Render_IsDeterministic_ForTheSameReport()
    {
        WslDiagnosticReport report = DiagnosticReportSamples.FullReport();

        Assert.Equal(_formatter.Render(report), _formatter.Render(report));
    }

    [Fact]
    public void Render_ProducesValidJson_WhenReportIsMinimal()
    {
        string output = _formatter.Render(DiagnosticReportSamples.MinimalReport());

        using JsonDocument document = JsonDocument.Parse(output);
        Assert.Equal(0, document.RootElement.GetProperty("distributions").GetArrayLength());
        Assert.Equal(JsonValueKind.Null, document.RootElement.GetProperty("environment").GetProperty("windowsVersion").ValueKind);
    }
}

internal static class DiagnosticReportSamples
{
    private static readonly DateTimeOffset Timestamp = new(2026, 7, 11, 14, 30, 0, TimeSpan.Zero);

    public static WslDiagnosticReport FullReport()
    {
        return new WslDiagnosticReport(
            "1.0",
            Timestamp,
            new DiagnosticEnvironmentInfo(
                "10.0.26300.8758", "2.9.3.0", "6.18.35.2-1", "1.0.79", 2, 2, 1, "Ubuntu", true, true,
                "1.611.1-81528511", "10.0.26100.1-240331-1435.ge-release", "1.2.7214"),
            [
                new DiagnosticDistributionInfo("Ubuntu", "Running", 2, true),
                new DiagnosticDistributionInfo("docker-desktop", "Stopped", 2, false)
            ],
            new DiagnosticHealthSummary("Attention needed", 1, 1, 1, 1, "Review the checks marked as errors."),
            [
                new DiagnosticHealthCheckInfo("Healthy check", "Wsl", "Healthy", "All good.", "No action needed."),
                new DiagnosticHealthCheckInfo("Warning check", "Distributions", "Warning", "Needs review.", "Take a look."),
                new DiagnosticHealthCheckInfo("Error check", "System", "Error", "Failed.", "Fix it."),
                new DiagnosticHealthCheckInfo("Unknown check", "Networking", "Unknown", "Not checked.", "Optional.")
            ],
            ["wsl.exe --list --verbose", "wsl.exe --status", "wsl.exe --version"]);
    }

    public static WslDiagnosticReport MinimalReport()
    {
        return new WslDiagnosticReport(
            "1.0",
            Timestamp,
            new DiagnosticEnvironmentInfo(
                null, null, null, null, null, 0, 0, null, false, false, null, null, null),
            [],
            new DiagnosticHealthSummary("Unavailable", 0, 0, 0, 0, "Try again after WSL is available."),
            [],
            ["wsl.exe --list --verbose", "wsl.exe --status", "wsl.exe --version"]);
    }
}
