using WslStudio.Core.Wsl;

namespace WslStudio.Tests.Core.Wsl;

public sealed class WslHealthCheckTests
{
    [Fact]
    public void HealthCheck_CapturesRequiredAndOptionalFields()
    {
        Uri documentationLink = new("https://learn.microsoft.com/windows/wsl/");
        Dictionary<string, string> metadata = new()
        {
            ["Command"] = "wsl.exe --status"
        };

        WslHealthCheck check = new(
            "WSL version available",
            WslHealthCheckCategory.Wsl,
            WslHealthCheckStatus.Healthy,
            "WSL version is available.",
            "WSL version: 2.9.3.0",
            "No action is needed.",
            documentationLink,
            metadata);

        Assert.Equal("WSL version available", check.Name);
        Assert.Equal(WslHealthCheckCategory.Wsl, check.Category);
        Assert.Equal(WslHealthCheckStatus.Healthy, check.Status);
        Assert.Equal(documentationLink, check.DocumentationLink);
        Assert.Equal("wsl.exe --status", check.TechnicalMetadata?["Command"]);
    }
}
