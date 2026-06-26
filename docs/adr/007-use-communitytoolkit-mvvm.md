# ADR-007: Use CommunityToolkit.Mvvm

## Status

Accepted

## Context

WSL Studio is a WinUI 3 desktop application that needs a maintainable separation between UI, state, commands, and application logic. The application will include screens for distribution management, settings, operation progress, validation messages, and confirmation flows.

Keeping this behavior in code-behind would make screens harder to test and easier to couple directly to WSL command execution. The project needs a presentation pattern that supports clear ownership boundaries while remaining approachable for contributors.

MVVM is appropriate for WSL Studio because it keeps views focused on rendering and interaction, while view models hold presentation state and expose user actions as commands. This fits the planned Clean Architecture approach: views depend on view models, view models call application services, and application services coordinate domain and infrastructure behavior.

## Decision

WSL Studio will use CommunityToolkit.Mvvm as the MVVM library for the application layer and view model layer.

CommunityToolkit.Mvvm is preferred because it is lightweight, actively maintained, and well suited to modern .NET desktop applications. It provides source-generator support for observable properties and relay commands, reducing boilerplate while keeping the generated behavior understandable.

View models should use CommunityToolkit.Mvvm features such as observable properties and relay commands where they improve clarity. View models should receive application services through dependency injection rather than constructing infrastructure dependencies directly. This keeps presentation logic testable and preserves the separation between UI behavior, application workflows, and WSL command execution.

## Alternatives Considered

- Plain code-behind.
- Custom MVVM implementation.
- Prism.
- ReactiveUI.
- Caliburn.Micro.

## Consequences

Using CommunityToolkit.Mvvm improves separation between UI and logic, lowers repetitive property and command boilerplate, and makes presentation behavior easier to unit test. It also fits the planned dependency injection and Clean Architecture boundaries without requiring a large framework.

The tradeoff is that the project adds one external dependency and contributors need basic MVVM familiarity. Heavier frameworks such as Prism, ReactiveUI, and Caliburn.Micro may be useful for some applications, but they add patterns and runtime expectations that are not necessary for the initial WSL Studio project. A custom MVVM implementation would avoid a dependency, but it would add maintenance work without a clear benefit at this stage.
