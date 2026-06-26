using Microsoft.UI.Xaml.Controls;
using WslStudio.App.ViewModels;

namespace WslStudio.App.Views;

public sealed partial class DashboardPage : Page
{
    public DashboardViewModel ViewModel { get; }

    public DashboardPage(DashboardViewModel viewModel)
    {
        ViewModel = viewModel;
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;

        InitializeComponent();
        DataContext = viewModel;
    }

    private async void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs args)
    {
        if (!ViewModel.LoadCommand.IsRunning)
        {
            await ViewModel.LoadCommand.ExecuteAsync(null);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs args)
    {
        Bindings.Update();
    }
}
