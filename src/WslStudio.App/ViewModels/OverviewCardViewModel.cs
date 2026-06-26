namespace WslStudio.App.ViewModels;

public sealed class OverviewCardViewModel(string title, string value, string description)
{
    public string Title { get; } = title;

    public string Value { get; } = value;

    public string Description { get; } = description;
}
