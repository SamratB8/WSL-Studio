namespace WslStudio.Core.Wsl;

public sealed record WslHealthCheck(
    string Name,
    WslHealthCheckCategory Category,
    WslHealthCheckStatus Status,
    string Summary,
    string Details,
    string Recommendation,
    Uri? DocumentationLink = null,
    IReadOnlyDictionary<string, string>? TechnicalMetadata = null);
