using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Sentry;
using WslStudio.App.Services;

namespace WslStudio.App;

public partial class App : Microsoft.UI.Xaml.Application
{
    private readonly IHost _host;
    private readonly IDisposable _sentry;
    private Window? _window;

    public App()
    {
        _sentry = SentrySdk.Init(options =>
        {
            // Reads the DSN from the SENTRY_DSN environment variable.
            // Sentry remains disabled if the variable is missing or empty.
            options.Dsn = Environment.GetEnvironmentVariable("SENTRY_DSN");

#if DEBUG
            // Useful while initially testing the integration.
            options.Debug = true;
#else
            options.Debug = false;
#endif

            options.AutoSessionTracking = true;
        });

        InitializeComponent();

        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        builder.Services.AddPresentation();

        _host = builder.Build();
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
            await SentrySdk.FlushAsync(TimeSpan.FromSeconds(2));
        }
        finally
        {
            _host.Dispose();
            _sentry.Dispose();
        }
    }
}