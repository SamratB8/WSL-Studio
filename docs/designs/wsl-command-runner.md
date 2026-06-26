# WSL Command Runner Design

## Purpose

WSL Studio needs a dedicated command runner abstraction because WSL operations are system-facing, failure-prone, and safety-sensitive. Calling `wsl.exe` directly from UI code would couple presentation logic to process execution, command construction, output parsing, timeout handling, and error reporting.

The command runner should provide a narrow boundary between the application and official WSL tooling. It should make command execution consistent, testable, auditable, and safe to evolve as more WSL workflows are added.

The abstraction should help ensure that:

- UI code does not construct command-line arguments.
- WSL process execution is centralized and reviewable.
- Commands are validated before execution.
- Output, errors, exit codes, and timeouts are handled consistently.
- Future tests can exercise command behavior without requiring WSL to be installed.

## Scope

The command runner is responsible for safely invoking official WSL commands. Initial read-only commands are expected to include:

- `wsl.exe --status`
- `wsl.exe --version`
- `wsl.exe --list --verbose`

The command runner should not directly modify WSL internals. It must not edit registry entries, WSL-managed metadata, virtual disks, or undocumented storage locations as a replacement for official WSL behavior.

The first implementation should focus on read-only discovery and diagnostics. Mutating or destructive commands, such as terminating, shutting down, importing, exporting, or unregistering distributions, should be added only after confirmation flows and safety policies are in place.

## Architecture Placement

The command runner should follow the existing Clean Architecture boundaries.

### `WslStudio.Core`

`WslStudio.Core` should contain domain models that are independent of UI, process execution, and Windows-specific APIs.

Appropriate responsibilities:

- Distribution-related domain models.
- Value objects for validated distribution names.
- Operation classification models.
- Error categories that are meaningful to the domain.

### `WslStudio.Application`

`WslStudio.Application` should contain command execution interfaces and use cases. This layer should define what the application needs from WSL without deciding how processes are started.

Appropriate responsibilities:

- Interfaces for running WSL commands.
- Use cases such as retrieving WSL status or listing distributions.
- Request and result models used by application workflows.
- Validation orchestration before a command is requested.
- Mapping infrastructure failures into application-level outcomes.

### `WslStudio.Infrastructure`

`WslStudio.Infrastructure` should contain the process execution implementation.

Appropriate responsibilities:

- Starting `wsl.exe` with explicit arguments.
- Capturing standard output and standard error.
- Enforcing timeouts and cancellation.
- Measuring duration.
- Returning structured command results.
- Handling process-level failures such as executable-not-found errors.

Infrastructure code should avoid shell interpolation. It should use structured process start APIs with executable and argument values passed separately.

### `WslStudio.App`

`WslStudio.App` should consume application use cases through ViewModels. ViewModels should not call `wsl.exe`, create process runners, parse command output, or construct command arguments.

Appropriate responsibilities:

- Requesting application use cases.
- Presenting loading, success, empty, and failure states.
- Showing user-safe error messages.
- Routing destructive actions through confirmation UI before application use cases are invoked.

## Process execution policy

WSL Studio shall execute `wsl.exe` directly using ProcessStartInfo.

The application shall not invoke:

- cmd.exe /c
- powershell.exe -Command
- bash -c

unless there is a documented architectural reason.

Executing the target executable directly reduces quoting issues,
avoids unnecessary shell interpretation,
improves security,
and simplifies error handling.

## Command categories

Read-only

- wsl --status
- wsl --version
- wsl --list --verbose

Safe operations

- wsl -d Ubuntu
- wsl --shutdown

Potentially destructive

- wsl --unregister
- wsl --import
- wsl --export

## Command Result Model

The planned command result shape should preserve enough detail for diagnostics while keeping UI consumption simple.

Planned fields:

- Command name, such as `wsl.exe`.
- Arguments as a structured list.
- Standard output.
- Standard error.
- Exit code, when available.
- Duration.
- Success or failure state.
- Timeout state.
- User-safe error message.

