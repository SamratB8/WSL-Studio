namespace WslStudio.Application.Wsl;

public interface IWslDashboardOverviewService
{
    Task<WslDashboardOverviewResult> GetOverviewAsync(CancellationToken cancellationToken);
}
