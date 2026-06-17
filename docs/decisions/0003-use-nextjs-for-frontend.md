# 0003 - Use Next.js for Frontend

## Status

Accepted

## Context

TrainMateX-Pro needs a frontend for users to browse exercises, view training content and later interact with workout plans, client pages, dashboards and training progress.

The frontend should support a product-like structure with routes, layouts, pages, reusable components and TypeScript. The project should also help improve skills in React, TypeScript and modern frontend development.

The backend will be built with ASP.NET Core, so the frontend should consume the backend API rather than own the main business logic.

## Decision

TrainMateX-Pro will use Next.js with React and TypeScript for the frontend.

Next.js will be responsible for:

- Frontend routing
- Page structure
- Layouts
- React components
- Forms and user interaction
- Data fetching from the ASP.NET Core API
- Loading and error states
- Product-facing user experience

The main backend logic, validation, persistence and security rules will remain in ASP.NET Core.

## Reasoning

Next.js is a strong fit because it builds on React while adding structure for real web applications.

Compared to a plain React/Vite setup, Next.js provides a more complete application framework with file-based routing, layouts, server and client components, metadata support and a clearer structure for larger applications.

This is useful for TrainMateX-Pro because the project is intended to grow beyond a small demo into a more product-like system.

Using TypeScript improves maintainability and makes the frontend contracts clearer when consuming data from the ASP.NET Core API.

## Alternatives Considered

| Alternative | Reason Not Selected |
|---|---|
| React with Vite | Simpler and very good for SPAs, but provides less built-in application structure than Next.js. |
| Angular | Powerful and structured, but less aligned with the chosen React-focused frontend path. |
| Blazor | Interesting for a .NET-centered stack, but React/Next.js has stronger relevance for the intended fullstack profile. |
| Next.js as fullstack backend and frontend | Would simplify the stack, but conflicts with the goal of using ASP.NET Core as the main backend. |

## Consequences

### Positive

- The frontend gets a clear structure for pages, layouts and routes.
- The project builds practical React and TypeScript skills.
- Next.js supports a more product-like user experience than a basic SPA setup.
- The frontend can grow naturally as new vertical slices are added.
- The separation between frontend and backend remains clear.

### Trade-offs

- Next.js adds more framework concepts than plain React.
- Server and client component boundaries need to be understood.
- Data fetching must be designed carefully to avoid duplicating backend logic.
- Local development requires coordination with the ASP.NET Core API.

## Revisit Criteria

This decision should be reconsidered if:

- The frontend becomes a very small internal-only SPA with no need for Next.js features.
- Next.js adds more complexity than value for the project.
- The project direction changes toward a different frontend framework.
- The frontend and backend responsibilities become unclear.

