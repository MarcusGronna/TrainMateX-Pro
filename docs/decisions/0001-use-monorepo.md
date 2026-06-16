# 0001 Use Monorepo

## Status
Accepted

## Context
TrainMateX-Pro is a fullstack project with an ASP.NET Core backend and a Next.js frontend.
The project will grow over time as new features, architecture patterns, tests, tooling and learning labs are added.
The first goal is to keep the project easy to navigate while supporting fullstack vertical slices.

## Decision
The project will use a monorepo structure.
The backend, frontend, documentation, labs and tooling will live in the same Git repository.

```text
TrainMateX-Pro/
	backend/
	frontend/
	docs/
	labs/
	tools/
```

## Reasoning
A single feature often requires changes in both the API and the frontend. Keeping them in one repository makes it easier to implement and review a complete vertical slice.
Since this is also a learning and portfolio project, a monorepo keeps the full system context visible in one place.
Architecture decisions and technical documentation can live close to the code they describe.
Commits can describe complete product changes instead of being split across separate repositories.
A monorepo reduces coordination overhead wile the project is still small.

## Alternatives Considered
- Separate repository for backend.
- Separate repository for frontend.
Separate repositories can be useful when frontend and backend are developed, deployed and versioned independently.
For this project, separate repositories would add unnecessary coordination overhead early on.

## Consequences 
### Positive
- Easier to work on fullstack features.
- Simpler project overview.
- Documentation stays close to the implementation.
- Easier to keep backend and frontend changes in the same commit or pull request.

### Trade-offs
- The repository may become larger over time.
- CI/CD configuration may need clear separation between backend and frontend workflows.
- The frontend and backend may eventually need separate deployment pipelines.

## Revisit Criteria
- The backend and frontend need completely separate release cycles.
- The repository becomes difficult to navigate or maintain.
- Separate teams work independently on the frontend and backend. 
- CI/CD becomes too complex to manage in one repository.
