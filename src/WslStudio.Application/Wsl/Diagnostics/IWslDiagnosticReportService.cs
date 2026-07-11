namespace WslStudio.Application.Wsl.Diagnostics;

/// <summary>
/// Builds a read-only diagnostic report by composing existing WSL overview services, and renders it
/// into a chosen portable format. Performs no file I/O and no UI work.
/// </summary>
public interface IWslDiagnosticReportService
{
    Task<WslDiagnosticReportResult> GenerateReportAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Renders the report in the requested format and returns the content together with the file name,
    /// extension, and description needed by the App layer to save it.
    /// </summary>
    DiagnosticReportContent CreateContent(WslDiagnosticReport report, DiagnosticReportFormat format);
}
