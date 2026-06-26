using Microsoft.UI.Xaml.Controls;
using WslStudio.App.ViewModels;

namespace WslStudio.App.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
