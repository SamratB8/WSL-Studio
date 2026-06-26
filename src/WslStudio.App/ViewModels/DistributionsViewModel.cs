using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WslStudio.Application.Wsl;

namespace WslStudio.App.ViewModels;

public sealed partial class DistributionsViewModel(
    IWslDistributionDiscoveryService distributionDiscoveryService)
    : PageViewModelBase(
        "Distributions",
        "View installed WSL distributions using read-only information from official WSL commands.")
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDistributions))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    public ObservableCollection<DistributionListItemViewModel> Distributions { get; } = [];

    public bool HasDistributions => !IsLoading && string.IsNullOrWhiteSpace(ErrorMessage) && Distributions.Count > 0;

    public bool HasError => !IsLoading && !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsEmpty => !IsLoading && !HasError && Distributions.Count == 0;

    [RelayCommand]
    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
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

            foreach (DistributionListItemViewModel distribution in result.Distributions.Select(d => new DistributionListItemViewModel(d)))
            {
                Distributions.Add(distribution);
            }
        }
        finally
        {
            IsLoading = false;
            NotifyDistributionStateChanged();
        }
    }

    private void NotifyDistributionStateChanged()
    {
        OnPropertyChanged(nameof(HasDistributions));
        OnPropertyChanged(nameof(IsEmpty));
    }
}
