using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using WslStudio.App.Services;
using WslStudio.Application.Wsl;
using WslStudio.Application.Wsl.Diagnostics;
using WslStudio.Core.Wsl;

namespace WslStudio.App.ViewModels;

public sealed partial class DiagnosticsViewModel(
    IWslHealthCenterService healthCenterService,
    IWslDiagnosticReportService reportService,
    IFileSaveService fileSaveService)
    : PageViewModelBase(
        "WSL Health Center",
        "Read-only checks for WSL readiness, configuration signals, and local environment diagnostics.")
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasHealthChecks))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    [NotifyPropertyChangedFor(nameof(CanExport))]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string OverallStatus { get; set; } = "Unavailable";

    [ObservableProperty]
    public partial string Recommendation { get; set; } = "Run Health Center to review WSL readiness.";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanExport))]
    public partial bool IsExporting { get; set; }

    [ObservableProperty]
    public partial bool IsExportInfoOpen { get; set; }

    [ObservableProperty]
    public partial string ExportInfoMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial InfoBarSeverity ExportInfoSeverity { get; set; } = InfoBarSeverity.Informational;

    public ObservableCollection<OverviewCardViewModel> SummaryCards { get; } = [];

    public ObservableCollection<HealthCheckGroupViewModel> HealthCheckGroups { get; } = [];

    public bool HasHealthChecks => !IsLoading && string.IsNullOrWhiteSpace(ErrorMessage) && HealthCheckGroups.Count > 0;

    public bool HasError => !IsLoading && !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsEmpty => !IsLoading && !HasError && HealthCheckGroups.Count == 0;

    public bool CanExport => !IsLoading && !IsExporting;

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

    [RelayCommand]
    private Task ExportMarkdownAsync() => ExportAsync(DiagnosticReportFormat.Markdown);

    [RelayCommand]
    private Task ExportTextAsync() => ExportAsync(DiagnosticReportFormat.Text);

    [RelayCommand]
    private Task ExportJsonAsync() => ExportAsync(DiagnosticReportFormat.Json);

    private async Task ExportAsync(DiagnosticReportFormat format)
    {
        if (IsExporting)
        {
            return;
        }

        IsExporting = true;
        IsExportInfoOpen = false;

        try
        {
            WslDiagnosticReportResult result = await reportService.GenerateReportAsync(CancellationToken.None);

            if (!result.Succeeded || result.Report is null)
            {
                ShowExportInfo(InfoBarSeverity.Error, $"The diagnostic report could not be generated. {result.UserSafeMessage}");
                return;
            }

            DiagnosticReportContent content = reportService.CreateContent(result.Report, format);
            FileSaveResult saveResult = await fileSaveService.SaveTextAsync(
                content.FileName,
                content.FileTypeDescription,
                content.FileExtension,
                content.Text);

            switch (saveResult.Outcome)
            {
                case FileSaveOutcome.Saved:
                    ShowExportInfo(InfoBarSeverity.Success, $"Diagnostic report saved to {saveResult.Path}.");
                    break;
                case FileSaveOutcome.Canceled:
                    ShowExportInfo(InfoBarSeverity.Informational, "Export canceled. No file was saved.");
                    break;
                default:
                    ShowExportInfo(InfoBarSeverity.Error, $"The report could not be saved. {saveResult.ErrorMessage}");
                    break;
            }
        }
        finally
        {
            IsExporting = false;
        }
    }

    private void ShowExportInfo(InfoBarSeverity severity, string message)
    {
        ExportInfoSeverity = severity;
        ExportInfoMessage = message;
        IsExportInfoOpen = true;
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
        OnPropertyChanged(nameof(CanExport));
    }
}
