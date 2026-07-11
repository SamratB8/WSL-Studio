using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public interface IWslDistributionDetailsService
{
    Task<WslDistributionDetailsResult> GetDetailsAsync(
        DistributionName distributionName,
        CancellationToken cancellationToken);
}
