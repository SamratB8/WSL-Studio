# Architecture

This document describes the planned architecture for WSL Studio. It is a Phase 0 planning document and does not define completed code or project files.

## Architectural Principles

WSL Studio should be built as a layered desktop application with clear ownership boundaries. The application should remain understandable to contributors, testable without running WSL for most business logic, and conservative about system-level operations.

Core principles:

- UI code should not build command-line strings directly.
- WSL state should be queried from official WSL behavior when accuracy matters.
- Application-owned data should be separated from WSL-managed state.
- Destructive operations should pass through explicit safety policies.
- Parsing and process execution should be isolated and tested.
- Features should be added through small, reviewable modules.

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

## Responsibilities of Each Future Project

The exact solution structure will be created in Phase 1. A likely structure is:

### WslStudio.App

The WinUI 3 desktop application.

Responsibilities:

- App startup.
- Windowing and navigation.
- Views and controls.
- Dependency injection composition.
- App-level resources and theme integration.

### WslStudio.Presentation

Presentation models and view models, if separated from the app project.

Responsibilities:

- View model state.
- Commands bound from the UI.
- UI-ready formatting.
- Presentation-level validation messages.

### WslStudio.Application

Workflow orchestration and use cases.

Responsibilities:

- Distribution management workflows.
- Import/export workflows.
- Safety policy enforcement.
- Operation status handling.
- Coordination between domain and infrastructure services.

### WslStudio.Domain

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
