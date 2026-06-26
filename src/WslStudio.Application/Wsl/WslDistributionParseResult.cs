using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed record WslDistributionParseResult(
    IReadOnlyList<WslDistribution> Distributions,
    bool Succeeded,
    string UserSafeMessage)
{
    public static WslDistributionParseResult Success(IReadOnlyList<WslDistribution> distributions)
    {
        return new WslDistributionParseResult(distributions, Succeeded: true, string.Empty);
    }

    public static WslDistributionParseResult Failure(string userSafeMessage)
    {
        return new WslDistributionParseResult([], Succeeded: false, userSafeMessage);
    }
}
