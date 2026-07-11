namespace WslStudio.Application.Wsl;

/// <summary>
/// Result of retrieving the consolidated WSL environment overview, following the same success/failure
/// shape used by the other read-only WSL services.
/// </summary>
public sealed record WslEnvironmentOverviewResult(
    WslEnvironmentOverview? Environment,
    bool Succeeded,
    string UserSafeMessage)
{
    public static WslEnvironmentOverviewResult Success(WslEnvironmentOverview environment)
    {
        return new WslEnvironmentOverviewResult(environment, Succeeded: true, string.Empty);
    }

    public static WslEnvironmentOverviewResult Failure(string userSafeMessage)
    {
        return new WslEnvironmentOverviewResult(null, Succeeded: false, userSafeMessage);
    }
}
