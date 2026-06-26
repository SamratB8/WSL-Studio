# ADR-002: Use Official WSL Commands

## Status

Accepted

## Context

WSL Studio manages Windows Subsystem for Linux distributions. WSL state and behavior are owned by Windows and the WSL platform. Directly modifying WSL internals would increase the risk of data loss, breakage across Windows releases, and unsupported behavior.

The project needs a clear boundary between WSL Studio and WSL itself. Users should be able to trust that WSL Studio performs management actions through supported mechanisms wherever possible.

## Decision

WSL Studio will use official WSL commands and documented behavior wherever possible. The application must not directly modify WSL internals, registry state, distribution metadata, virtual disks, or undocumented WSL storage locations as a substitute for supported WSL functionality.

WSL command execution will be centralized behind an infrastructure boundary. Commands should be constructed from validated arguments and executed without shell interpolation.

## Alternatives Considered

- Directly modify WSL registry entries or internal storage.
- Inspect or edit distribution virtual disks directly.
- Use undocumented WSL implementation details for richer functionality.
- Build custom wrappers around Linux-side files without explicit user action.

## Consequences

Using official WSL commands reduces the risk of corrupting user environments and makes the project more maintainable as WSL evolves. It also gives contributors a clear standard for deciding whether a feature belongs in the application.

The tradeoff is that some desired features may be limited by what WSL officially exposes. WSL Studio may need to defer or reject features that require unsupported internals, even if they appear technically possible. Command output may also vary by WSL version, so parsing must be tested carefully and failures must be handled transparently.
