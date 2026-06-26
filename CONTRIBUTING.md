# Contributing to WSL Studio

Thank you for considering a contribution to WSL Studio. The project is currently in Phase 0, so the most useful contributions are documentation improvements, architecture review, WSL behavior verification, issue discussion, and careful planning for future implementation.

Application code will be added in a later phase.

## Branch Strategy

The repository will use a simple branch strategy during early development:

- `main` is the stable development branch.
- Feature work should happen on short-lived branches.
- Branch names should describe the work, such as `docs/security-model` or `feature/distribution-list`.
- Pull requests should be small enough to review carefully.

Long-lived release branches may be introduced when the project approaches public releases.

## Coding Standards

The planned implementation stack is C#, .NET 8 or later, WinUI 3, Windows App SDK, SQLite, and xUnit.

When application code is introduced, contributors should follow these standards:

- Prefer clear, maintainable C# over clever abstractions.
- Keep UI code separate from WSL command execution.
- Route WSL operations through reviewed application services.
- Use structured process execution rather than shell-interpolated command strings.
- Add tests for parsing, validation, command construction, and safety policies.
- Keep public behavior documented when it affects users.
- Follow established .NET naming and formatting conventions.

Detailed formatting and analyzer rules will be added when the solution is scaffolded.

## Pull Request Expectations

Pull requests should include:

- A clear description of the change.
- The reason the change is needed.
- Any relevant issue links.
- Notes about testing or validation performed.
- Screenshots only when UI exists and the screenshot is real.

Pull requests should avoid:

- Mixing unrelated changes.
- Adding undocumented dependencies.
- Introducing unsupported WSL internals.
- Adding destructive operations without security-model updates.
- Creating CI/CD, packaging, or application scaffolding before the relevant project phase.

## Issue Reporting

Useful issues include:

- Documentation gaps or inconsistencies.
- Confirmed WSL command behavior that affects the command map.
- Safety concerns.
- Usability concerns.
- Feature proposals that align with official WSL functionality.

When reporting a behavior issue in future application phases, include:

- Windows version.
- WSL version information, if available.
- Distribution name and version, if relevant.
- Steps to reproduce.
- Expected behavior.
- Actual behavior.
- Error output, with sensitive information removed.

Please do not include secrets, private keys, access tokens, or sensitive files from a WSL distribution.

## Code Review Philosophy

Code review should protect maintainability and user safety. Reviews should focus on:

- Correctness.
- Clear ownership boundaries.
- Test coverage for meaningful behavior.
- Safe command construction.
- Validation and confirmation for risky operations.
- Readable user-facing errors.
- Alignment with the documented architecture and security model.

Review feedback should be specific, respectful, and grounded in the project goals. Contributors should expect careful review for any feature that changes WSL state, touches user data, or affects destructive operations.

## Documentation Contributions

Documentation should use a professional open-source tone. Avoid marketing language, unsupported claims, and promises about features that have not been designed or implemented.

Good documentation contributions:

- Clarify project scope.
- Correct command references.
- Improve safety explanations.
- Make future implementation work easier to review.
- Keep documents consistent with one another.

## Development Phase Notes

During Phase 0:

- Do not add application code.
- Do not scaffold the WinUI solution.
- Do not add GitHub Actions.
- Do not add packaging.
- Do not add invented screenshots, badges, or release artifacts.

These items will be introduced in later phases when the repository is ready for them.
