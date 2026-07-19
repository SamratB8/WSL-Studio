using Microsoft.Extensions.DependencyInjection;
using WslStudio.Application.Commands;
using WslStudio.Application.Wsl;
using WslStudio.Application.Wsl.Diagnostics;
using WslStudio.Application.Wsl.Terminal;
using WslStudio.Infrastructure.Commands;
using WslStudio.Infrastructure.Terminal;
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
        services.AddSingleton<IWslStatusParser, WslStatusParser>();
        services.AddSingleton<IWslVersionParser, WslVersionParser>();
        services.AddTransient<IWslDistributionDiscoveryService, WslDistributionDiscoveryService>();
        services.AddTransient<IWslDistributionDetailsService, WslDistributionDetailsService>();
        services.AddTransient<IWslDashboardOverviewService, WslDashboardOverviewService>();
        services.AddTransient<IWslHealthCenterService, WslHealthCenterService>();
        services.AddTransient<IWslEnvironmentService, WslEnvironmentService>();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IDiagnosticReportFormatter, MarkdownDiagnosticReportFormatter>();
        services.AddSingleton<IDiagnosticReportFormatter, TextDiagnosticReportFormatter>();
        services.AddSingleton<IDiagnosticReportFormatter, JsonDiagnosticReportFormatter>();
        services.AddTransient<IWslDiagnosticReportService, WslDiagnosticReportService>();
        services.AddSingleton<IFileSaveService, FileSaveService>();

        services.AddSingleton<ITerminalProcessLauncher, ProcessTerminalLauncher>();
        services.AddTransient<IWslTerminalService, WslTerminalService>();

        services.AddTransient<DashboardViewModel>();
        services.AddTransient<DistributionsViewModel>();
        services.AddTransient<DistributionDetailsViewModel>();
        services.AddTransient<ConfigurationViewModel>();
        services.AddTransient<BackupsViewModel>();
        services.AddTransient<DiagnosticsViewModel>();
        services.AddTransient<EnvironmentViewModel>();
        services.AddTransient<SettingsViewModel>();

        services.AddTransient<DashboardPage>();
        services.AddTransient<DistributionsPage>();
        services.AddTransient<DistributionDetailsPage>();
        services.AddTransient<ConfigurationPage>();
        services.AddTransient<BackupsPage>();
        services.AddTransient<DiagnosticsPage>();
        services.AddTransient<EnvironmentPage>();
        services.AddTransient<SettingsPage>();

        return services;
    }
}
