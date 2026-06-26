using Microsoft.UI.Xaml.Controls;
using WslStudio.App.ViewModels;

namespace WslStudio.App.Views;

public sealed partial class DistributionsPage : Page
{
    public DistributionsPage(DistributionsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
