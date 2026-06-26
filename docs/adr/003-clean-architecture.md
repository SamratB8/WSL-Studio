# ADR-003: Clean Architecture

## Status

Accepted

## Context

WSL Studio is intended to be a long-term open-source project. It will include a Windows desktop UI, WSL command integration, validation rules, safety policies, persistence, and tests. Without clear boundaries, UI code could become tightly coupled to process execution, command parsing, and configuration logic.

The project needs an architecture that supports reviewable changes, automated testing, and future growth without requiring broad rewrites.

## Decision

WSL Studio will use a clean, layered architecture.

The planned layers are:

- Presentation for WinUI views, controls, and view models.
- Application for user workflows and orchestration.
- Domain for core models, validation rules, operation results, and safety policies.
- Infrastructure for WSL command execution, parsing, SQLite persistence, logging, and Windows-specific integrations.

Dependencies should point inward toward domain and application logic. UI code should not construct WSL command lines directly, and infrastructure code should be hidden behind interfaces where practical.

## Alternatives Considered

- A single-project application with all logic in the UI.
- A service-oriented design with a local background daemon.
- A plugin-first architecture from the beginning.
- A strict domain-driven design structure with more abstraction than the project currently needs.

## Consequences

This architecture keeps risky behavior, such as command execution and destructive actions, easier to locate and review. It also makes core behavior testable without starting the UI or requiring WSL for every test.

The tradeoff is additional project structure and discipline. Small features may require touching more than one layer. The project should keep abstractions practical and avoid adding patterns before they solve a real maintenance problem.
