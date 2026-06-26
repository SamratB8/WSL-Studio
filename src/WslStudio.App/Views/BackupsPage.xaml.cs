using Microsoft.UI.Xaml.Controls;
using WslStudio.App.ViewModels;

namespace WslStudio.App.Views;

public sealed partial class BackupsPage : Page
{
    public BackupsPage(BackupsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
