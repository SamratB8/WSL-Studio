using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using WslStudio.App.Navigation;
using WslStudio.App.Views;

namespace WslStudio.App.Services;

public sealed class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
    private readonly Dictionary<NavigationPageKey, Type> _pageTypes = new()
    {
        [NavigationPageKey.Dashboard] = typeof(DashboardPage),
        [NavigationPageKey.Distributions] = typeof(DistributionsPage),
        [NavigationPageKey.DistributionDetails] = typeof(DistributionDetailsPage),
        [NavigationPageKey.Configuration] = typeof(ConfigurationPage),
        [NavigationPageKey.Backups] = typeof(BackupsPage),
        [NavigationPageKey.Diagnostics] = typeof(DiagnosticsPage),
        [NavigationPageKey.Environment] = typeof(EnvironmentPage),
        [NavigationPageKey.Settings] = typeof(SettingsPage)
    };

    private Frame? _frame;

    public void Initialize(Frame frame)
    {
        _frame = frame;
    }

    public bool NavigateTo(NavigationPageKey pageKey, object? parameter = null)
    {
        if (_frame is null || !_pageTypes.TryGetValue(pageKey, out Type? pageType))
        {
            return false;
        }

        if (_frame.Content?.GetType() == pageType && parameter is null)
        {
            return true;
        }

        object page = serviceProvider.GetRequiredService(pageType);
        _frame.Content = page;

        if (page is INavigationAware navigationAware)
        {
            navigationAware.OnNavigatedTo(parameter);
        }

        return true;
    }
}
