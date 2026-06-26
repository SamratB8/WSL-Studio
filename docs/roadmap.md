# Roadmap

This roadmap describes planned phases from repository preparation through an initial public release. It is intentionally conservative and may change as the project validates technical assumptions.

## Phase 0: Repository Blueprint and Documentation

Status: In progress.

Goals:

- Establish the project vision and principles.
- Document the planned architecture.
- Map planned features to official WSL commands.
- Define the security and safety model.
- Create contribution guidance.
- Prepare the repository for Phase 1 scaffolding.

Out of scope:

- Application code.
- WinUI project scaffolding.
- Continuous integration.
- Packaging.
- Screenshots or release badges.

Exit criteria:

- Documentation is internally consistent.
- The repository clearly communicates project intent.
- Phase 1 implementation boundaries are understandable.

## Phase 1: Solution Scaffolding

Goals:

- Create the .NET solution and initial project structure.
- Add a minimal WinUI 3 application shell.
- Establish dependency injection and project references.
- Add initial xUnit test projects.
- Define coding conventions in the repository.
- Add basic local build instructions.

Expected output:

- A buildable application shell.
- Empty or minimal project layers aligned with the architecture document.
- Initial tests for domain primitives or command construction.

## Phase 2: Read-Only WSL Discovery

Goals:

- Implement WSL command execution infrastructure.
- Display installed distributions.
- Show default distribution, running state, and WSL version where available.
- Add refresh behavior and basic error reporting.
- Add parser tests using representative command output.

Expected output:

- A read-only distribution overview.
- No destructive actions.
- Clear handling for missing WSL, unavailable commands, or parsing failures.

## Phase 3: Safe Lifecycle Operations

Goals:

- Add supported lifecycle actions such as launch, terminate, and shutdown.
- Show progress and operation results.
- Add confirmation where an action may interrupt running work.
- Improve diagnostics for WSL command failures.

Expected output:

- Users can manage basic distribution lifecycle actions.
- Operations are routed through application services and safety policies.
- Tests cover command construction and workflow behavior.

## Phase 4: Import, Export, and Destructive Safeguards

Goals:

- Add export workflows for distribution backups.
- Add import workflows with path and name validation.
- Add unregister workflows with explicit destructive-action confirmation.
- Store non-sensitive operation history where useful.
- Document recovery expectations and limitations.

Expected output:

- Backup-oriented workflows are available before the most destructive actions.
- Destructive operations require deliberate confirmation.
- The application avoids implying that unregister can be undone without a prior export.

## Phase 5: Settings, Configuration, and Diagnostics

Goals:

- Add application settings backed by SQLite.
- Add supported WSL configuration workflows where safe and documented.
- Validate configuration changes before applying them.
- Add local diagnostic logs with privacy-conscious defaults.
- Improve user-facing error explanations.

Expected output:

- Settings are explicit, validated, and reversible where practical.
- Diagnostics help users report issues without exposing unnecessary data.
- Configuration behavior remains aligned with official WSL functionality.

## Phase 6: Packaging and Initial Public Release

Goals:

- Add automated checks in GitHub Actions.
- Add MSIX packaging.
- Prepare release notes and installation guidance.
- Review accessibility, usability, and error states.
- Complete a security review of destructive workflows.
- Publish an initial public release.

Expected output:

- A signed or clearly documented development package strategy.
- Repeatable build and test checks.
- Initial release artifacts and documentation.

## Post-Initial Release Candidates

Future work may include:

- Better diagnostics and troubleshooting workflows.
- Optional terminal launch integration.
- Export profiles.
- Enhanced distribution metadata.
- More advanced configuration editing.
- Community-requested features that align with WSL Studio's safety model.

These items are not commitments for the initial release.
