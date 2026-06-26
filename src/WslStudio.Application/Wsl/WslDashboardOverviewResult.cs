namespace WslStudio.Application.Wsl;

public sealed record WslDashboardOverviewResult(
    WslDashboardOverview? Overview,
    bool Succeeded,
    string UserSafeMessage)
{
    public static WslDashboardOverviewResult Success(WslDashboardOverview overview)
    {
        return new WslDashboardOverviewResult(overview, Succeeded: true, string.Empty);
    }

    public static WslDashboardOverviewResult Failure(string userSafeMessage)
    {
        return new WslDashboardOverviewResult(null, Succeeded: false, userSafeMessage);
    }
}
