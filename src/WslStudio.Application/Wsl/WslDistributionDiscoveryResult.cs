using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed record WslDistributionDiscoveryResult(
    IReadOnlyList<WslDistribution> Distributions,
    bool Succeeded,
    string UserSafeMessage)
{
    public static WslDistributionDiscoveryResult Success(IReadOnlyList<WslDistribution> distributions)
    {
        return new WslDistributionDiscoveryResult(distributions, Succeeded: true, string.Empty);
    }

    public static WslDistributionDiscoveryResult Failure(string userSafeMessage)
    {
        return new WslDistributionDiscoveryResult([], Succeeded: false, userSafeMessage);
    }
}
