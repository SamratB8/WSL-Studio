using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WslStudio.Application.Wsl;
using WslStudio.Core.Wsl;

namespace WslStudio.App.ViewModels;

public sealed partial class EnvironmentViewModel(
    IWslEnvironmentService environmentService)
    : PageViewModelBase(
        "Environment",
        "A read-only, consolidated overview of your local WSL environment gathered from official WSL commands.")
{
    private const string Unavailable = "Unavailable";
    private const string NotReported = "Not reported by WSL";

    private bool _hasContent;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasEnvironment))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    /// <summary>WSL component versions (WSL, default version, kernel, WSLg, Windows).</summary>
    public ObservableCollection<OverviewCardViewModel> WslCards { get; } = [];

    /// <summary>Graphics component versions (Direct3D, DXCore, MSRDC).</summary>
    public ObservableCollection<OverviewCardViewModel> GraphicsCards { get; } = [];

    /// <summary>Distribution and integration facts (counts, default, Docker Desktop, WSLg).</summary>
    public ObservableCollection<OverviewCardViewModel> EnvironmentCards { get; } = [];

    public bool HasEnvironment => !IsLoading && string.IsNullOrWhiteSpace(ErrorMessage) && _hasContent;

    public bool HasError => !IsLoading && !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsEmpty => !IsLoading && !HasError && !_hasContent;

    [RelayCommand]
    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        _hasContent = false;
        WslCards.Clear();
        GraphicsCards.Clear();
        EnvironmentCards.Clear();
        NotifyEnvironmentStateChanged();

        try
        {
            WslEnvironmentOverviewResult result = await environmentService.GetEnvironmentAsync(cancellationToken);

            if (!result.Succeeded || result.Environment is null)
            {
                ErrorMessage = result.UserSafeMessage;
                return;
            }

            PopulateCards(result.Environment);
            _hasContent = WslCards.Count > 0 || GraphicsCards.Count > 0 || EnvironmentCards.Count > 0;
        }
        finally
        {
            IsLoading = false;
            NotifyEnvironmentStateChanged();
        }
    }

    private void PopulateCards(WslEnvironmentOverview environment)
    {
        WslDashboardOverview overview = environment.Overview;
        WslVersionInfo? version = overview.VersionInfo;
        WslStatusInfo? status = overview.StatusInfo;

        WslCards.Add(new OverviewCardViewModel(
            "WSL version", OrUnavailable(version?.WslVersion), "Installed WSL release."));
        WslCards.Add(new OverviewCardViewModel(
            "Default WSL version", status?.DefaultVersion?.ToString() ?? Unavailable, "Default version for new distributions."));
        WslCards.Add(new OverviewCardViewModel(
            "Kernel version", OrUnavailable(version?.KernelVersion ?? status?.KernelVersion), "Linux kernel reported by WSL."));
        WslCards.Add(new OverviewCardViewModel(
            "WSLg version", OrUnavailable(version?.WslgVersion), "Linux GUI (WSLg) component."));
        WslCards.Add(new OverviewCardViewModel(
            "Windows version", OrUnavailable(version?.WindowsVersion), "Host Windows build."));

        GraphicsCards.Add(new OverviewCardViewModel(
            "Direct3D version", OrNotReported(version?.Direct3DVersion), "Direct3D component for WSLg."));
        GraphicsCards.Add(new OverviewCardViewModel(
            "DXCore version", OrNotReported(version?.DxCoreVersion), "DXCore component for WSLg."));
        GraphicsCards.Add(new OverviewCardViewModel(
            "MSRDC version", OrNotReported(version?.MsrdcVersion), "Remote display component for WSLg."));

        EnvironmentCards.Add(new OverviewCardViewModel(
            "Installed", overview.TotalDistributionCount.ToString(), "Distributions reported by WSL."));
        EnvironmentCards.Add(new OverviewCardViewModel(
            "Running", overview.RunningDistributionCount.ToString(), "Distributions currently running."));
        EnvironmentCards.Add(new OverviewCardViewModel(
            "Default", overview.DefaultDistribution?.Value ?? "Not set", "Default distribution for WSL commands."));
        EnvironmentCards.Add(new OverviewCardViewModel(
            "Docker", environment.DockerDesktopDetected ? "Detected" : "Not detected", "Optional docker-desktop WSL integration."));
        EnvironmentCards.Add(new OverviewCardViewModel(
            "WSLg", environment.WslgAvailable ? "Available" : "Not available", "Linux GUI application support."));
    }

    private static string OrUnavailable(string? value) =>
        string.IsNullOrWhiteSpace(value) ? Unavailable : value;

    private static string OrNotReported(string? value) =>
        string.IsNullOrWhiteSpace(value) ? NotReported : value;

    private void NotifyEnvironmentStateChanged()
    {
        OnPropertyChanged(nameof(HasEnvironment));
        OnPropertyChanged(nameof(IsEmpty));
    }
}
