# 0004 - Select Exercise Library Viewer as First Slice

## Status

Accepted

## Context

TrainMateX-Pro is a fullstack training application built with ASP.NET Core, Next.js and React.

The project has already chosen a monorepo structure, ASP.NET Core for the backend and Next.js for the frontend. The next decision is what the first product slice should be.

The first slice should be small enough to build without unnecessary complexity, but still valuable enough to represent a real product capability.

The project should avoid starting with advanced architecture, authentication, roles, dashboards or workout planning before the basic product flow has been proven.

## Decision

The first product slice will be an Exercise Library Viewer.

A client should be able to open an exercise library, select an exercise and view instructions for how the exercise should be performed.

The first version will be read-only.

## User Story

As a client,  
I want to browse an exercise library and open an exercise detail page,  
so that I can understand how an exercise should be performed.

## Reasoning

This slice was selected because it is small, user-facing and directly connected to the core domain of TrainMateX-Pro.

It also gives the project a useful first end-to-end flow:

- Next.js route for the exercise library
- React components for list and detail views
- TypeScript types for exercise data
- ASP.NET Core API endpoints for exercises
- Basic frontend/backend integration
- A foundation for later workout and program features

The slice avoids unnecessary early complexity while still proving that the frontend and backend can work together.

## Alternatives Considered

| Alternative | Reason Not Selected |
|---|---|
| Coach creates exercises | Useful, but introduces create forms, validation and administration earlier than needed. |
| Coach creates workouts | Important later, but depends on exercises existing first. |
| Client views today's workout | Valuable, but requires workout and assignment concepts before the basics are established. |
| Client logs completed workout | High value, but too complex for the first slice because it requires workout sessions, results and history. |
| Public landing page | Simple to build, but weaker as a fullstack product slice. |

## In Scope

- Show a list of exercises
- Show basic exercise information in the list
- Allow a client to select an exercise
- Show an exercise detail page
- Show instructions for how the exercise should be performed
- Fetch exercise data from the ASP.NET Core API

## Out of Scope

- Creating exercises
- Editing exercises
- Deleting exercises
- Authentication
- Client accounts
- Coach/admin roles
- Workout plans
- Completed workout tracking
- Media uploads
- Advanced filtering
- Search
- Dashboards

## Acceptance Criteria

- A client can open the exercise library page.
- The exercise library displays a list of exercises.
- A client can select an exercise from the list.
- The selected exercise opens in a detail view.
- The detail view shows instructions for how the exercise should be performed.
- Exercise data is provided by the ASP.NET Core API.

## Consequences

### Positive

- The first slice is small and realistic.
- The project gets an early end-to-end fullstack flow.
- The slice creates a foundation for future workout-related features.
- The frontend can start with basic routing, components and data fetching.
- The backend can start with simple read endpoints before write operations are added.

### Trade-offs

- The first slice does not yet allow users to create or manage exercises.
- The slice provides limited product value until more features are added.
- Exercise data may need to be seeded or hardcoded before database persistence is introduced.
- More advanced functionality will need separate future slices.

## Revisit Criteria

This decision should be reconsidered if:

- The project needs to prioritize coach workflows before client-facing flows.
- The first slice becomes too small to demonstrate meaningful fullstack functionality.
- Exercise data cannot be modeled clearly enough without first defining workouts or programs.
- The project direction changes from client education to coach administration.
