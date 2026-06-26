using Microsoft.UI.Xaml.Controls;
using WslStudio.App.ViewModels;

namespace WslStudio.App.Views;

public sealed partial class DistributionsPage : Page
{
    public DistributionsViewModel ViewModel { get; }

    public DistributionsPage(DistributionsViewModel viewModel)
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
