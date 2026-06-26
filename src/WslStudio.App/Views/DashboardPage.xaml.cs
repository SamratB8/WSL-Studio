using Microsoft.UI.Xaml.Controls;
using WslStudio.App.ViewModels;

namespace WslStudio.App.Views;

public sealed partial class DashboardPage : Page
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
