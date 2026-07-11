using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WslStudio.App.Navigation;
using WslStudio.App.Services;

namespace WslStudio.App;

public sealed partial class MainWindow : Window
{
    private readonly INavigationService _navigationService;

    public MainWindow(INavigationService navigationService)
    {
        _navigationService = navigationService;

        InitializeComponent();

        Title = "WSL Studio";
        SystemBackdrop = new MicaBackdrop();
    }

    private void OnShellNavigationViewLoaded(object sender, RoutedEventArgs args)
    {
        _navigationService.Initialize(ContentFrame);

        ShellNavigationView.SelectedItem = ShellNavigationView.MenuItems[0];
        NavigateTo(NavigationPageKey.Dashboard);
    }

    private void OnShellNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            NavigateTo(NavigationPageKey.Settings);
            return;
        }

        if (args.SelectedItemContainer?.Tag is string tag &&
            Enum.TryParse(tag, out NavigationPageKey pageKey))
        {
            NavigateTo(pageKey);
        }
    }

    private void NavigateTo(NavigationPageKey pageKey)
    {
        if (_navigationService.NavigateTo(pageKey))
        {
            ShellNavigationView.Header = GetPageHeader(pageKey);
        }
    }

    private static string GetPageHeader(NavigationPageKey pageKey)
    {
        return pageKey switch
        {
            NavigationPageKey.Dashboard => "Dashboard",
            NavigationPageKey.Distributions => "Distributions",
            NavigationPageKey.DistributionDetails => "Distribution details",
            NavigationPageKey.Configuration => "Configuration",
            NavigationPageKey.Backups => "Backups",
            NavigationPageKey.Diagnostics => "Diagnostics",
            NavigationPageKey.Environment => "Environment",
            NavigationPageKey.Settings => "Settings",
            _ => "WSL Studio"
        };
    }
}
