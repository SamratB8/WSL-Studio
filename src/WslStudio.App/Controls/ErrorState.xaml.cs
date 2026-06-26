using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WslStudio.App.Controls;

/// <summary>
/// Presents a user-safe error message in a consistent card layout.
/// </summary>
public sealed partial class ErrorState : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(ErrorState), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register(nameof(Message), typeof(string), typeof(ErrorState), new PropertyMetadata(string.Empty));

    public ErrorState()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }
}
