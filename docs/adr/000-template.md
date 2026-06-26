# ADR-000: Template

## Status

Accepted

## Context

Architecture Decision Records document important technical and product decisions for WSL Studio. They should explain why a decision was made, what alternatives were considered, and what consequences the project accepts as a result.

The project is expected to evolve over time. ADRs provide a durable record for contributors who need to understand the reasoning behind the repository structure, technology choices, safety policies, and implementation boundaries.

## Decision

WSL Studio will use this ADR format for major architecture and product decisions:

- Title with a stable ADR number.
- Status.
- Context.
- Decision.
- Alternatives considered.
- Consequences.

New ADRs should be added as separate Markdown files in this directory. Existing ADRs should not be rewritten to change history. If a decision changes, a later ADR should supersede the earlier one and explain why.

## Alternatives Considered

- Keep decisions only in issue discussions or pull requests.
- Maintain a single design-decisions document.
- Defer decision records until implementation begins.

## Consequences

This format gives the project a clear and lightweight way to record decisions. It helps new contributors understand why choices were made without searching through old discussions.

The tradeoff is that ADRs require maintenance. Contributors should avoid creating ADRs for minor implementation details, but decisions that affect architecture, safety, supported WSL behavior, or long-term maintenance should be recorded.
