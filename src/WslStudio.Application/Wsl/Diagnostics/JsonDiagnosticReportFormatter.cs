using System.Text.Json;

namespace WslStudio.Application.Wsl.Diagnostics;

/// <summary>
/// Renders the diagnostic report as indented, valid UTF-8 JSON using <see cref="System.Text.Json"/>.
/// The stable <see cref="WslDiagnosticReport"/> export DTO is serialized directly; no UI or domain
/// types are serialized.
/// </summary>
public sealed class JsonDiagnosticReportFormatter : IDiagnosticReportFormatter
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public DiagnosticReportFormat Format => DiagnosticReportFormat.Json;

    public string FileExtension => ".json";

    public string FileTypeDescription => "JSON document";

    public string Render(WslDiagnosticReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        return JsonSerializer.Serialize(report, SerializerOptions);
    }
}
