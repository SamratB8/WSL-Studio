namespace WslStudio.App.Services;

public enum FileSaveOutcome
{
    Saved,
    Canceled,
    Failed
}

/// <summary>
/// Outcome of a user-driven file save operation.
/// </summary>
public sealed record FileSaveResult(FileSaveOutcome Outcome, string? Path, string? ErrorMessage)
{
    public static FileSaveResult Saved(string path) => new(FileSaveOutcome.Saved, path, null);

    public static FileSaveResult Canceled() => new(FileSaveOutcome.Canceled, null, null);

    public static FileSaveResult Failed(string errorMessage) => new(FileSaveOutcome.Failed, null, errorMessage);
}
