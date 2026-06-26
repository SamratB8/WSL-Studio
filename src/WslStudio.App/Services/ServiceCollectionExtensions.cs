using Microsoft.Extensions.DependencyInjection;
using WslStudio.App.ViewModels;
using WslStudio.App.Views;

namespace WslStudio.App.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddTransient<DashboardViewModel>();
        services.AddTransient<DistributionsViewModel>();
        services.AddTransient<ConfigurationViewModel>();
        services.AddTransient<BackupsViewModel>();
        services.AddTransient<DiagnosticsViewModel>();
        services.AddTransient<SettingsViewModel>();

        services.AddTransient<DashboardPage>();
        services.AddTransient<DistributionsPage>();
        services.AddTransient<ConfigurationPage>();
        services.AddTransient<BackupsPage>();
        services.AddTransient<DiagnosticsPage>();
        services.AddTransient<SettingsPage>();

        return services;
    }
}
