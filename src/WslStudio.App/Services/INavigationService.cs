using Microsoft.UI.Xaml.Controls;
using WslStudio.App.Navigation;

namespace WslStudio.App.Services;

public interface INavigationService
{
    void Initialize(Frame frame);

    bool NavigateTo(NavigationPageKey pageKey, object? parameter = null);
}
