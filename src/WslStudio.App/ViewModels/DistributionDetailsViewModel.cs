using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using WslStudio.App.Navigation;
using WslStudio.App.Services;
using WslStudio.Application.Wsl;
using WslStudio.Application.Wsl.Terminal;
using WslStudio.Core.Wsl;

namespace WslStudio.App.ViewModels;

public sealed partial class DistributionDetailsViewModel(
    IWslDistributionDetailsService distributionDetailsService,
    IWslTerminalService terminalService,
    INavigationService navigationService)
    : PageViewModelBase(
        "Distribution details",
        "Read-only information reported by official WSL commands.")
{
    private DistributionName? _distributionName;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    public partial string EmptyMessage { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    [NotifyCanExecuteChangedFor(nameof(OpenTerminalCommand))]
    public partial bool HasDetails { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenTerminalCommand))]
    public partial bool IsLaunchingTerminal { get; set; }

    [ObservableProperty]
    public partial bool IsLaunchInfoOpen { get; set; }

    [ObservableProperty]
    public partial string LaunchInfoMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial InfoBarSeverity LaunchInfoSeverity { get; set; } = InfoBarSeverity.Error;

    [ObservableProperty]
    public partial string DistributionDisplayName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string State { get; set; } = "Unknown";

    [ObservableProperty]
    public partial string DefaultStatus { get; set; } = "Not default";

    public ObservableCollection<OverviewCardViewModel> SummaryCards { get; } = [];

    public ObservableCollection<DistributionDetailItemViewModel> DetailItems { get; } = [];

    public bool HasError => !IsLoading && !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsEmpty => !IsLoading && !HasError && !HasDetails;

    public bool CanOpenTerminal => HasDetails && !IsLaunchingTerminal;

    public void SetDistribution(DistributionName distributionName)
    {
        _distributionName = distributionName;
        DistributionDisplayName = distributionName.Value;
    }

    [RelayCommand]
    private void GoBack()
    {
        navigationService.NavigateTo(NavigationPageKey.Distributions);
    }

    [RelayCommand(CanExecute = nameof(CanOpenTerminal))]
    private async Task OpenTerminalAsync()
    {
        if (_distributionName is null || IsLaunchingTerminal)
        {
            return;
        }

        IsLaunchingTerminal = true;
        IsLaunchInfoOpen = false;

        try
        {
            WslTerminalLaunchResult result =
                await terminalService.LaunchDistributionAsync(_distributionName, CancellationToken.None);

            if (!result.Succeeded)
            {
                LaunchInfoSeverity = InfoBarSeverity.Error;
                LaunchInfoMessage = result.UserSafeMessage;
                IsLaunchInfoOpen = true;
            }
        }
        finally
        {
            IsLaunchingTerminal = false;
        }
    }

    [RelayCommand]
    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        EmptyMessage = string.Empty;
        HasDetails = false;
        SummaryCards.Clear();
        DetailItems.Clear();

        try
        {
            if (_distributionName is null)
            {
                EmptyMessage = "Select a distribution from the Distributions page to view its details.";
                return;
            }

            WslDistributionDetailsResult result =
                await distributionDetailsService.GetDetailsAsync(_distributionName, cancellationToken);

            if (!result.Succeeded)
            {
                ErrorMessage = result.UserSafeMessage;
                return;
            }

            if (result.NotFound || result.Details is null)
            {
                EmptyMessage = result.UserSafeMessage;
                return;
            }

            ApplyDetails(result.Details);
            HasDetails = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ApplyDetails(WslDistributionDetails details)
    {
        DistributionDisplayName = details.Distribution.Name.Value;
        State = ToDisplayText(details.Distribution.State);
        DefaultStatus = details.Distribution.IsDefault ? "Default distribution" : "Not default";

        SummaryCards.Add(new OverviewCardViewModel(
            "State",
            State,
            "Current state reported by WSL."));
        SummaryCards.Add(new OverviewCardViewModel(
            "WSL version",
            details.Distribution.Version?.ToString() ?? "Not reported",
            "WSL generation for this distribution."));
        SummaryCards.Add(new OverviewCardViewModel(
            "Default status",
            DefaultStatus,
            details.Distribution.IsDefault
                ? "Used when no distribution is specified."
                : "Another distribution is currently the default."));

        DetailItems.Add(new DistributionDetailItemViewModel(
            "Architecture",
            details.Architecture ?? "Not available",
            "Official WSL discovery does not currently report this value."));
        DetailItems.Add(new DistributionDetailItemViewModel(
            "Installation location",
            details.InstallationLocation ?? "Not available",
            "This location is not exposed by the current read-only discovery flow."));
        DetailItems.Add(new DistributionDetailItemViewModel(
            "WSL kernel version",
            details.KernelVersion ?? "Not reported",
            "Kernel information applies to the local WSL environment when available."));
    }

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
