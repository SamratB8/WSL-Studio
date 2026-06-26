using Microsoft.UI.Xaml.Controls;
using WslStudio.App.ViewModels;

namespace WslStudio.App.Views;

public sealed partial class ConfigurationPage : Page
{
    public ConfigurationPage(ConfigurationViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
