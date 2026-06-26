using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

namespace WslStudio.App;

public partial class App : Application
{
    private readonly IHost _host;
    private Window? _window;

    public App()
    {
        InitializeComponent();

        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton<MainWindow>();

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
        await _host.StopAsync();
        _host.Dispose();
    }
}
