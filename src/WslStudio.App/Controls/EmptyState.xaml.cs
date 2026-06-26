using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WslStudio.App.Controls;

/// <summary>
/// Presents a neutral empty state for pages or sections with no available data.
/// </summary>
public sealed partial class EmptyState : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(EmptyState), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(EmptyState), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(Symbol), typeof(EmptyState), new PropertyMetadata(Symbol.Document));

    public EmptyState()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public Symbol Icon
    {
        get => (Symbol)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
