using WslStudio.Application.Commands;
using WslStudio.Core.Wsl;

namespace WslStudio.Application.Wsl;

public sealed class WslDistributionDiscoveryService(
    ICommandRunner commandRunner,
    IWslDistributionParser distributionParser) : IWslDistributionDiscoveryService
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    public async Task<WslDistributionDiscoveryResult> GetDistributionsAsync(CancellationToken cancellationToken)
    {
        CommandResult result = await commandRunner.RunAsync(
            new CommandRequest("wsl.exe", ["--list", "--verbose"], DefaultTimeout),
            cancellationToken);

        if (!result.Succeeded)
        {
            return WslDistributionDiscoveryResult.Failure(ToUserSafeMessage(result));
        }

        WslDistributionParseResult parseResult = distributionParser.ParseListVerbose(result.StandardOutput);
        return parseResult.Succeeded
            ? WslDistributionDiscoveryResult.Success(parseResult.Distributions)
            : WslDistributionDiscoveryResult.Failure(parseResult.UserSafeMessage);
    }

    public async Task<WslEnvironmentResult> GetEnvironmentInfoAsync(CancellationToken cancellationToken)
    {
        CommandResult statusResult = await commandRunner.RunAsync(
            new CommandRequest("wsl.exe", ["--status"], DefaultTimeout),
            cancellationToken);

        if (!statusResult.Succeeded)
        {
            return WslEnvironmentResult.Failure(ToUserSafeMessage(statusResult));
        }

        CommandResult versionResult = await commandRunner.RunAsync(
            new CommandRequest("wsl.exe", ["--version"], DefaultTimeout),
            cancellationToken);

        if (!versionResult.Succeeded)
        {
            return WslEnvironmentResult.Failure(ToUserSafeMessage(versionResult));
        }

        return WslEnvironmentResult.Success(new WslEnvironmentInfo(
            statusResult.StandardOutput,
            versionResult.StandardOutput,
            UserSafeMessage: null));
    }

    private static string ToUserSafeMessage(CommandResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.UserSafeErrorMessage))
        {
            return result.UserSafeErrorMessage;
        }

        return result.ExitCode is 0
            ? string.Empty
            : "WSL returned an error while retrieving distribution information.";
    }
}
