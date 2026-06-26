using Microsoft.UI.Xaml.Controls;
using WslStudio.App.ViewModels;

namespace WslStudio.App.Views;

public sealed partial class DiagnosticsPage : Page
{
    public DiagnosticsPage(DiagnosticsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
