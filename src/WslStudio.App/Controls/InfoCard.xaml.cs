using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WslStudio.App.Controls;

public sealed partial class InfoCard : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(InfoCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(InfoCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(Symbol), typeof(InfoCard), new PropertyMetadata(Symbol.Document));

    public InfoCard()
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
