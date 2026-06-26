using Microsoft.Extensions.DependencyInjection;
using WslStudio.Application.Commands;
using WslStudio.Application.Wsl;
using WslStudio.Infrastructure.Commands;
using WslStudio.App.ViewModels;
using WslStudio.App.Views;

namespace WslStudio.App.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ICommandRunner, ProcessCommandRunner>();
        services.AddSingleton<IWslDistributionParser, WslDistributionParser>();
        services.AddTransient<IWslDistributionDiscoveryService, WslDistributionDiscoveryService>();

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
