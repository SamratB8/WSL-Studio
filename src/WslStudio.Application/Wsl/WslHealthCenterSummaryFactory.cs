using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

internal static class WslHealthCenterSummaryFactory
{
    public static WslHealthCenterSummary Create(IReadOnlyList<WslHealthCheck> checks)
    {
        int healthyCount = checks.Count(check => check.Status == WslHealthCheckStatus.Healthy);
        int warningCount = checks.Count(check => check.Status == WslHealthCheckStatus.Warning);
        int errorCount = checks.Count(check => check.Status == WslHealthCheckStatus.Error);
        int unknownCount = checks.Count(check => check.Status == WslHealthCheckStatus.Unknown);

        if (errorCount > 0)
        {
            return new WslHealthCenterSummary(
                healthyCount,
                warningCount,
                errorCount,
                unknownCount,
                "Attention needed",
                "Review the checks marked as errors before using WSL Studio for management tasks.");
        }

        if (warningCount > 0)
        {
            return new WslHealthCenterSummary(
                healthyCount,
                warningCount,
                errorCount,
                unknownCount,
                "Review recommended",
                "WSL is available, but one or more checks may need attention.");
        }

        if (unknownCount > 0)
        {
            return new WslHealthCenterSummary(
                healthyCount,
                warningCount,
                errorCount,
                unknownCount,
                "Read-only health check complete",
                "Some Windows-side checks are not implemented yet and are listed as not checked. This does not indicate an unhealthy system.");
        }

        return new WslHealthCenterSummary(
            healthyCount,
            warningCount,
            errorCount,
            unknownCount,
            "Healthy",
            "No issues were detected by the available read-only checks.");
    }
}
