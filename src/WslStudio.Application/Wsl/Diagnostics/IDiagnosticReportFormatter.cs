namespace WslStudio.Application.Wsl.Diagnostics;

/// <summary>
/// Renders a <see cref="WslDiagnosticReport"/> into a single portable text document. Formatters are
/// pure: they perform no I/O and do not run WSL commands.
/// </summary>
public interface IDiagnosticReportFormatter
{
    DiagnosticReportFormat Format { get; }

    /// <summary>File extension including the leading dot, for example <c>.md</c>.</summary>
    string FileExtension { get; }

    /// <summary>Human-readable file-type description shown in the file save picker.</summary>
    string FileTypeDescription { get; }

    string Render(WslDiagnosticReport report);
}
