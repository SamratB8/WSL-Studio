using System.Collections.ObjectModel;
using WslStudio.Core.Wsl;

namespace WslStudio.App.ViewModels;

public sealed class HealthCheckGroupViewModel(
    WslHealthCheckCategory category,
    IEnumerable<WslHealthCheck> checks)
{
    public string Category { get; } = ToCategoryText(category);

    public ObservableCollection<HealthCheckItemViewModel> Checks { get; } =
        new(checks.Select(check => new HealthCheckItemViewModel(check)));

    private static string ToCategoryText(WslHealthCheckCategory category)
    {
        return category switch
        {
            WslHealthCheckCategory.Wsl => "WSL",
            WslHealthCheckCategory.WindowsFeatures => "Windows Features",
            WslHealthCheckCategory.Virtualization => "Virtualization",
            WslHealthCheckCategory.Distributions => "Distributions",
            WslHealthCheckCategory.Networking => "Networking",
            WslHealthCheckCategory.Docker => "Docker",
            WslHealthCheckCategory.Wslg => "WSLg",
            WslHealthCheckCategory.System => "System",
            _ => "Other"
        };
    }
}
