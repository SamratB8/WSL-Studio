namespace WslStudio.Application.Wsl.Diagnostics;

/// <summary>
/// Shared, non-sensitive text used across diagnostic report formats.
/// </summary>
public static class DiagnosticReportText
{
    public const string PrivacyNotice =
        "This report is intended to be safe to share. It excludes usernames, home and file paths, " +
        "environment variables, secrets, machine identifiers, and raw command output. Review the " +
        "contents before sharing to confirm no sensitive information is present.";
}
