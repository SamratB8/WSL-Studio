namespace WslStudio.App.ViewModels;

public abstract partial class PageViewModelBase(string title, string subtitle) : ViewModelBase
{
    public string Title { get; } = title;

    public string Subtitle { get; } = subtitle;
}
