# Security Model

This document describes the planned security and safety model for WSL Studio. It focuses on user protection, predictable behavior, and careful handling of WSL operations.

## Safety Philosophy

WSL Studio should make WSL management clearer without hiding the consequences of system-level actions. The application should favor explicit user intent, conservative defaults, and recoverability.

Core safety principles:

- Use official WSL functionality whenever possible.
- Do not modify WSL internals directly.
- Avoid hidden background operations.
- Do not run commands automatically unless the user initiated the workflow or explicitly enabled the behavior.
- Prefer read-only discovery before mutation.
- Treat user data in WSL distributions as valuable and potentially irreplaceable.
- Make destructive actions deliberate.

## Operation Classification

Operations should be classified before implementation and review.

### Read-Only

Read-only operations inspect WSL state without changing it.

Examples:

- Listing distributions.
- Checking WSL status.
- Reading WSL version information.

Expected behavior:

- No confirmation required.
- Failures should be displayed clearly.
- Output should be parsed through tested infrastructure.

### Starts Process

These operations launch WSL or a distribution but do not intentionally modify configuration.

Examples:

- Launching the default distribution.
- Launching a specific distribution.

Expected behavior:

- User intent must be clear.
- The app should not launch distributions unexpectedly on startup.
- Failure output should be visible or accessible.

### Interruptive

Interruptive operations may stop running processes or affect active work.

Examples:

- Terminating a distribution.
- Shutting down all WSL distributions.

Expected behavior:

- The UI should communicate what will be stopped.
- Confirmation should be required when the scope is broad or user work may be interrupted.
- The result should indicate whether the operation succeeded or failed.

### Configuration Change

Configuration changes alter WSL behavior or application preferences.

Examples:

- Setting the default distribution.
- Changing distribution WSL version.
- Editing supported WSL configuration files.

Expected behavior:

- Validate inputs before applying changes.
- Show the target and expected effect.
- Preserve previous values where practical.
- Report failures without implying partial success.

### Destructive

Destructive operations can remove distribution data or make recovery impossible without a backup.

Examples:

- Unregistering a distribution.
- Deleting application-owned records that cannot be recovered.

Expected behavior:

- Require explicit confirmation.
- Identify the target by name.
- Explain that data may be removed.
- Recommend or offer export before proceeding where supported.
- Log operation metadata without logging sensitive content.

## Destructive Action Policy

Destructive actions must use a dedicated confirmation flow. A general dialog is not enough for operations that can remove data.

Minimum requirements:

- Show the exact distribution or resource name.
- Explain the consequence in direct language.
- Require an affirmative action that is distinct from ordinary navigation.
- Prevent accidental confirmation through default focused buttons where practical.
- Re-check that the target still exists before execution.
- Display the final result and any WSL error output.

For unregister operations, the application should strongly encourage exporting the distribution first. It should not claim that unregister can be undone unless a valid backup exists.

## Backup Strategy

WSL Studio should treat export as the primary supported backup workflow for distributions.

Planned behavior:

- Provide export workflows before destructive distribution operations.
- Validate export destination paths.
- Warn when a destination file already exists.
- Surface disk-space and permission failures clearly when WSL reports them.
- Keep backup operation history limited to non-sensitive metadata such as distribution name, timestamp, and file path if the user has not disabled such history.

The application should not inspect or copy Linux filesystem contents directly as a backup mechanism in early releases.

## Logging Philosophy

Logs should help diagnose failures while minimizing unnecessary data collection.

Logging principles:

- Logs are local by default.
- Do not send telemetry by default.
- Do not log secrets, environment variables, Linux file contents, or command payloads that may contain sensitive data.
- Record operation category, timestamp, exit code, and high-level failure information.
- Include WSL standard error when useful, with care for sensitive content.
- Make logs understandable enough for issue reports without requiring users to expose private distribution data.

If telemetry is ever considered, it should require a separate design, privacy review, documentation update, and explicit user control.

## Configuration Validation

Configuration workflows should validate inputs before writing changes.

Validation should cover:

- Distribution names.
- File and directory paths.
- WSL version values.
- Import and export file extensions where relevant.
- Conflicting options.
- Unsupported or unknown configuration keys.

When editing user-owned configuration files, the application should:

- Read the current file before writing.
- Preserve formatting where practical.
- Create a backup before modification when reasonable.
- Refuse to write invalid configuration.
- Show validation errors in the UI.

## Elevation and Permissions

WSL Studio should avoid requiring elevation for normal workflows. If a future feature requires administrative privileges, it should be isolated, documented, and reviewed separately.

Expected behavior:

- Do not silently elevate.
- Explain why elevation is required.
- Keep elevated operations narrow in scope.
- Avoid adding a background service solely for convenience.

## Threats and Mitigations

| Risk | Mitigation |
| --- | --- |
| Accidental distribution deletion | Dedicated destructive confirmation flow and export recommendation. |
| Incorrect command construction | Centralized command builder and tests. |
| Shell injection through names or paths | Pass arguments directly without shell interpolation and validate inputs. |
| Stale UI state | Refresh from WSL before sensitive operations. |
| Sensitive data in logs | Minimize logs and avoid recording command payloads or file contents. |
| Unsupported WSL behavior | Prefer official commands and document unsupported feature requests. |

The security model should be reviewed whenever a new operation category is added.
