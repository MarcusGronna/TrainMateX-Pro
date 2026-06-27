# 0005 - Use PostgreSQL and EF Core for Exercise Persistence

## Status

Accepted

## Context

TrainMateX-Pro currently stores exercise data in a JSON file that the backend reads at runtime. This approach was sufficient for the first slice, which focused on proving the fullstack integration between Next.js and ASP.NET Core.

As the project moves toward CRUD operations, workout and program builders, client management and progress tracking, a real database is needed. The data model will eventually include exercises, clients, programs, workouts, logs and progression data, all of which have relational characteristics.

The project needs a database technology and an ORM/data access strategy that fits the ASP.NET Core backend, supports the relational domain and provides a practical local development experience.

## Decision

The project will use PostgreSQL as the database and Entity Framework Core with the Npgsql provider as the data access layer.

Local development will use PostgreSQL through Docker Compose to provide a repeatable and isolated database environment.

## Reasoning

PostgreSQL fits the relational domain that the project is moving toward. Exercises, clients, programs, workouts, logs and progression data are naturally relational and benefit from a structured schema with referential integrity.

EF Core is the standard ORM for ASP.NET Core applications. It integrates naturally with the existing backend, supports migrations, seeding and LINQ-based queries. Using EF Core keeps the learning path relevant for .NET backend development.

Docker Compose gives a repeatable local database setup. Any developer can start the database with a single command without installing PostgreSQL directly on their machine.

This decision prepares the project for future slices including CRUD endpoints, the workout and program builder, and progress tracking features.

## Alternatives Considered

| Alternative | Reason Not Selected |
|---|---|
| Continue with JSON files | Sufficient for read-only demos, but does not support concurrent writes, queries, relationships or transactions. Not viable as the project grows beyond a single read slice. |
| SQLite | Simpler setup (no Docker needed), but less representative of a production environment. Lacks some PostgreSQL features that may be useful later (JSON columns, full-text search, advanced indexing). Harder to share state across multiple backend instances if needed in the future. |
| SQL Server | Viable and well-supported with EF Core, but heavier for local development. PostgreSQL is free, widely used in production and has strong community tooling. SQL Server licensing is more complex for a portfolio/learning project. |
| Cloud-hosted database immediately | Adds cost, latency during development and external dependencies before the project needs them. Local-first development is simpler and faster for iteration. Cloud hosting can be introduced later when deployment is planned. |

## Consequences

### Positive

* The project gets a real persistence layer that supports relational data modeling.
* EF Core provides migrations, making schema changes trackable and reproducible.
* Docker Compose gives every developer the same database environment.
* The project is ready for future CRUD, workout/program and progress tracking slices.
* PostgreSQL is production-ready and widely used, making the project more portfolio-relevant.

### Trade-offs

* PostgreSQL adds local setup complexity. Developers need Docker installed and running.
* The first persistence slice does not visibly change the UI. Its value is in enabling future features.
* EF Core has a learning curve for developers unfamiliar with it.
* The database adds another component that must be started before the backend can run locally.

## Notes

* The first slice using PostgreSQL will be an enablement slice that replaces JSON reads with database reads, preserving the existing API contract.
* CRUD endpoints are not part of the initial persistence slice.
* The existing exercise JSON data should be migrated or seeded into the database so that the API continues to return the same data.
* The README should be updated with instructions for starting the database locally.

## Revisit Criteria

This decision should be reconsidered if:

* The project moves away from relational data modeling.
* Docker becomes an unacceptable dependency for local development.
* A managed cloud database becomes necessary before local development is established.
* The team decides to use a different backend framework that does not integrate well with EF Core.
