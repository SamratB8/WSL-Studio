using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WslStudio.App.Navigation;
using WslStudio.App.ViewModels;
using WslStudio.Core.Wsl;

namespace WslStudio.App.Views;

public sealed partial class DistributionDetailsPage : Page, INavigationAware
{
    public DistributionDetailsViewModel ViewModel { get; }

    public DistributionDetailsPage(DistributionDetailsViewModel viewModel)
    {
        ViewModel = viewModel;
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;

        InitializeComponent();
        DataContext = viewModel;
    }

    public void OnNavigatedTo(object? parameter)
    {
        if (parameter is DistributionName distributionName)
        {
            ViewModel.SetDistribution(distributionName);
        }
    }

    private async void OnLoaded(object sender, RoutedEventArgs args)
    {
        if (!ViewModel.LoadCommand.IsRunning)
        {
            await ViewModel.LoadCommand.ExecuteAsync(null);
        }
    }

    private void OnBackButtonClick(object sender, RoutedEventArgs args)
    {
        ViewModel.GoBackCommand.Execute(null);
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs args)
    {
        Bindings.Update();
    }
}
