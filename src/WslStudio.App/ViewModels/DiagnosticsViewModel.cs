using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WslStudio.Application.Wsl;
using WslStudio.Core.Wsl;

namespace WslStudio.App.ViewModels;

public sealed partial class DiagnosticsViewModel(
    IWslHealthCenterService healthCenterService)
    : PageViewModelBase(
        "WSL Health Center",
        "Read-only checks for WSL readiness, configuration signals, and local environment diagnostics.")
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasHealthChecks))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string OverallStatus { get; set; } = "Unavailable";

    [ObservableProperty]
    public partial string Recommendation { get; set; } = "Run Health Center to review WSL readiness.";

    public ObservableCollection<OverviewCardViewModel> SummaryCards { get; } = [];

    public ObservableCollection<HealthCheckGroupViewModel> HealthCheckGroups { get; } = [];

    public bool HasHealthChecks => !IsLoading && string.IsNullOrWhiteSpace(ErrorMessage) && HealthCheckGroups.Count > 0;

    public bool HasError => !IsLoading && !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsEmpty => !IsLoading && !HasError && HealthCheckGroups.Count == 0;

    [RelayCommand]
    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        OverallStatus = "Checking";
        Recommendation = "Health Center is evaluating read-only WSL information.";
        SummaryCards.Clear();
        HealthCheckGroups.Clear();
        NotifyHealthStateChanged();

        try
        {
            WslHealthCenterResult result = await healthCenterService.GetHealthAsync(cancellationToken);

            if (!result.Succeeded)
            {
                ErrorMessage = result.UserSafeMessage;
                return;
            }

            OverallStatus = result.Summary.OverallStatus;
            Recommendation = result.Summary.Recommendation;
            AddSummaryCards(result.Summary);
            AddHealthGroups(result.Checks);
        }
        finally
        {
            IsLoading = false;
            NotifyHealthStateChanged();
        }
    }

    private void AddSummaryCards(WslHealthCenterSummary summary)
    {
        SummaryCards.Add(new OverviewCardViewModel("Healthy", summary.HealthyCount.ToString(), "Passed"));
        SummaryCards.Add(new OverviewCardViewModel("Warning", summary.WarningCount.ToString(), "Review"));
        SummaryCards.Add(new OverviewCardViewModel("Error", summary.ErrorCount.ToString(), "Attention"));
        SummaryCards.Add(new OverviewCardViewModel("Unknown", summary.UnknownCount.ToString(), "Not checked by WSL Studio yet"));
    }

    private void AddHealthGroups(IReadOnlyList<WslHealthCheck> checks)
    {
        foreach (IGrouping<WslHealthCheckCategory, WslHealthCheck> group in checks.GroupBy(check => check.Category))
        {
            HealthCheckGroups.Add(new HealthCheckGroupViewModel(group.Key, group));
        }
    }

    private void NotifyHealthStateChanged()
    {
        OnPropertyChanged(nameof(HasHealthChecks));
        OnPropertyChanged(nameof(IsEmpty));
    }
}
