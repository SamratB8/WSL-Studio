using WslStudio.Application.Wsl;

namespace WslStudio.App.ViewModels;

/// <summary>
/// A selectable state filter paired with its display label for the Distributions toolbar.
/// </summary>
public sealed record DistributionFilterOption(string Label, DistributionStateFilter Value);
