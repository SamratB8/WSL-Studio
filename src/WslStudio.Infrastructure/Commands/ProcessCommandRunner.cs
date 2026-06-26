using System.ComponentModel;
using System.Diagnostics;
using WslStudio.Application.Commands;

namespace WslStudio.Infrastructure.Commands;

public sealed class ProcessCommandRunner : ICommandRunner
{
    public async Task<CommandResult> RunAsync(CommandRequest request, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        using Process process = new()
        {
            StartInfo = CreateStartInfo(request)
        };

        try
        {
            process.Start();

            Task<string> standardOutputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            Task<string> standardErrorTask = process.StandardError.ReadToEndAsync(cancellationToken);

            using CancellationTokenSource timeoutTokenSource = new(request.Timeout);
            using CancellationTokenSource linkedTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token);

            try
            {
                await process.WaitForExitAsync(linkedTokenSource.Token);
            }
            catch (OperationCanceledException) when (timeoutTokenSource.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                TryKill(process);
                stopwatch.Stop();
                return CommandResult.Timeout(request.CommandName, request.Arguments, stopwatch.Elapsed);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                TryKill(process);
                stopwatch.Stop();
                return CommandResult.Failure(
                    request.CommandName,
                    request.Arguments,
                    "The WSL command was canceled.",
                    stopwatch.Elapsed);
            }

            string standardOutput = await standardOutputTask;
            string standardError = await standardErrorTask;

            stopwatch.Stop();

            return CommandResult.Success(
                request.CommandName,
                request.Arguments,
                standardOutput,
                standardError,
                process.ExitCode,
                stopwatch.Elapsed);
        }
        catch (Win32Exception)
        {
            stopwatch.Stop();
            return CommandResult.Failure(
                request.CommandName,
                request.Arguments,
                "WSL is not installed or wsl.exe could not be found.",
                stopwatch.Elapsed);
        }
        catch (InvalidOperationException)
        {
            stopwatch.Stop();
            return CommandResult.Failure(
                request.CommandName,
                request.Arguments,
                "WSL could not be started.",
                stopwatch.Elapsed);
        }
    }

    private static ProcessStartInfo CreateStartInfo(CommandRequest request)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = request.CommandName,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (string argument in request.Arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        return startInfo;
    }

    private static void TryKill(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch (InvalidOperationException)
        {
        }
        catch (Win32Exception)
        {
        }
    }
}
