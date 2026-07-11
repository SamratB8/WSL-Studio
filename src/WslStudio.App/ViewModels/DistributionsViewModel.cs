using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WslStudio.App.Navigation;
using WslStudio.App.Services;
using WslStudio.Application.Wsl;
using WslStudio.Core.Wsl;

namespace WslStudio.App.ViewModels;

public sealed partial class DistributionsViewModel(
    IWslDistributionDiscoveryService distributionDiscoveryService,
    INavigationService navigationService)
    : PageViewModelBase(
        "Distributions",
        "View installed WSL distributions using read-only information from official WSL commands.")
{
    private static readonly DistributionFilterOption[] FilterOptions =
    [
        new("All", DistributionStateFilter.All),
        new("Running", DistributionStateFilter.Running),
        new("Stopped", DistributionStateFilter.Stopped),
        new("Other", DistributionStateFilter.Other)
    ];

    private static readonly DistributionSortOption[] SortOptionList =
    [
        new("Default first", DistributionSortKey.DefaultFirst),
        new("Name", DistributionSortKey.Name),
        new("State", DistributionSortKey.State),
        new("WSL version", DistributionSortKey.Version)
    ];

    private readonly List<WslDistribution> _allDistributions = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDistributions))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    [NotifyPropertyChangedFor(nameof(HasAnyDistributions))]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    [NotifyPropertyChangedFor(nameof(HasAnyDistributions))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial DistributionFilterOption SelectedStateFilter { get; set; } = FilterOptions[0];

    [ObservableProperty]
    public partial DistributionSortOption SelectedSort { get; set; } = SortOptionList[0];

    public IReadOnlyList<DistributionFilterOption> StateFilterOptions => FilterOptions;

    public IReadOnlyList<DistributionSortOption> SortOptions => SortOptionList;

    public ObservableCollection<DistributionListItemViewModel> Distributions { get; } = [];

    public bool HasDistributions => !IsLoading && string.IsNullOrWhiteSpace(ErrorMessage) && Distributions.Count > 0;

    public bool HasError => !IsLoading && !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsEmpty => !IsLoading && !HasError && Distributions.Count == 0;

    /// <summary>
    /// True when at least one distribution is installed, regardless of the current search or filter.
    /// Drives visibility of the search/filter/sort toolbar so the user can always clear an empty result.
    /// </summary>
    public bool HasAnyDistributions => !IsLoading && !HasError && _allDistributions.Count > 0;

    /// <summary>
    /// Visibility of the search/filter/sort toolbar. The toolbar stays in the visual tree at all
    /// times and is shown or hidden via this property rather than <c>x:Load</c>, because the page
    /// refreshes compiled bindings with <c>Bindings.Update()</c> and that is incompatible with a
    /// deferred element that contains nested <c>x:Bind</c> bindings.
    /// </summary>
    public Visibility ToolbarVisibility => HasAnyDistributions ? Visibility.Visible : Visibility.Collapsed;

    public string EmptyStateTitle =>
        _allDistributions.Count == 0 ? "No distributions found" : "No matching distributions";

    public string EmptyStateDescription =>
        _allDistributions.Count == 0
            ? "WSL did not report any installed distributions. This page only shows real read-only data returned by WSL."
            : "No distributions match the current search or filter. Try clearing the search box or selecting a different filter.";

    public Symbol EmptyStateIcon => _allDistributions.Count == 0 ? Symbol.Library : Symbol.Find;

    [RelayCommand]
    private void OpenDetails(DistributionListItemViewModel? distribution)
    {
        if (distribution is not null)
        {
            navigationService.NavigateTo(NavigationPageKey.DistributionDetails, distribution.DistributionName);
        }
    }

    [RelayCommand]
    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        _allDistributions.Clear();
        Distributions.Clear();
        NotifyDistributionStateChanged();

        try
        {
            WslDistributionDiscoveryResult result =
                await distributionDiscoveryService.GetDistributionsAsync(cancellationToken);

            if (!result.Succeeded)
            {
                ErrorMessage = result.UserSafeMessage;
                return;
            }

            _allDistributions.AddRange(result.Distributions);
            ApplyQuery();
        }
        finally
        {
            IsLoading = false;
            NotifyDistributionStateChanged();
        }
    }

    partial void OnSearchTextChanged(string value) => ApplyQuery();

    partial void OnSelectedStateFilterChanged(DistributionFilterOption value) => ApplyQuery();

    partial void OnSelectedSortChanged(DistributionSortOption value) => ApplyQuery();

    private void ApplyQuery()
    {
        IReadOnlyList<WslDistribution> results = WslDistributionQuery.Apply(
            _allDistributions,
            SearchText,
            SelectedStateFilter.Value,
            SelectedSort.Value);

        Distributions.Clear();
        foreach (WslDistribution distribution in results)
        {
            Distributions.Add(new DistributionListItemViewModel(distribution));
        }

        NotifyDistributionStateChanged();
    }

    private void NotifyDistributionStateChanged()
    {
        OnPropertyChanged(nameof(HasDistributions));
        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(HasAnyDistributions));
        OnPropertyChanged(nameof(ToolbarVisibility));
        OnPropertyChanged(nameof(EmptyStateTitle));
        OnPropertyChanged(nameof(EmptyStateDescription));
        OnPropertyChanged(nameof(EmptyStateIcon));
    }
}
