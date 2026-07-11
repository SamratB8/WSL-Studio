# Architecture

This document describes the architecture of WSL Studio. The layered structure described here is implemented in the solution (Core, Application, Infrastructure, and the WinUI 3 App). The application is currently **read-only** with respect to WSL state; sections that describe mutating or destructive operations are forward-looking and are gated behind later phases and the security model.

## Architectural Principles

WSL Studio is built as a layered desktop application with clear ownership boundaries. The application remains understandable to contributors, testable without running WSL for most business logic, and conservative about system-level operations.

Core principles:

- The UI does not call `wsl.exe` directly and does not build command-line strings.
- ViewModels call application services; they do not execute processes or parse command output.
- The application layer defines use cases and owns output parsing into structured models.
- Infrastructure executes official WSL commands through the command runner and is the only layer that starts processes.
- Core remains independent of the UI and of process execution.
- WSL state is queried from official WSL behavior when accuracy matters; application-owned data is kept separate from WSL-managed state.
- The application is read-only today. Destructive or state-changing operations are deferred to later phases and must pass through explicit safety policies.
- Parsing and process execution are isolated and tested; features are added through small, reviewable modules.

## Planned Layers

### Presentation Layer

The presentation layer will be the WinUI 3 desktop application.

Responsibilities:

- Application shell and navigation.
- Windows 11 Fluent Design implementation.
- Views, controls, dialogs, and layout.
- View models and UI state.
- User interaction patterns for confirmations, errors, and progress.

The presentation layer should call application services rather than invoking WSL directly. It should receive structured models and operation results that are already safe to display.

### Application Layer

The application layer will coordinate user-facing workflows.

Responsibilities:

- Distribution list refresh.
- Lifecycle operations such as start, launch, terminate, and shutdown.
- Import, export, and unregister workflows.
- Settings workflows.
- Confirmation requirements and operation orchestration.
- Translating domain results into presentation-friendly outcomes.

This layer is where use cases live. It should be thin enough to remain understandable but explicit enough that workflows can be tested independently from the UI.

### Domain Layer

The domain layer will contain application concepts that do not depend on WinUI or process execution.

Responsibilities:

- Distribution entities and value objects.
- Operation request and result types.
- Safety classifications.
- Validation rules.
- Error categories.
- Policy decisions for destructive or sensitive actions.

The domain layer should have minimal dependencies and should be the easiest part of the system to test.

### Infrastructure Layer

The infrastructure layer will connect WSL Studio to external systems.

Responsibilities:

- Process execution.
- Official WSL command invocation.
- Output parsing.
- SQLite persistence.
- File path validation helpers.
- Local logging implementation.
- Windows-specific integration points.

Infrastructure code should be hidden behind interfaces used by the application layer. This allows tests to substitute fake WSL command runners and controlled persistence implementations.

## Responsibilities of Each Project

The solution is organized into the following projects:

### WslStudio.App

The WinUI 3 desktop application.

Responsibilities:

- App startup.
- Windowing and navigation.
- Views and controls.
- View models and UI state (CommunityToolkit.Mvvm), including commands bound from the UI, UI-ready formatting, and presentation-level validation messages.
- Dependency injection composition.
- App-level resources and theme integration.

View models live in this project rather than a separate presentation assembly. They should remain thin and call application services rather than executing processes or parsing command output.

### WslStudio.Application

Workflow orchestration and use cases.

Responsibilities:

- Distribution discovery, dashboard, details, and health workflows.
- Output parsing into structured models.
- Safety policy enforcement (for future mutating operations).
- Operation status handling.
- Coordination between core models and infrastructure services.

### WslStudio.Core

Core models and rules.

Responsibilities:

- Distribution model.
- Operation models.
- Safety and validation rules.
- Result and error types.

### WslStudio.Infrastructure

External integration implementations.

Responsibilities:

- `wsl.exe` command runner.
- WSL output parsers.
- SQLite data access.
- File system and path checks.
- Logging implementation.

### WslStudio.Tests

xUnit test projects.

Responsibilities:

- Domain tests.
- Application workflow tests.
- Parser tests.
- Command construction tests.
- Regression tests for safety policies.

## WSL Command Boundary

WSL Studio should treat WSL as an external system. The command boundary should be narrow, typed, and audited.

The command runner should:

- Avoid shell interpolation.
- Pass executable names and arguments explicitly.
- Capture standard output, standard error, exit code, and cancellation state.
- Classify errors without hiding raw diagnostic details.
- Record enough context for troubleshooting without logging unnecessary sensitive data.

Command construction should be centralized. New commands should be reviewed against the command map and security model before use.

## Persistence Boundary

SQLite should store application-owned data only. It should not attempt to mirror the full WSL state or become a replacement for WSL queries.

Appropriate persistence candidates:

- User preferences.
- Non-sensitive operation history.
- Recently used application options.
- Cached display metadata that can be refreshed.

Inappropriate persistence candidates:

- Linux filesystem contents.
- Secrets from distributions.
- Assumptions about distribution state that WSL has not confirmed.
- Undocumented WSL internal paths or metadata.

## Future Extensibility

The architecture should support future additions without forcing the application into a large rewrite.

Possible extension areas include:

- More detailed diagnostics.
- Optional terminal integration.
- Enhanced configuration editing.
- Distribution templates.
- Export profiles.
- Plugin-like internal feature modules.

Extensibility should be introduced gradually. The project should avoid premature plugin systems or broad abstractions before there is a clear need.

## Testing Strategy

Testing should begin with the most stable logic:

- Command argument construction.
- Output parsing.
- Validation rules.
- Safety policy behavior.
- Application workflows using fake command runners.

Integration tests that require WSL should be added carefully and should not be required for every contributor unless the environment is clearly documented.
