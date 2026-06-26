# ADR-008: Use the .NET Generic Host

## Status

Accepted

## Context

WSL Studio will eventually contain numerous application services. These services are expected to include WSL command execution, configuration management, settings, logging, diagnostics, backup management, plugin loading in a future phase, and update checking in a future phase.

The application needs dependency injection, centralized service registration, logging, configuration, consistent application startup, easy testing, and a modular architecture. These needs apply even though WSL Studio is a desktop application rather than a web application.

The .NET Generic Host originated in the ASP.NET Core ecosystem, but it is equally applicable to modern desktop applications. It provides a standard composition model for registering services, configuring logging, loading configuration, and managing service lifetimes.

## Decision

WSL Studio will use the Microsoft .NET Generic Host, provided by `Microsoft.Extensions.Hosting`, as the application's composition root.

The Generic Host will be responsible for:

- Dependency injection.
- Configuration.
- Logging.
- Service lifetime management.
- Startup initialization.
- Graceful application shutdown.

The WinUI layer should remain responsible only for presentation and user interaction. It should receive view models and application services through dependency injection rather than constructing infrastructure dependencies directly.

## Alternatives Considered

- Manual object construction. This is simple for very small applications, but it becomes difficult to maintain as the number of services grows. It also makes dependency replacement and test setup more repetitive.
- Static singleton services. Static services can reduce constructor wiring, but they create hidden dependencies, make tests harder to isolate, and increase the risk of shared mutable state.
- Service Locator pattern. A service locator centralizes lookup but hides dependencies from constructors. This makes classes harder to reason about and weakens compile-time visibility into required collaborators.
- Third-party dependency injection containers. Mature containers can provide advanced features, but WSL Studio does not currently need those features. The Microsoft-provided hosting and dependency injection stack is sufficient for the initial architecture and keeps the dependency model conventional.

## Consequences

Using the .NET Generic Host gives WSL Studio a consistent Microsoft architecture for application composition. It provides cleaner dependency management, easier unit testing, better scalability as services are added, easier future plugin support, easier service replacement, and better maintainability.

The tradeoff is slightly more startup code than manual construction. Contributors should understand basic dependency injection concepts, service lifetimes, and how application services are registered. The project will also depend on additional `Microsoft.Extensions` packages, including hosting, configuration, logging, and dependency injection packages as needed.
