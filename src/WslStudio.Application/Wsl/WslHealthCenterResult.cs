using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed record WslHealthCenterResult(
    IReadOnlyList<WslHealthCheck> Checks,
    WslHealthCenterSummary Summary,
    bool Succeeded,
    string UserSafeMessage)
{
    public static WslHealthCenterResult Success(IReadOnlyList<WslHealthCheck> checks)
    {
        return new WslHealthCenterResult(
            checks,
            WslHealthCenterSummaryFactory.Create(checks),
            Succeeded: true,
            string.Empty);
    }

    public static WslHealthCenterResult Failure(string userSafeMessage)
    {
        return new WslHealthCenterResult(
            [],
            new WslHealthCenterSummary(0, 0, 0, 0, "Unavailable", "Try again after WSL is available."),
            Succeeded: false,
            userSafeMessage);
    }
}
