# WSL Studio Blueprint

This document describes the initial product and repository blueprint for WSL Studio. It is intended to guide Phase 1 scaffolding and later implementation work without committing the project to unsupported or premature functionality.

## Project Goals

WSL Studio aims to provide a native Windows 11 management experience for Windows Subsystem for Linux. The application should make common WSL tasks easier to inspect and perform while staying aligned with official WSL behavior.

The primary goals are:

- Present installed WSL distributions in a clear desktop interface.
- Map management actions to official WSL commands and supported APIs wherever possible.
- Make destructive actions visible, deliberate, and reversible where practical.
- Provide useful status, command output, and error information.
- Avoid direct modification of WSL internals.
- Keep the application fast to start and light when idle.
- Establish a maintainable architecture that can support future features.

Non-goals for the initial release include:

- Replacing the WSL command line.
- Acting as a Linux package manager.
- Editing distribution filesystems directly.
- Running an always-on background service unless a future feature clearly requires it.
- Supporting undocumented WSL internals.

## Module Breakdown

The planned implementation can be organized into the following logical modules. Exact project names will be finalized during Phase 1.

### Desktop App

The WinUI 3 application will host the user interface, navigation, command surfaces, dialogs, and view models. It should remain focused on presentation and user interaction rather than direct process execution or persistence details.

Expected responsibilities:

- Windows 11 Fluent UI layout and controls.
- Distribution list and detail views.
- Operation dialogs and confirmation flows.
- Status presentation, progress indicators, and error surfaces.
- User preferences exposed through validated settings screens.

### WSL Integration

The WSL integration layer will encapsulate calls to official WSL functionality. It should provide typed results to the rest of the application and isolate command-line parsing from UI code.

Expected responsibilities:

- Running supported `wsl.exe` commands.
- Parsing distribution lists and status output.
- Returning structured success, failure, cancellation, and diagnostic data.
- Centralizing command construction and argument validation.
- Avoiding shell-specific behavior when invoking WSL commands.

### Domain Model

The domain layer will describe WSL Studio concepts independently from the UI and process runner.

Expected responsibilities:

- Distribution metadata.
- Operation definitions and result types.
- Safety classifications for commands.
- Validation rules for user-supplied paths, names, and options.
- Future abstractions for configuration and diagnostics.

### Persistence

SQLite is planned for application-owned data. It should store WSL Studio preferences, operation history metadata, and cached non-sensitive state where useful.

Expected responsibilities:

- User preferences.
- Recent export/import locations, subject to privacy review.
- Operation history summaries.
- Application diagnostics metadata.

Persistence must not become a source of truth for WSL state. WSL state should be queried from WSL itself when accuracy matters.

### Testing

xUnit is planned for automated tests. Early test coverage should focus on command construction, parsing, validation, safety policies, and view model behavior.

Expected responsibilities:

- Unit tests for command mapping and validation.
- Parser tests using representative WSL command output.
- Safety policy tests for destructive operations.
- View model tests where behavior can be tested without a UI runtime.

## Major Features

The following features are planned candidates for the first public release. They may be adjusted as WSL behavior, Windows App SDK capabilities, or implementation complexity become clearer.

### Distribution Overview

The application should show installed distributions, their running state, version, default status, and basic metadata available through supported WSL commands.

### Distribution Lifecycle

Users should be able to start, terminate, shut down, and launch distributions. Actions should show progress or completion state and expose failure information.

### Default Distribution Management

Users should be able to set the default WSL distribution using official WSL functionality.

### Import and Export

WSL Studio should support export and import workflows using official WSL commands. These workflows should validate paths, explain the operation, and preserve command output for troubleshooting.

### Unregister and Destructive Actions

Destructive actions such as unregistering a distribution must require explicit confirmation. Confirmation text should identify the target distribution and the expected consequence.

### Settings and Configuration

Settings should focus on application preferences and supported WSL configuration workflows. The application should validate configuration edits before applying them and avoid direct undocumented changes.

### Diagnostics

Diagnostics should help users and maintainers understand failures without collecting unnecessary sensitive information. Logs should be local by default and should distinguish application failures from WSL command failures.

## High-Level Architecture

At a high level, WSL Studio should be structured as:

```text
WinUI 3 Desktop App
        |
Presentation and View Models
        |
Application Services
        |
Domain Model and Safety Policies
        |
WSL Integration / Process Execution
        |
Official WSL Commands and Windows Facilities
```

The UI should ask application services to perform actions. Application services should validate requests, apply safety policies, call the WSL integration layer, and return structured results. The WSL integration layer should be the only area responsible for constructing and executing WSL commands.

This separation is intended to keep the application testable, reduce accidental coupling to command output formats, and make future extension possible without rewriting the UI.