The result should distinguish process execution failures from WSL command failures. For example, `wsl.exe` not being found is different from `wsl.exe --status` returning a non-zero exit code.

Illustrative shape:

```text
CommandResult
  CommandName
  Arguments
  StandardOutput
  StandardError
  ExitCode
  Duration
  Succeeded
  TimedOut
  UserMessage
```

This is a design shape, not a required implementation type.

## Command lifecycle

ViewModel
    ↓
Application Use Case
    ↓
ICommandRunner
    ↓
Infrastructure Process Runner
    ↓
ProcessStartInfo
    ↓
wsl.exe
    ↓
CommandResult
    ↓
Application Mapping
    ↓
ViewModel

## Non-goals

The command runner is not responsible for:

- Parsing WSL command output into domain models.
- Deciding whether a command should be executed.
- Presenting UI messages.
- Requesting user confirmation.
- Persisting command history.
- Managing application state.

Those responsibilities belong to higher architectural layers.

## Safety Rules

The command runner and surrounding application workflows should follow these rules:

- Never execute commands built from unsafe user input.
- Validate distribution names before using them as command arguments.
- Apply timeouts to all command execution.
- Avoid shell interpolation.
- Avoid direct PowerShell execution unless a documented scenario requires it.
- Log command metadata without leaking sensitive content.
- Never run destructive commands without explicit user confirmation.
- Prefer read-only discovery before mutating state.
- Keep command construction centralized.
- Treat user-supplied paths as untrusted until validated.

Command logging should favor metadata such as command category, argument count, duration, exit code, and operation result. Logs should avoid recording secrets, Linux filesystem contents, environment variables, or arbitrary command payloads.

## Error Handling

The command runner should produce structured errors that application use cases can translate into user-facing states.

### WSL Not Installed

If WSL is unavailable, the application should show a clear message explaining that WSL is required for the requested operation. The UI should not imply that distributions can be managed until WSL is installed and available.

### `wsl.exe` Not Found

If the executable cannot be found, the infrastructure layer should return an executable-not-found result. The application layer can map this to a user-safe message and diagnostics metadata.

### Command Timeout

Timeouts should be reported distinctly from non-zero exit codes. The result should include timeout state and duration. The application should avoid retry loops unless a future design explicitly defines them.

### Non-Zero Exit Code

Non-zero exit codes should be treated as WSL command failures. Standard error should be captured for diagnostics, but the UI should show a concise user-safe message.

### Malformed Output

Malformed or unexpected output should be handled by parsers, not by the process runner itself. Parser failures should preserve the raw command result for diagnostics while returning a structured parse failure to the application layer.

### Permission Errors

Permission failures should be surfaced without automatically elevating privileges. If a future feature requires elevation, it should be covered by a separate design and safety review.

### Unsupported WSL Version

If a command or output format is not supported by the installed WSL version, the application should report that limitation clearly. Feature availability should be detected through official commands or documented behavior rather than assumptions.

## Testing Strategy

Most command runner behavior should be testable without requiring real WSL.

Recommended test areas:

- Parser tests using representative command output fixtures.
- Fake process runner tests for success, failure, timeout, and executable-not-found cases.
- Command construction tests that verify executable and arguments are passed separately.
- Application use case tests using command result fixtures.
- Error mapping tests for user-safe messages.

Integration tests that require real WSL should be added later and kept separate from the default unit test suite. They should be clearly documented because contributor environments may not have WSL installed or configured.

## Future Expansion

The command runner design should allow future expansion without broad rewrites.

Possible future areas:

- Docker command execution, if Docker integration is added later.
- PowerShell diagnostics for Windows-specific troubleshooting.
- Backup and export workflows using official WSL commands.
- Command audit logs for sensitive or destructive operations.
- Separate policies for read-only, mutating, interruptive, and destructive command categories.

Future expansion should continue to preserve the same boundaries: UI requests application use cases, application logic defines intent and safety policy, infrastructure performs process execution, and domain models remain independent of external process APIs.
