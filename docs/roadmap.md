# Roadmap

This roadmap describes the development phases for WSL Studio, from initial planning through advanced integrations. It is intentionally conservative and may change as the project validates technical assumptions.

Phases are delivered in order. Quality, safety, and maintainability take priority over feature count. The application remains **read-only** with respect to WSL state until Phase 4 introduces explicitly designed, safeguarded operations.

## Phase Summary

| Phase | Title | WSL state |
| --- | --- | --- |
| Phase 0 | Planning | Read-only |
| Phase 1 | Foundation | Read-only |
| Phase 2 | Core WSL Integration | Read-only |
| Phase 3 | WSL Workspace | Read-only |
| Phase 4 | Safe WSL Operations | Mutating (safeguarded) |
| Phase 5 | Backup & Recovery | Mutating (safeguarded) |
| Phase 6 | Configuration | Mutating (safeguarded) |
| Phase 7 | Advanced Integrations | Varies |
| Phase 8 | Linux GUI & Remote Desktop | Varies |

## Phase 0 — Planning

Status: Complete.

Goals:

- Establish the project vision and principles.
- Document the planned architecture and security model.
- Map planned features to official WSL commands.
- Create contribution guidance.
- Prepare the repository for foundation work.

Exit criteria:

- Documentation is internally consistent.
- The repository clearly communicates project intent.
- Implementation boundaries are understandable.

## Phase 1 — Foundation

Status: Complete.

Goals:

- Create the .NET solution and layered project structure (Core, Application, Infrastructure, App).
- Add a minimal WinUI 3 application shell with Fluent navigation.
- Establish dependency injection through the .NET Generic Host.
- Adopt CommunityToolkit.Mvvm for view models.
- Add an initial xUnit test project.

Expected output:

- A buildable application shell aligned with the architecture document.
- Initial tests for domain primitives and command construction.

## Phase 2 — Core WSL Integration

Status: Complete.

Goals:

- Implement the WSL command execution infrastructure (`ICommandRunner` over `wsl.exe`).
- Discover installed distributions with name, running state, version, and default flag.
- Surface WSL status and version information where available.
- Add refresh behavior and clear error reporting for missing WSL or unavailable commands.
- Cover parsers with representative-output tests.

Expected output:

- A read-only distribution overview and dashboard.
- No destructive actions.
- Graceful handling of missing WSL, unavailable commands, and parsing failures.

## Phase 3 — WSL Workspace

Status: In progress.

Phase 3 deepens read-only inspection, diagnostics, and developer-workspace polish before any state-changing operations are introduced. **This phase must not include destructive or state-changing WSL operations.** It uses only official, read-only WSL commands.

Goals:

- **WSL Health Center** — read-only environment and configuration health checks.
- **Distribution Details** — per-distribution inspection of state, version, kernel, and available metadata.
- **Search and filtering** — quickly locate distributions in the workspace.
- **Environment details** — visibility into WSL version, kernel, WSLg, and Windows version.
- **Diagnostics viewer** — present diagnostic information gathered from official commands.
- **Diagnostic report export** — export a read-only diagnostic summary for issue reporting.
- **Terminal launch integration** — only where it is read-only/safe and user-initiated (open a terminal; do not manage state).
- **Performance visibility** — surface available metrics without introducing a background monitoring service.
- General workspace and Fluent design polish.

Expected output:

- A polished, read-only WSL workspace for inspection and diagnostics.
- All workflows routed through application services and tested parsers.
- No mutation of WSL state.

## Phase 4 — Safe WSL Operations

Status: Planned.

Phase 4 introduces the first state-changing operations, each routed through application services and safety policies.

Goals:

- Add supported lifecycle actions such as launch, terminate, and shutdown.
- Show progress and operation results.
- Require confirmation where an action may interrupt running work.
- Improve diagnostics for WSL command failures.

Expected output:

- Users can manage basic distribution lifecycle actions.
- Operations are routed through safety classifications before execution.
- Tests cover command construction and workflow behavior.

## Phase 5 — Backup & Recovery

Status: Planned.

Goals:

- Add export workflows for distribution backups.
- Add import workflows with path and name validation.
- Add unregister workflows with explicit destructive-action confirmation.
- Store non-sensitive operation history where useful.
- Document recovery expectations and limitations.

Expected output:

- Backup-oriented workflows are available before the most destructive actions.
- Destructive operations require deliberate confirmation.
- The application never implies that unregister can be undone without a prior export.

## Phase 6 — Configuration

Status: Planned.

Goals:

- Add application settings backed by SQLite.
- Add supported WSL configuration workflows (`.wslconfig`, `wsl.conf`) where safe and documented.
- Validate configuration changes before applying them.
- Distinguish Windows-level WSL configuration from per-distribution Linux configuration.
- Add local diagnostic logs with privacy-conscious defaults.

Expected output:

- Settings are explicit, validated, and reversible where practical.
- Configuration behavior remains aligned with official WSL functionality.

## Phase 7 — Advanced Integrations

Status: Planned.

Goals:

- Optional integrations with adjacent developer tooling (for example, Docker and editor integrations) using official, supported interfaces.
- Windows Terminal integration.
- Enhanced diagnostics and troubleshooting workflows.
- Packaging (MSIX) and automated checks in GitHub Actions ahead of broader distribution.

Expected output:

- Integrations that complement WSL without modifying its internals.
- Repeatable build, test, and packaging checks.

## Phase 8 — Linux GUI & Remote Desktop

Status: Planned.

Goals:

- Surface WSLg / Linux GUI application visibility where supported.
- Explore remote desktop and session management workflows aligned with official capabilities.

Expected output:

- Advanced workspace capabilities that remain within supported WSL functionality.

## Notes

- Phase boundaries may shift as the project validates WSL behavior across Windows and WSL versions.
- Any change that moves the application from read-only to state-changing behavior requires a corresponding security-model review.
