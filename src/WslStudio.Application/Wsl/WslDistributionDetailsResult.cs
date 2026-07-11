namespace WslStudio.Application.Wsl;

public sealed record WslDistributionDetailsResult(
    WslDistributionDetails? Details,
    bool Succeeded,
    bool NotFound,
    string UserSafeMessage)
{
    public static WslDistributionDetailsResult Success(WslDistributionDetails details)
    {
        return new WslDistributionDetailsResult(details, Succeeded: true, NotFound: false, string.Empty);
    }

    public static WslDistributionDetailsResult Missing(string userSafeMessage)
    {
        return new WslDistributionDetailsResult(null, Succeeded: true, NotFound: true, userSafeMessage);
    }

    public static WslDistributionDetailsResult Failure(string userSafeMessage)
    {
        return new WslDistributionDetailsResult(null, Succeeded: false, NotFound: false, userSafeMessage);
    }
}
