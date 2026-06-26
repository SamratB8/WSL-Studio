using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed record WslEnvironmentResult(
    WslEnvironmentInfo? EnvironmentInfo,
    bool Succeeded,
    string UserSafeMessage)
{
    public static WslEnvironmentResult Success(WslEnvironmentInfo environmentInfo)
    {
        return new WslEnvironmentResult(environmentInfo, Succeeded: true, string.Empty);
    }

    public static WslEnvironmentResult Failure(string userSafeMessage)
    {
        return new WslEnvironmentResult(null, Succeeded: false, userSafeMessage);
    }
}
