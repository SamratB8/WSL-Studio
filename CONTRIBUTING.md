# Contributing to WSL Studio

<<<<<<< HEAD
Thank you for considering a contribution to WSL Studio. The project is in active development, currently in **Phase 3 — WSL Workspace** (see the [roadmap](docs/roadmap.md)). The application is intentionally **read-only** with respect to WSL state during this phase, so contributions should focus on read-only discovery, diagnostics, inspection, workspace polish, documentation, tests, and architecture review.

Mutating or destructive WSL operations (start, stop, terminate, shutdown, import, export, unregister, configuration editing) are deferred to later phases and should not be introduced unless an issue explicitly schedules them.
=======
First off, thank you for your interest in contributing to **WSL
Studio**.

Whether you're fixing a typo, improving the UI, writing tests, reviewing
architecture, or implementing a new feature, your contribution is
appreciated.
>>>>>>> origin/main

WSL Studio aims to become a modern, safe, and extensible management
platform for Windows Subsystem for Linux (WSL). We value thoughtful
engineering, respectful collaboration, and high-quality contributions
over rapid feature growth.

------------------------------------------------------------------------

# Project Status

WSL Studio is currently in **Phase 3 -- WSL Workspace**.

The project already includes:

<<<<<<< HEAD
The implementation stack is C#, .NET 10, WinUI 3, Windows App SDK, CommunityToolkit.Mvvm, the .NET Generic Host, and xUnit. SQLite is planned for application-owned data in a later phase.

Contributors should follow these standards:

- Prefer clear, maintainable C# over clever abstractions.
- Keep UI code separate from WSL command execution; the UI does not call `wsl.exe` directly.
- Route WSL operations through reviewed application services; view models call services rather than executing processes or parsing output.
- Use structured process execution rather than shell-interpolated command strings.
- Add tests for parsing, validation, command construction, and safety policies.
- Keep public behavior documented when it affects users.
- Follow established .NET naming and formatting conventions.

## Pull Request Expectations
=======
-   WinUI 3 application shell
-   Clean Architecture
-   Generic Host + Dependency Injection
-   Official WSL command runner
-   Dashboard
-   Distribution discovery
-   Distribution Details
-   WSL Health Center
-   Environment Overview
-   Responsive Fluent UI
-   Automated unit tests
-   Architecture Decision Records (ADRs)

Current development is intentionally **read-only**.

Operations that modify WSL (start, stop, shutdown, unregister, import,
export, configuration editing, etc.) are intentionally deferred until
later roadmap phases.

------------------------------------------------------------------------

# Our Principles
>>>>>>> origin/main

Every contribution should support these principles:

-   Use **official Microsoft-supported WSL functionality** whenever
    possible.
-   Do not modify undocumented WSL internals.
-   Prefer correctness over convenience.
-   Prefer maintainability over cleverness.
-   Design for long-term extensibility.
-   Keep user data safe.
-   Preserve clear architecture boundaries.
-   Write code that is easy to review.

------------------------------------------------------------------------

# Branch Strategy

  |Branch|Purpose|
  |------------------|-----------------------|
  |`main`|Stable branch|
  |`dev`|Active development|
  |Feature branches|Individual work items|

Recommended branch names:

    feature/distribution-export
    feature/environment-page
    fix/navigation-layout
    fix/health-center
    docs/readme
    docs/roadmap
    refactor/parser
    test/discovery

Keep pull requests focused on a single concern.

------------------------------------------------------------------------

# Architecture

WSL Studio follows **Clean Architecture**.

    WslStudio.App
            ↓
    WslStudio.Application
            ↓
    WslStudio.Core
            ↓
    WslStudio.Infrastructure

## Responsibilities

### WslStudio.App

-   WinUI pages
-   ViewModels
-   Navigation
-   Reusable controls
-   Presentation state

Must **not**:

-   Execute WSL commands
-   Parse WSL output
-   Contain business logic

------------------------------------------------------------------------

### WslStudio.Application

<<<<<<< HEAD
During Phase 3 (WSL Workspace), the application stays read-only:

- Do not add commands that start, stop, terminate, shut down, import, export, or unregister distributions.
- Do not add configuration editing or other state-changing WSL operations.
- Do not introduce background services or performance monitoring that runs continuously.
- Do not add invented screenshots, badges, release artifacts, or fake production data.

