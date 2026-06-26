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
        [NavigationPageKey.Configuration] = typeof(ConfigurationPage),
        [NavigationPageKey.Backups] = typeof(BackupsPage),
        [NavigationPageKey.Diagnostics] = typeof(DiagnosticsPage),
        [NavigationPageKey.Settings] = typeof(SettingsPage)
    };

    private Frame? _frame;

    public void Initialize(Frame frame)
    {
        _frame = frame;
    }

    public bool NavigateTo(NavigationPageKey pageKey)
    {
        if (_frame is null || !_pageTypes.TryGetValue(pageKey, out Type? pageType))
        {
            return false;
        }

        if (_frame.Content?.GetType() == pageType)
        {
            return true;
        }

        _frame.Content = serviceProvider.GetRequiredService(pageType);
        return true;
    }
}
