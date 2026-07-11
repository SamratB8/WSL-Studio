using WslStudio.Core.Wsl;

namespace WslStudio.App.ViewModels;

public sealed class HealthCheckItemViewModel(WslHealthCheck healthCheck)
{
    public string Name { get; } = healthCheck.Name;

    public string Status { get; } = ToStatusText(healthCheck.Status);

    public string Summary { get; } = healthCheck.Summary;

    public string Details { get; } = healthCheck.Details;

    public string Recommendation { get; } = healthCheck.Recommendation;

    public string DocumentationLink { get; } = healthCheck.DocumentationLink?.ToString() ?? string.Empty;

    private static string ToStatusText(WslHealthCheckStatus status)
    {
        return status switch
        {
            WslHealthCheckStatus.Healthy => "Healthy",
            WslHealthCheckStatus.Warning => "Warning",
            WslHealthCheckStatus.Error => "Error",
            _ => "Not checked"
        };
    }
}
