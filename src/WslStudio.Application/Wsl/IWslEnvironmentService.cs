namespace WslStudio.Application.Wsl;

/// <summary>
/// Provides a consolidated, read-only view of the local WSL environment for the Environment page.
/// </summary>
public interface IWslEnvironmentService
{
    Task<WslEnvironmentOverviewResult> GetEnvironmentAsync(CancellationToken cancellationToken);
}
