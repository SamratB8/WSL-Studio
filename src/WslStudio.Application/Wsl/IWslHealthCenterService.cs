namespace WslStudio.Application.Wsl;

public interface IWslHealthCenterService
{
    Task<WslHealthCenterResult> GetHealthAsync(CancellationToken cancellationToken);
}