GitHub Actions and MSIX packaging are scheduled for a later phase. These items, and any state-changing WSL operations, will be introduced when the roadmap and security model are ready for them.
=======
Contains:

-   Use cases
-   Services
-   Parsers
-   Query logic
-   Validation
-   Workflow orchestration

Should know **what** the application wants to do, not **how** Windows
executes it.

------------------------------------------------------------------------

### WslStudio.Core

Contains:

-   Domain models
-   Value objects
-   Enums
-   Domain concepts

Must remain independent of UI and infrastructure.

------------------------------------------------------------------------

### WslStudio.Infrastructure

Contains:

-   Process execution
-   External integrations
-   ICommandRunner implementation

Should not contain application workflow logic.

------------------------------------------------------------------------

# WSL Safety Rules

WSL Studio always prefers official WSL behavior.

Contributors must:

-   Execute `wsl.exe` directly.
-   Use structured process APIs.
-   Pass arguments separately.
-   Apply timeouts.
-   Support cancellation.
-   Return structured results.
-   Preserve diagnostics.

Avoid:

-   `cmd.exe /c`
-   `powershell.exe -Command`
-   `bash -c`

unless there is a documented architectural reason.

Never:

-   Edit undocumented registry keys.
-   Modify WSL virtual disks directly.
-   Manipulate internal metadata.
-   Bypass official WSL tooling.

------------------------------------------------------------------------

# Coding Standards

General expectations:

-   Modern C#
-   Clear naming
-   Small methods
-   Single responsibility
-   Minimal duplication
-   Meaningful comments
-   XML documentation for public APIs

Prefer readability over cleverness.

------------------------------------------------------------------------

# UI Guidelines

WSL Studio aims to feel like a first-party Windows application.

When changing UI:

-   Reuse the design system.
-   Prefer shared controls.
-   Avoid duplicated XAML.
-   Use responsive layouts.
-   Test narrow, medium, and wide window sizes.
-   Avoid unnecessary scrolling.
-   Keep terminology consistent.

Never hardcode colors when ThemeResources already exist.

------------------------------------------------------------------------

# Testing

Every meaningful change should include appropriate tests.

Common validation:

``` powershell
dotnet build WslStudio.slnx
dotnet test WslStudio.slnx --no-build
```

Expected:

-   0 build warnings
-   0 build errors
-   All tests passing

Test additions are expected for:

-   Parsers
-   Services
-   Query logic
-   Domain rules
-   Validation
-   Safety behavior

------------------------------------------------------------------------

# Pull Requests

A good pull request should include:

-   Summary
-   Motivation
-   Related issue(s)
-   Testing performed
-   Screenshots (for UI changes)
-   Architecture notes (if applicable)

Avoid combining unrelated work.

------------------------------------------------------------------------

# Issue Reports

Helpful issues include:

-   Bugs
-   Documentation improvements
-   UI polish
-   Accessibility
-   Performance
-   WSL compatibility
-   Feature proposals aligned with the roadmap

When reporting bugs include:

-   Windows version
-   WSL version
-   Steps to reproduce
-   Expected behavior
-   Actual behavior
-   Screenshots if relevant

Never post secrets or sensitive information.

------------------------------------------------------------------------

# Feature Stability

Features are categorized as:

  |Level|Meaning|
  |--------------|-------------------------------------|
  |Stable|Recommended for everyone|
  |Preview|Functional but evolving|
  |Experimental|Power-user features that may change|

Experimental features should always be opt-in.

------------------------------------------------------------------------

# Security

If you discover a security issue:

-   Please avoid opening a public issue immediately.
-   Contact the maintainers privately if practical.
-   Provide enough information to reproduce the problem.
-   Avoid publishing exploit details before a fix is available.

------------------------------------------------------------------------

# Long-Term Vision

The long-term vision extends beyond basic WSL management.

Future roadmap areas include:

-   Configuration Center
-   Lifecycle management
-   Backup & Recovery
-   Docker integration
-   GPU & AI tooling
-   Linux application management
-   Plugin architecture
-   AI-assisted diagnostics
-   Enterprise capabilities

Contributions should support this long-term direction without
compromising current architecture.

------------------------------------------------------------------------

# Development Philosophy

WSL Studio values:

-   Safety
-   Simplicity
-   Transparency
-   Maintainability
-   Performance
-   Accessibility
-   Professional documentation
-   Incremental, reviewable development

Every feature should leave the project in a better state than it was
found.
>>>>>>> origin/main
