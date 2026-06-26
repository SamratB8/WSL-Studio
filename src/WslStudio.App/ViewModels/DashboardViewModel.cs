using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WslStudio.Application.Wsl;

namespace WslStudio.App.ViewModels;

public sealed partial class DashboardViewModel(
    IWslDashboardOverviewService dashboardOverviewService)
    : PageViewModelBase(
        "Dashboard",
        "A read-only overview of your WSL environment.")
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasOverview))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string WslVersion { get; set; } = "Unavailable";

    [ObservableProperty]
    public partial string WslStatus { get; set; } = "Unavailable";

    public ObservableCollection<OverviewCardViewModel> OverviewCards { get; } = [];

    public bool HasOverview => !IsLoading && string.IsNullOrWhiteSpace(ErrorMessage) && OverviewCards.Count > 0;

    public bool HasError => !IsLoading && !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsEmpty => !IsLoading && !HasError && OverviewCards.Count == 0;

    [RelayCommand]
    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        OverviewCards.Clear();
        WslVersion = "Unavailable";
        WslStatus = "Unavailable";
        NotifyOverviewStateChanged();

        try
        {
            WslDashboardOverviewResult result =
                await dashboardOverviewService.GetOverviewAsync(cancellationToken);

            if (!result.Succeeded || result.Overview is null)
            {
                ErrorMessage = result.UserSafeMessage;
                return;
            }

            WslDashboardOverview overview = result.Overview;

            if (overview.TotalDistributionCount == 0)
            {
                return;
            }

            OverviewCards.Add(new OverviewCardViewModel(
                "Total distributions",
                overview.TotalDistributionCount.ToString(),
                "Installed distributions reported by WSL."));
            OverviewCards.Add(new OverviewCardViewModel(
                "Running",
                overview.RunningDistributionCount.ToString(),
                "Distributions currently reported as running."));
            OverviewCards.Add(new OverviewCardViewModel(
                "Stopped",
                overview.StoppedDistributionCount.ToString(),
                "Distributions currently reported as stopped."));
            OverviewCards.Add(new OverviewCardViewModel(
                "Default distribution",
                overview.DefaultDistribution?.Value ?? "Not set",
                "Default target for WSL commands."));

            WslVersion = overview.VersionInfo?.WslVersion ?? "Unavailable";
            WslStatus = ToStatusText(overview);
        }
        finally
        {
            IsLoading = false;
            NotifyOverviewStateChanged();
        }
    }

    private static string ToStatusText(WslDashboardOverview overview)
    {
        if (overview.StatusInfo?.DefaultVersion is int defaultVersion)
        {
            return $"Default version {defaultVersion}";
        }

        return overview.TotalDistributionCount > 0
            ? "Available"
            : "No distributions installed";
    }

    private void NotifyOverviewStateChanged()
    {
        OnPropertyChanged(nameof(HasOverview));
        OnPropertyChanged(nameof(IsEmpty));
    }
}
