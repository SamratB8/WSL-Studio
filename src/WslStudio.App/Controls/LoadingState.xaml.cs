using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WslStudio.App.Controls;

/// <summary>
/// Displays an indeterminate loading indicator with concise status text.
/// </summary>
public sealed partial class LoadingState : UserControl
{
    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register(nameof(Message), typeof(string), typeof(LoadingState), new PropertyMetadata("Loading..."));

    public LoadingState()
    {
        InitializeComponent();
    }

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }
}
