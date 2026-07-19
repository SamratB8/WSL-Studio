using System.Diagnostics;
using System.Text;

namespace WslStudio.App.Diagnostics;

/// <summary>
/// Minimal crash logging for otherwise opaque startup failures.
/// </summary>
/// <remarks>
/// WinUI surfaces many startup faults as stowed COM exceptions (0xC000027B) with no managed detail,
/// which makes issues such as a missing dependency registration very hard to diagnose. Recording the
/// managed exception chain to a local file turns those into actionable reports. Only exception
/// metadata is written; environment variable values, secrets, and user data are never recorded.
/// </remarks>
internal static class StartupDiagnostics
{
    private static readonly object Gate = new();

    public static void LogException(string source, Exception? exception)
    {
        StringBuilder builder = new();
        builder.AppendLine($"{DateTimeOffset.UtcNow:u} {source}");

        int depth = 0;
        for (Exception? current = exception; current is not null; current = current.InnerException, depth++)
        {
            builder.AppendLine($"  [{depth}] {current.GetType().FullName}");
            builder.AppendLine($"       HRESULT: 0x{current.HResult:X8}");
            builder.AppendLine($"       Message: {current.Message}");
            builder.AppendLine($"       Stack  : {current.StackTrace}");
        }

        if (exception is null)
        {
            builder.AppendLine("  (no managed exception object was supplied)");
        }

        string text = builder.ToString();
        Debug.WriteLine(text);

        try
        {
            lock (Gate)
            {
                string directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "WSL Studio");

                Directory.CreateDirectory(directory);
                File.AppendAllText(
                    Path.Combine(directory, "startup-diagnostic.log"),
                    text + Environment.NewLine,
                    new UTF8Encoding(false));
            }
        }
        catch
        {
            // Diagnostics must never break the application.
        }
    }
}
