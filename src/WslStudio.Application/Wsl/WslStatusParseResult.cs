using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed record WslStatusParseResult(
    WslStatusInfo? StatusInfo,
    bool Succeeded,
    string UserSafeMessage)
{
    public static WslStatusParseResult Success(WslStatusInfo statusInfo)
    {
        return new WslStatusParseResult(statusInfo, Succeeded: true, string.Empty);
    }

    public static WslStatusParseResult Failure(string userSafeMessage)
    {
        return new WslStatusParseResult(null, Succeeded: false, userSafeMessage);
    }
}
