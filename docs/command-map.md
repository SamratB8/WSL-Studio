# WSL Command Map

This document maps planned WSL Studio features to official WSL commands wherever possible. It is a planning reference for implementation and review.

WSL Studio should prefer supported `wsl.exe` behavior and avoid direct modification of WSL internals.

## Command Mapping Principles

- Commands should be constructed through a centralized command builder.
- Arguments should be passed without shell interpolation.
- Output parsing should be isolated and covered by tests.
- Destructive commands should be classified and routed through confirmation policies.
- The application should expose raw command output for troubleshooting when useful.
- Unsupported or undocumented WSL internals should not be used as implementation shortcuts.

## Planned Feature Map

| Planned feature | Likely WSL command | Safety classification | Notes |
| --- | --- | --- | --- |
| List installed distributions | `wsl.exe --list --verbose` or `wsl.exe -l -v` | Read-only | Used to discover name, running state, default distribution, and version where available. Parser must tolerate localized or changed output where practical. |
| Show available online distributions | `wsl.exe --list --online` or `wsl.exe -l -o` | Read-only | Candidate for install workflows. Availability depends on WSL version and network behavior outside the app. |
| Show WSL status | `wsl.exe --status` | Read-only | Useful for diagnostics and default WSL configuration visibility. |
| Show WSL version information | `wsl.exe --version` | Read-only | Availability may vary by Windows and WSL installation state. |
| Launch default distribution | `wsl.exe` | Starts process | Should be treated as user-initiated launch, not background management. |
| Launch specific distribution | `wsl.exe --distribution <name>` or `wsl.exe -d <name>` | Starts process | Distribution name must come from trusted discovery or validated input. |
| Run a command in a distribution | `wsl.exe --distribution <name> -- <command>` | Sensitive | Not planned as an early general-purpose feature. Requires careful command and quoting design. |
| Terminate a distribution | `wsl.exe --terminate <name>` or `wsl.exe -t <name>` | Interruptive | Can stop running processes inside the distribution. Should require a clear confirmation or at least an interruptive-action prompt. |
| Shut down all WSL distributions | `wsl.exe --shutdown` | Interruptive | Affects all running distributions and WSL services. Should clearly communicate scope. |
| Set default distribution | `wsl.exe --set-default <name>` or `wsl.exe -s <name>` | Configuration change | Should validate the target distribution exists. |
| Set distribution WSL version | `wsl.exe --set-version <name> <version>` | Configuration change | Can be long-running and may fail depending on environment. Needs progress and failure handling. |
| Install distribution | `wsl.exe --install` with supported options | System change | Exact options should be verified during implementation against current WSL documentation. |
| Export distribution | `wsl.exe --export <name> <file>` | Backup operation | Should validate output path and warn about time and disk usage. |
| Import distribution | `wsl.exe --import <name> <installLocation> <file>` | System change | Should validate distribution name, install location, and source file. |
| Unregister distribution | `wsl.exe --unregister <name>` | Destructive | Removes the distribution registration and data managed by WSL. Requires explicit confirmation and should recommend export first. |
| Import distribution from VHD | `wsl.exe --import-in-place <name> <vhdx>` | System change | Candidate for later phases only after validating support and safety implications. |
| Mount disk | `wsl.exe --mount` | Privileged or advanced | Not planned for the initial release. Requires separate security review. |
| Unmount disk | `wsl.exe --unmount` | Privileged or advanced | Not planned for the initial release. Requires separate security review. |

## Feature Areas Requiring Extra Review

### General Command Execution

Running arbitrary Linux commands through WSL is powerful but broad. If added in the future, it should be treated as a terminal-like feature rather than a hidden implementation detail.

Review requirements:

- Clear user intent.
- No hidden command execution.
- Careful argument handling.
- No automatic elevation.
- Clear display of command output and exit status.

### Configuration Editing

WSL configuration can involve `.wslconfig`, `wsl.conf`, and command-level settings. Configuration features should use official documentation and should validate edits before writing files.

Review requirements:

- Distinguish Windows-level WSL configuration from per-distribution Linux configuration.
- Avoid editing distribution files without clear user consent.
- Back up user-editable configuration files before writing changes where practical.
- Show validation failures before applying changes.

### Advanced Disk Operations

Commands such as mount and unmount can affect disks and require careful handling. These are not planned for the initial release.

Review requirements:

- Separate design document.
- Clear privilege model.
- Strong confirmation flow.
- Recovery guidance.
- Manual testing matrix.

## Implementation Notes

The application should model command execution as structured requests:

- Executable path.
- Argument list.
- Working directory where relevant.
- Timeout or cancellation token.
- Expected safety classification.
- Standard output and error capture.
- Exit code.

The UI should not assemble command text. Displayed command previews may be useful, but they should be generated from the same structured request used for execution.
