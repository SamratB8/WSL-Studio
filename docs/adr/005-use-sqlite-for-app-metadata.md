# ADR-005: Use SQLite for App Metadata

## Status

Accepted

## Context

WSL Studio will need to store application-owned data such as user preferences, non-sensitive operation history, and cached display metadata. This data should be local, structured, and easy to migrate as the application evolves.

The application must not treat local persistence as the source of truth for WSL state. WSL state should be discovered through official WSL behavior when accuracy matters.

## Decision

WSL Studio will use SQLite for application metadata.

SQLite may store application preferences, recent non-sensitive operation metadata, UI state, and other data owned by WSL Studio. It must not be used to mirror or modify WSL internals.

## Alternatives Considered

- JSON configuration files only.
- Windows registry storage.
- A larger database server.
- No local persistence beyond in-memory state.

## Consequences

SQLite provides structured local storage without requiring a separate database service. It is widely understood, testable, and appropriate for desktop application metadata.

The tradeoff is that schema migrations and data access patterns must be managed deliberately. The project should keep stored data minimal and privacy-conscious. Sensitive information, Linux filesystem contents, secrets, and undocumented WSL state should not be stored in the application database.
