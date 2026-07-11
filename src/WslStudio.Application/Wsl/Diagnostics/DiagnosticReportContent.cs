namespace WslStudio.Application.Wsl.Diagnostics;

/// <summary>
/// Rendered report content plus the metadata the App layer needs to save it (suggested file name,
/// extension, and a human-readable file-type description for the file save picker).
/// </summary>
public sealed record DiagnosticReportContent(
    string FileName,
    string FileExtension,
    string FileTypeDescription,
    string Text);
