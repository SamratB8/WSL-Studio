# ADR-006: Safe Destructive Actions

## Status

Accepted

## Context

Some WSL management operations can interrupt active work or remove distribution data. For example, terminating a distribution can stop running processes, and unregistering a distribution can remove data managed by WSL.

WSL Studio should make these operations possible only when the user clearly understands the scope and consequence. Safety is part of the product design, not only an implementation detail.

## Decision

WSL Studio will classify destructive and interruptive operations and route them through explicit safety flows.

Destructive actions must require clear confirmation, identify the target resource, explain the consequence, and recommend or provide a backup/export path where practical. The application should refresh relevant WSL state before executing sensitive operations and should display the final result, including useful WSL error output.

## Alternatives Considered

- Rely on a standard confirmation dialog for all actions.
- Allow destructive actions immediately from context menus.
- Hide destructive actions from the MVP entirely.
- Depend on users knowing the behavior of the equivalent command-line operation.

## Consequences

This decision reduces the risk of accidental data loss and makes the application behavior more predictable. It also gives contributors a clear review standard for features that affect user data or running workloads.

The tradeoff is additional UI and workflow complexity. Some operations will take more steps than the equivalent command-line command. That is acceptable for WSL Studio because the application prioritizes safety and clarity over speed for destructive operations.
