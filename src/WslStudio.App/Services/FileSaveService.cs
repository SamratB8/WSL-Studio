using Windows.Storage;
using Windows.Storage.Pickers;

namespace WslStudio.App.Services;

/// <summary>
/// Saves text using the WinUI <see cref="FileSavePicker"/>. The picker is initialized with the main
/// window handle as required for unpackaged Windows App SDK apps. Content is written as UTF-8.
/// </summary>
public sealed class FileSaveService(MainWindow window) : IFileSaveService
{
    public async Task<FileSaveResult> SaveTextAsync(
        string suggestedFileName,
        string fileTypeDescription,
        string fileExtension,
        string content)
    {
        try
        {
            FileSavePicker picker = new()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = suggestedFileName
            };
            picker.FileTypeChoices.Add(fileTypeDescription, [fileExtension]);

            nint windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, windowHandle);

            StorageFile? file = await picker.PickSaveFileAsync();
            if (file is null)
            {
                return FileSaveResult.Canceled();
            }

            await FileIO.WriteTextAsync(file, content);
            return FileSaveResult.Saved(file.Path);
        }
        catch (Exception exception)
        {
            return FileSaveResult.Failed(exception.Message);
        }
    }
}
