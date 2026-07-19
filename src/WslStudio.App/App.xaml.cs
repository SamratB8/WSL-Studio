using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Sentry;
using WslStudio.App.Diagnostics;
using WslStudio.App.Services;

namespace WslStudio.App;

public partial class App : Microsoft.UI.Xaml.Application
{
    private readonly IHost _host;
    private readonly IDisposable? _sentry;
    private Window? _window;

    public App()
    {
        // Record otherwise opaque startup failures before they terminate the process.
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            StartupDiagnostics.LogException("AppDomain.UnhandledException", e.ExceptionObject as Exception);

        TaskScheduler.UnobservedTaskException += (_, e) =>
            StartupDiagnostics.LogException("TaskScheduler.UnobservedTaskException", e.Exception);

        UnhandledException += (_, e) =>
            StartupDiagnostics.LogException($"App.UnhandledException: {e.Message}", e.Exception);

        // The XAML application must be initialized before any integration that inspects or hooks it.
        InitializeComponent();

        _sentry = TryInitializeMonitoring();

        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        builder.Services.AddPresentation();

        _host = builder.Build();
    }

    /// <summary>
    /// Initializes optional error monitoring. Monitoring is best-effort: it is skipped when no DSN is
    /// configured, and any initialization failure is logged and swallowed so it can never prevent the
    /// application from starting.
    /// </summary>
    /// <remarks>
    /// This must run after <see cref="Microsoft.UI.Xaml.Application"/> initialization. Sentry's Windows
    /// integration inspects the current XAML application, which does not exist until
    /// <c>InitializeComponent()</c> has run; initializing earlier faults inside the XAML/COM layer and
    /// terminates the process with a stowed exception before any window is shown.
    /// </remarks>
    private static IDisposable? TryInitializeMonitoring()
    {
        string? dsn = Environment.GetEnvironmentVariable("SENTRY_DSN");

        if (string.IsNullOrWhiteSpace(dsn))
        {
            // Not configured. Running without monitoring is a supported configuration.
            return null;
        }

        try
        {
            return SentrySdk.Init(options =>
            {
                options.Dsn = dsn;
                options.AutoSessionTracking = true;

#if DEBUG
                // Keep the environment and diagnostic logging consistent so Sentry does not enable
                // debug logging for a production environment.
                options.Environment = "development";
                options.Debug = true;
#else
                options.Environment = "production";
                options.Debug = false;
#endif
            });
        }
        catch (Exception exception)
        {
            // A malformed DSN or any other monitoring failure must never crash the application.
            StartupDiagnostics.LogException("Sentry initialization failed; continuing without monitoring", exception);
            return null;
        }
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _host.Start();

        _window = _host.Services.GetRequiredService<MainWindow>();
        _window.Closed += OnMainWindowClosed;
        _window.Activate();
    }

    private async void OnMainWindowClosed(object sender, WindowEventArgs args)
    {
        try
        {
            await _host.StopAsync();

            if (_sentry is not null)
            {
                await SentrySdk.FlushAsync(TimeSpan.FromSeconds(2));
            }
        }
        finally
        {
            _host.Dispose();
            _sentry?.Dispose();
        }
    }
}
