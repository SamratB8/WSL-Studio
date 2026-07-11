using WslStudio.Core.Wsl;

namespace WslStudio.App.ViewModels;

public sealed class DistributionListItemViewModel(WslDistribution distribution)
{
    public DistributionName DistributionName { get; } = distribution.Name;

    public string Name { get; } = distribution.Name.Value;

    public string State { get; } = ToDisplayText(distribution.State);

    public string Version { get; } = distribution.Version?.ToString() ?? "Unknown";

    public bool IsDefault { get; } = distribution.IsDefault;

    public string DefaultStatus { get; } = distribution.IsDefault ? "Default" : "No";

    private static string ToDisplayText(WslDistributionState state)
    {
        return state switch
        {
            WslDistributionState.Running => "Running",
            WslDistributionState.Stopped => "Stopped",
            WslDistributionState.Installing => "Installing",
            WslDistributionState.Converting => "Converting",
            _ => "Unknown"
        };
    }
}
