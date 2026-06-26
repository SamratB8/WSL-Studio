namespace WslStudio.Application.Commands;

public interface ICommandRunner
{
    Task<CommandResult> RunAsync(CommandRequest request, CancellationToken cancellationToken);
}
