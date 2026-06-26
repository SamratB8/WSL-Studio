namespace WslStudio.Application.Wsl;

public interface IWslDistributionDiscoveryService
{
    Task<WslDistributionDiscoveryResult> GetDistributionsAsync(CancellationToken cancellationToken);

    Task<WslEnvironmentResult> GetEnvironmentInfoAsync(CancellationToken cancellationToken);
}
