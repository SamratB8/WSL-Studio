# WSL Studio

![Status](https://img.shields.io/badge/status-planning-blue)
![Platform](https://img.shields.io/badge/platform-Windows%2011-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![Phase](https://img.shields.io/badge/phase-0-lightgrey)

WSL Studio is a planned Windows desktop application for managing Windows Subsystem for Linux (WSL) distributions from a native Windows 11 interface.

The project is currently in Phase 0. No application code has been scaffolded yet. This repository contains the initial project blueprint, architecture notes, command mapping, security model, roadmap, and contribution guidance that will guide future implementation.

Inspired by the usability of Docker Desktop while remaining focused entirely on Windows Subsystem for Linux and its official feature set.

> **Note**
>
> WSL Studio aims to support officially documented WSL features. Feature availability may vary depending on the installed WSL version and Windows release.

## Vision

WSL is an important part of many Windows development workflows, but day-to-day management is still centered around command-line usage. WSL Studio aims to provide a focused desktop experience for common WSL management tasks while preserving the behavior, safety, and expectations of official WSL tooling.

The long-term goal is not to replace the WSL command line. The goal is to expose official WSL functionality through a clear, modern, native Windows interface that helps users inspect, manage, and operate their distributions with confidence.

## Why WSL Studio?

WSL has evolved into an essential part of Windows development, but most management tasks still rely on remembering command-line syntax or manually editing configuration files.

WSL Studio aims to simplify those workflows by providing a modern desktop interface that complements—not replaces—the official WSL tools.

## Planned Feature Overview

The initial product direction includes:

- Viewing installed WSL distributions and their state.
- Starting, stopping, terminating, and launching distributions.
- Installing, importing, exporting, unregistering, and setting default distributions.
- Viewing WSL version and distribution configuration where official tooling allows it.
- Managing common WSL settings through validated configuration workflows.
- Providing safe flows for backup, export, and destructive operations.
- Showing command output, operation status, and failure details in a readable way.
- Offering a modular architecture suitable for future extension.

Feature implementation will be phased. Functionality will be added only when it can be mapped to supported WSL behavior and implemented with appropriate validation and user confirmation.

## Planned Technology Stack

WSL Studio is planned to use:

- C#
- .NET 8 or later
- WinUI 3
- Windows App SDK
- SQLite
- xUnit
- GitHub Actions in a future phase
- MSIX packaging in a future phase

The exact project structure and package versions will be decided during Phase 1 when application scaffolding begins.

## Project Scope

WSL Studio is intended to manage WSL using officially supported Windows interfaces.

The project does not aim to:

- Replace `wsl.exe`
- Modify undocumented WSL internals
- Introduce proprietary WSL functionality
- Run unnecessary background services
- Become another Linux terminal

Instead, it focuses on making existing WSL capabilities easier to discover, configure, and use.

## Project Philosophy

WSL Studio is designed around a few durable principles:

- Use official WSL functionality whenever possible.
- Do not modify WSL internals directly.
- Prefer safety and clarity over convenience.
- Require explicit confirmation before destructive actions.
- Validate configuration before applying changes.
- Start quickly and avoid unnecessary background services.
- Follow modern Windows 11 Fluent Design principles.
- Keep the codebase modular, testable, and maintainable.
- Build in the open with clear documentation and reviewable changes.

## Current Status

🚧 **Planning (Phase 0)**

The project is currently focused on architecture, documentation, and repository organization.

Application development will begin after the project structure and design decisions have been finalized.

## Roadmap Summary

The planned development path is:

- Phase 0: Repository blueprint and documentation.
- Phase 1: Solution scaffolding, project boundaries, and baseline application shell.
- Phase 2: Read-only WSL discovery and distribution overview.
- Phase 3: Safe distribution lifecycle actions.
- Phase 4: Import, export, backup, and destructive-action safeguards.
- Phase 5: Settings, validation, persistence, and diagnostics.
- Phase 6: Packaging, automated checks, release preparation, and initial public release.

See [docs/roadmap.md](docs/roadmap.md) for the detailed roadmap.

Possible Future Features include:

- Live resource monitoring
- Docker integration
- VS Code integration
- Plugin support
- Backup scheduling
- Diagnostic report generation
- Performance benchmarking

## Architecture

```text
WinUI 3
    │
Application
    │
Core
    │
Infrastructure
    │
├── WSL (wsl.exe)
├── PowerShell
├── Docker
└── SQLite
```

## Documentation

- [Project Blueprint](docs/blueprint.md)
- [Architecture](docs/architecture.md)
- [Roadmap](docs/roadmap.md)
- [Command Map](docs/command-map.md)
- [Security Model](docs/security-model.md)
- [Architecture Decision Records](docs/adr/)
- [Contributing](CONTRIBUTING.md)

## Contributing

WSL Studio is in its planning stage, and the architecture is still evolving.

Thoughtful discussions, architecture reviews, documentation improvements, and WSL expertise are especially valuable at this stage.

If you're interested in helping shape the project, feel free to open an issue or start a discussion.

Before contributing, please read [CONTRIBUTING.md](CONTRIBUTING.md).

## License

This project is licensed under the MIT License.
