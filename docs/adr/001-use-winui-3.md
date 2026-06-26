# ADR-001: Use WinUI 3

## Status

Accepted

## Context

WSL Studio is intended to be a native Windows 11 desktop application. The user interface should feel consistent with modern Windows applications, support Fluent Design patterns, and integrate cleanly with the planned C# and .NET application stack.

The application is expected to provide distribution management, settings, dialogs, progress states, and safety confirmations. These workflows benefit from a desktop UI framework that is actively aligned with Windows 11 design and platform direction.

## Decision

WSL Studio will use WinUI 3 with the Windows App SDK for the desktop application.

The initial implementation will target a native Windows application experience rather than a web-based shell. The UI layer will remain separate from WSL command execution and application logic.

## Alternatives Considered

- WPF.
- Windows Forms.
- Electron.
- .NET MAUI.
- A command-line application only.

WPF was considered because it is mature and stable, but it does not align as closely with the modern Windows App SDK direction.

Electron was considered because it is popular for developer tools, but it would add unnecessary runtime weight for a Windows-only native application.

.NET MAUI was considered, but cross-platform support is not a project goal for the desktop app.

## Consequences

WinUI 3 supports the goal of a modern Windows 11 interface and fits the planned C# and .NET stack. It also aligns with Fluent UI controls and Windows App SDK capabilities.

The tradeoff is that WinUI 3 has different tooling and platform constraints than older desktop frameworks. Contributors will need Windows development environments that support WinUI 3 and the Windows App SDK. The project should keep UI concerns isolated so core WSL integration and safety logic can be tested without requiring a full UI runtime.
