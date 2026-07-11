using WslStudio.Application.Wsl;

namespace WslStudio.App.ViewModels;

/// <summary>
/// A selectable sort order paired with its display label for the Distributions toolbar.
/// </summary>
public sealed record DistributionSortOption(string Label, DistributionSortKey Value);
