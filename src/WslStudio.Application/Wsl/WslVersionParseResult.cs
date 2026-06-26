using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed record WslVersionParseResult(
    WslVersionInfo? VersionInfo,
    bool Succeeded,
    string UserSafeMessage)
{
    public static WslVersionParseResult Success(WslVersionInfo versionInfo)
    {
        return new WslVersionParseResult(versionInfo, Succeeded: true, string.Empty);
    }

    public static WslVersionParseResult Failure(string userSafeMessage)
    {
        return new WslVersionParseResult(null, Succeeded: false, userSafeMessage);
    }
}
