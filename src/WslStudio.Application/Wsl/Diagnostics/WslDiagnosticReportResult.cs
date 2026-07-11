namespace WslStudio.Application.Wsl.Diagnostics;

/// <summary>
/// Result of generating the consolidated diagnostic report, following the project's success/failure
/// result shape.
/// </summary>
public sealed record WslDiagnosticReportResult(
    WslDiagnosticReport? Report,
    bool Succeeded,
    string UserSafeMessage)
{
    public static WslDiagnosticReportResult Success(WslDiagnosticReport report)
    {
        return new WslDiagnosticReportResult(report, Succeeded: true, string.Empty);
    }

    public static WslDiagnosticReportResult Failure(string userSafeMessage)
    {
        return new WslDiagnosticReportResult(null, Succeeded: false, userSafeMessage);
    }
}
