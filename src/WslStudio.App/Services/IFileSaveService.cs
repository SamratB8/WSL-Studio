namespace WslStudio.App.Services;

/// <summary>
/// Saves text content to a user-chosen location using the Windows file save picker. The user always
/// chooses the destination; nothing is written silently and no elevation is required.
/// </summary>
public interface IFileSaveService
{
    Task<FileSaveResult> SaveTextAsync(
        string suggestedFileName,
        string fileTypeDescription,
        string fileExtension,
        string content);
}
