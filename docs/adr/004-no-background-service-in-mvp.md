# ADR-004: No Background Service in MVP

## Status

Accepted

## Context

WSL Studio is planned as a desktop management application. Many WSL management workflows are user-initiated, such as listing distributions, launching a distribution, exporting a backup, or changing a setting. A background service would add operational complexity, security considerations, installation requirements, and maintenance burden.

For the initial product, there is not yet a confirmed requirement that justifies an always-running service.

## Decision

WSL Studio will not include a background service in the MVP.

The application should perform work in response to user actions while the desktop app is running. It should avoid unnecessary background activity and should start quickly.

## Alternatives Considered

- Install a Windows service for monitoring and scheduled work.
- Run a tray application that remains active after the main window closes.
- Add a background task for polling WSL state.
- Use a service only for privileged operations.

## Consequences

Avoiding a background service keeps the MVP simpler, easier to install, and easier to reason about. It also reduces the amount of code that can act without direct user visibility.

The tradeoff is that features such as scheduled backups, continuous monitoring, notifications, or long-running automation may need to wait until a later phase. If a future feature requires background execution, it should be introduced through a separate ADR with a clear security and maintenance review.
