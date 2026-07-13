# WSL Studio

![Status](https://img.shields.io/badge/status-active%20development-success)
![Platform](https://img.shields.io/badge/platform-Windows%2011-blue)
![Framework](https://img.shields.io/badge/.NET-10-purple)
![UI](https://img.shields.io/badge/UI-WinUI%203-blueviolet)
![License](https://img.shields.io/badge/license-Apache%202.0-green)
![Development](https://img.shields.io/badge/development-Phase%203%20WSL%20Workspace-orange)

WSL Studio is an open-source native Windows 11 application for managing Windows Subsystem for Linux (WSL) through a modern WinUI 3 interface.

The project focuses on providing a safe, discoverable, and Microsoft-style desktop experience built on top of officially supported WSL functionality. Rather than replacing the command line, WSL Studio complements it by exposing common management, diagnostics, inspection, and configuration workflows through a polished graphical interface.

WSL Studio is currently focused on **read-only WSL workspace features**. Mutating operations such as starting, stopping, shutting down, importing, exporting, unregistering, and editing WSL configuration are intentionally deferred until the safety and confirmation layers are ready.

> **Note**
>
> WSL Studio aims to support officially documented WSL features. Feature availability may vary depending on the installed WSL version and Windows release.

---

## Project Status

| Component | Status |
|-----------|--------|
| Documentation | ✅ Complete |
| Architecture | ✅ Complete |
| WinUI 3 Shell | ✅ Complete |
| Clean Architecture Solution | ✅ Complete |
| WSL Command Runner | ✅ Complete |
| Read-only WSL Discovery | ✅ Complete |
| Dashboard | ✅ Complete |
| Distribution List | ✅ Complete |
| Distribution Details | ✅ Complete |
| Distribution Search / Filter / Sort | ✅ Complete |
| WSL Health Center | ✅ Complete |
| Environment Overview | ✅ Complete |
| Responsive Card Layouts | ✅ Complete |
| Design System | ✅ Complete |
| Diagnostic Report Export | ✅ Complete |
| Terminal Launch Helpers | ⏳ Planned |
| Safe Lifecycle Operations | ⏳ Planned |
| Backup & Recovery | ⏳ Planned |
| Configuration Center | ⏳ Planned |

---

## Vision

WSL is an important part of many Windows development workflows, but day-to-day management is still centered around command-line usage and scattered configuration files.

WSL Studio aims to provide a focused desktop experience for WSL management while preserving the behavior, safety, and expectations of official WSL tooling.

The long-term goal is not to replace `wsl.exe`. The goal is to expose official WSL functionality through a clear, modern, native Windows interface that helps users inspect, manage, configure, troubleshoot, and operate their WSL environments with confidence.

---

## Why WSL Studio?

WSL has evolved into an essential part of Windows development, but many workflows still require remembering command-line syntax, manually inspecting configuration files, or jumping between separate tools.

WSL Studio is being built to make those workflows easier to discover and safer to perform.

It is inspired by the usability of tools like Docker Desktop and Windows Settings, while remaining focused on the WSL ecosystem and official Microsoft-supported behavior.

---

## Current Development Focus

🚧 **Active Development — Phase 3: WSL Workspace**

Phase 3 is focused on building a polished, read-only WSL workspace before introducing any mutating operations.

Implemented so far:

- Native WinUI 3 application shell
- Clean Architecture solution structure
- Generic Host dependency injection
- CommunityToolkit.Mvvm integration
- Official WSL command runner abstraction
- Read-only distribution discovery
- Dashboard overview
- Distribution details page
- Search, filtering, and sorting for distributions
- WSL Health Center
- WSL Environment overview
- Responsive Fluent card layouts
- Reusable WinUI design system
- Unit tests for parsing, discovery, dashboard, health, environment, and query behavior
- Architecture Decision Records (ADRs)

Current Phase 3 work remains strictly read-only.

---

## Features

### Implemented

- **Native Windows 11 app shell**
  - WinUI 3
  - Windows App SDK
  - Fluent-style navigation
  - Responsive card layouts

- **Dashboard**
  - Installed distribution count
  - Running distribution count
  - Stopped distribution count
  - Default distribution
  - WSL version overview

- **Distribution workspace**
  - List installed WSL distributions
  - Display state, WSL version, and default status
  - Search distributions by name
  - Filter by state
  - Sort by default, name, state, and WSL version
  - Open read-only distribution details

- **Distribution details**
  - State
  - WSL version
  - Default status
  - Kernel information when available
  - Clear messaging for values not reported by WSL

- **WSL Health Center**
  - Read-only health checks
  - WSL availability
  - Version availability
  - Kernel availability
  - Distribution checks
  - Docker Desktop WSL integration detection
  - WSLg availability
  - Not-checked state for Windows-side checks not implemented yet

- **Environment overview**
  - WSL version
  - Default WSL version
  - Kernel version
  - WSLg version
  - Windows version
  - Direct3D version
  - DXCore version
  - MSRDC version
  - Installed and running distribution counts
  - Default distribution
  - Docker Desktop detection
  - WSLg availability

- **Architecture and infrastructure**
  - Official `wsl.exe` command execution boundary
  - No shell interpolation
  - Structured command request/result models
  - Timeout and cancellation support
  - Application-layer parsing
  - Core domain models
  - Unit-tested parsing and service behavior

---

### Planned

- Diagnostic report export
  - Markdown
  - JSON
  - TXT

- Terminal launch helpers
  - Open Windows Terminal
  - Open selected distribution shell
  - No lifecycle changes

- Safe lifecycle operations
  - Start distribution
  - Terminate distribution
  - Shutdown WSL
  - Confirmation-first design

- Backup and recovery
  - Export workflows
  - Import workflows
  - Restore workflows
  - Destructive-action safeguards

- Configuration management
  - `.wslconfig` GUI
  - `wsl.conf` GUI
  - Memory and processor settings
  - File system settings
  - Networking settings
  - Optional WSL features
  - Developer settings

- Advanced integrations
  - Docker
  - VS Code
  - Visual Studio
  - GPU / AI tooling
  - Linux GUI app management
  - Remote desktop workflows

---

## Technology Stack

WSL Studio currently uses:

- C#
- .NET 10
- WinUI 3
- Windows App SDK
- CommunityToolkit.Mvvm
- Microsoft.Extensions.Hosting
- Microsoft dependency injection
- xUnit

Planned later:

- SQLite for local app metadata
- GitHub Actions
- MSIX packaging
- Plugin architecture
- Optional advanced integrations

---

## Project Scope

WSL Studio is intended to manage WSL using officially supported Windows and WSL interfaces.

The project does **not** aim to:

- Replace `wsl.exe`
- Modify undocumented WSL internals
- Edit WSL-managed registry or virtual disk internals directly
- Introduce proprietary WSL behavior
- Run unnecessary background services in the MVP
- Become another Linux terminal

Instead, it focuses on making existing WSL capabilities easier to discover, inspect, configure, and use safely.

---

## Safety Model

WSL Studio follows a safety-first development model.

Current behavior:

- Read-only commands only
- No WSL state modification
- No lifecycle actions
- No import/export/unregister operations
- No repair buttons
- No automatic configuration changes
- No background monitoring

Future mutating operations will require:

- Clear user intent
- Explicit confirmation
- Strong validation
- User-safe error messages
- Audit-friendly command boundaries
- No hidden destructive behavior

---

## Feature Stability Model

As WSL Studio grows, features will be grouped by stability:

| Stability | Meaning |
|----------|---------|
| Stable | Safe for normal users and expected to remain supported |
| Preview | Functional but still being refined |
| Experimental | Advanced or power-user features that may change |

The default experience will remain safe and approachable. Advanced and experimental features will be opt-in.

---

## Roadmap

### Core Roadmap

- **Phase 0 — Planning**
  - Repository setup
  - Documentation
  - Architecture decisions

- **Phase 1 — Foundation**
  - WinUI 3 shell
  - Clean Architecture
  - Dependency injection
  - MVVM foundation
  - Design system

- **Phase 2 — Core WSL Integration**
  - WSL command runner
  - Read-only distribution discovery
  - Dashboard overview

- **Phase 3 — WSL Workspace**
  - Health Center
  - Distribution details
  - Search, filtering, sorting
  - Environment overview
  - Diagnostic report export
  - Terminal launch helpers
  - Accessibility and UI polish

- **Phase 4 — Safe WSL Operations**
  - Lifecycle operations
  - Start, terminate, shutdown
  - Confirmation workflows
  - Safe operation results

- **Phase 5 — Backup & Recovery**
  - Import
  - Export
  - Restore
  - Backup workflows
  - Destructive-action safeguards

- **Phase 6 — Configuration**
  - WSL settings GUI
  - `.wslconfig`
  - `wsl.conf`
  - Validation
  - Local settings persistence

- **Phase 7 — Advanced Integrations**
  - Docker integration
  - VS Code integration
  - Visual Studio integration
  - Developer workflow helpers

- **Phase 8 — Linux GUI & Remote Desktop**
  - WSLg inspection
  - Linux GUI app workflows
  - Remote desktop guidance
  - GUI tooling management

See [docs/roadmap.md](docs/roadmap.md) for the detailed roadmap.

---

## Long-Term Platform Roadmap

After the core roadmap is complete, WSL Studio may expand into a broader Linux-on-Windows platform.

Possible future areas:

- Advanced WSL Configuration Center
- Distribution cloning, moving, compression, and repair
- Snapshot and restore workflows
- GPU acceleration setup
- CUDA / DirectML / ROCm readiness checks
- Linux app installation GUI
- Package manager integrations
- WSLg application management
- Docker and Podman management
- Kubernetes and Dev Container tooling
- AI-assisted diagnostics
- Plugin marketplace
- Enterprise and multi-machine management

The long-term goal is for WSL Studio to become a complete, extensible WSL management environment while preserving a safe default user experience.

---

## Architecture

```text
WinUI 3 / Presentation
        │
        ▼
Application Use Cases
        │
        ▼
Core Domain Models
        │
        ▼
Infrastructure
        │
┌─────────────┬─────────────┬──────────────┐
│   wsl.exe   │ PowerShell  │   SQLite     │
└─────────────┴─────────────┴──────────────┘
```

## Current principles

- UI never calls wsl.exe directly.
- ViewModels call Application services.
- Application defines use cases, parsing, and workflow intent.
- Infrastructure performs process execution.
- Core remains independent of UI and process execution.
- WSL commands are executed through a controlled command runner.
- User input is not converted into shell-concatenated command strings.
- Read-only discovery is implemented before mutating operations.

## Screenshots

> Screenshots will be updated as the interface stabilizes.

## Documentation

- [Project Blueprint](docs/blueprint.md)
- [Architecture](docs/architecture.md)
- [Architecture Decision Records](docs/adr/)
- [Roadmap](docs/roadmap.md)
- [Security Model](docs/security-model.md)
- [Command Map](docs/command-map.md)
- [Contributing Guide](CONTRIBUTING.md)

## Development

Common development commands:

```powershell
dotnet build WslStudio.slnx
dotnet test WslStudio.slnx --no-build
dotnet run --project src/WslStudio.App
```

Recommended workflow:

```text
Issue
  ↓
Design / ADR if needed
  ↓
Implementation on dev
  ↓
Build and tests
  ↓
Commit
  ↓
Push to dev
  ↓
Pull request to main at milestone checkpoints
```

## Contributing

WSL Studio is under active development, and contributions are welcome.

Helpful contribution areas include:

- WinUI design polish
- WSL expertise
- Documentation improvements
- Parser tests
- Accessibility reviews
- Diagnostics design
- Safe workflow design
- Windows developer tooling experience

Please review [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request.

## Community and Support

For quick questions, community discussion, testing feedback, and contributor coordination, join the Nova Systems Lab Discord server:

[Join Nova Systems Lab on Discord](https://discord.gg/sfFyVyTfX8)

For official project work:

- Use GitHub Discussions for detailed proposals and long-term discussions.
- Use GitHub Issues for confirmed bugs and actionable tasks.
- Use Pull Requests for code and documentation changes.

## Development Philosophy

WSL Studio follows a quality-first development approach.

Every feature is designed around:

- Official Microsoft APIs and tooling
- Safe defaults
- Clear architecture boundaries
- Comprehensive testing
- Modern Windows 11 design principles
- Incremental, reviewable development
- Long-term maintainability
- Transparent project documentation

The project prioritizes correctness, usability, and transparency over rapid feature growth.

---

## Part of Nova Systems Lab

<p align="center">
  <img
    src="https://raw.githubusercontent.com/Nova-Systems-Lab/.github/main/profile/assets/nova-systems-lab-horizontal.png"
    alt="Nova Systems Lab"
    width="420"
  >
</p>

WSL Studio is developed under **Nova Systems Lab**, an independent open-source organization focused on systems software, developer tools, platform integration, and experimental runtime technologies.

---

## License

This project is licensed under the MIT License.
