# 0002 - Use ASP.NET Core for Backend

## Status

Accepted

## Context

TrainMateX-Pro is a fullstack training application built as a long-term learning and portfolio project.

The backend needs to provide a stable API for the frontend, own the core business logic, handle validation, manage persistence and later support authentication, authorization, testing, logging and deployment.

The project should start simple, but it should not block a future move toward a more structured architecture such as Clean Architecture or a modular monolith.

## Decision

TrainMateX-Pro will use ASP.NET Core as the backend framework.

The backend will start as a simple ASP.NET Core Web API using Minimal APIs. The initial structure will be kept small, but the project layout will allow future expansion into separate projects such as Application, Domain and Infrastructure if the system grows enough to justify it.

The backend will be responsible for:

- API endpoints
- Domain and business rules
- Validation
- Data persistence
- Authentication and authorization rules later
- Logging and error handling
- Automated tests
- Integration with external services later

## Reasoning

ASP.NET Core is a strong fit for TrainMateX-Pro because it provides a mature, performant and production-ready backend platform.

It also aligns with the main learning goal of becoming stronger in C#, .NET, backend development, API design, testing and production-oriented software engineering.

Starting with Minimal APIs keeps the first version simple and reduces unnecessary ceremony. This allows the project to focus on building vertical slices before introducing heavier architecture patterns.

The backend should own the important rules of the system. The frontend may perform basic user experience validation, but the backend must remain the source of truth for data, rules and security.

## Alternatives Considered

| Alternative | Reason Not Selected |
|---|---|
| Node.js / Express | Useful and flexible, but less aligned with the main .NET-focused learning path. |
| Next.js API routes as the main backend | Would reduce the need for a separate backend, but would weaken the goal of learning ASP.NET Core and could blur frontend/backend responsibilities. |
| Full Clean Architecture from the start | Valuable later, but too much structure before the domain and first product slices are understood. |
| Blazor fullstack | Interesting within .NET, but React/Next.js gives stronger frontend market relevance for this project. |

## Consequences

### Positive

- The backend stack aligns with the main learning and career direction.
- ASP.NET Core gives a strong foundation for APIs, testing, validation, logging and deployment.
- Minimal APIs allow the project to start small.
- The backend can grow toward a more structured architecture later.
- The separation between frontend and backend responsibilities remains clear.

### Trade-offs

- A separate backend adds more setup than a purely Next.js-based application.
- Frontend and backend contracts must be kept in sync.
- Local development needs both the frontend and backend running.
- CORS and environment configuration must be handled correctly.

## Revisit Criteria

This decision should be reconsidered if:

- The project no longer needs a separate backend.
- The backend becomes so small that ASP.NET Core adds unnecessary overhead.
- The project direction changes toward a frontend-only prototype.
- A future deployment strategy strongly favors a different backend model.
