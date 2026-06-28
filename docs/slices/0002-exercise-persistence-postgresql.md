# Slice 2 - Exercise Persistence with PostgreSQL

## Status

Accepted

## Context

Slice 1 proved that the Next.js frontend can fetch and render exercise data from the ASP.NET Core backend. The current backend uses a JSON file as the data source for exercises.

As the project grows toward CRUD operations, workout and program builders, client management and progress tracking, a real database is needed. This slice introduces PostgreSQL as the persistence layer for exercises, replacing the JSON-backed reads while preserving the existing API contract.

This is an enablement slice. The frontend behavior and API responses should remain unchanged after this slice is complete.

## Goal

Replace the JSON-backed exercise data source with a PostgreSQL database accessed through EF Core, without changing the API contract or breaking the frontend.

## Non-Goals

* Adding CRUD endpoints for exercises.
* Adding workout, program or client features.
* Changing the exercise API response shape.
* Requiring meaningful frontend changes.
* Cloud-hosted database setup.
* Authentication or authorization.

## User / Developer Story

As a developer,
I want exercises to be stored in PostgreSQL and accessed through EF Core,
so that the project has a real persistence layer ready for future CRUD, workout and progress features.

## In Scope

* Add PostgreSQL as a local development dependency through Docker Compose.
* Add EF Core with the Npgsql provider to the backend.
* Define an EF Core DbContext and Exercise entity.
* Create an initial migration that sets up the exercises table.
* Seed the database with the existing exercise data from the JSON file.
* Replace the JSON-based exercise repository/service with a database-backed implementation.
* Preserve the existing API contract:
  * `GET /api/exercises` returns the exercise list.
  * `GET /api/exercises/{id}` returns exercise details.
  * Missing exercise id returns `404 Not Found`.
* Update the README with local database startup instructions.

## Out of Scope

* CRUD endpoints (create, update, delete exercises).
* Workout or program features.
* Client accounts or authentication.
* Production database hosting or configuration.
* Advanced database features (full-text search, indexing beyond primary key).
* Frontend changes beyond trivial adjustments if needed.

## Proposed Implementation Outline

1. Add a `docker-compose.yml` at the repository root (or in `backend/`) with a PostgreSQL service.
2. Add EF Core and Npgsql NuGet packages to the backend project.
3. Define an `Exercise` entity matching the existing exercise data shape.
4. Create an `AppDbContext` (or similar) with a `DbSet<Exercise>`.
5. Configure the connection string for local development (e.g. via `appsettings.Development.json`).
6. Create an initial EF Core migration.
7. Seed the database with the existing exercise JSON data (via EF Core data seeding or a startup seed step).
8. Replace or update the exercise data access layer to query PostgreSQL instead of reading from JSON.
9. Verify that existing endpoints return the same responses as before.
10. Update the README with instructions for starting the database and running migrations.

## Acceptance Criteria

* PostgreSQL runs locally through Docker Compose with a single command.
* The backend connects to PostgreSQL using EF Core and the Npgsql provider.
* Existing exercise data from the JSON file is seeded into the database.
* `GET /api/exercises` returns `200 OK` with the exercise list from the database.
* `GET /api/exercises/{id}` returns `200 OK` with exercise details from the database.
* `GET /api/exercises/{id}` returns `404 Not Found` for an unknown exercise id.
* The API response shape is unchanged from Slice 1.
* The frontend continues to work without meaningful changes.
* The README documents how to start the local database and run migrations.
* CRUD endpoints are not introduced in this slice.

## Test Strategy

### Backend tests

* `GET /api/exercises` returns `200 OK`.
* `GET /api/exercises` returns a list of exercises from the database.
* `GET /api/exercises/{id}` returns `200 OK` for a seeded exercise.
* `GET /api/exercises/{id}` returns the expected exercise details.
* `GET /api/exercises/{id}` returns `404 Not Found` for an unknown exercise id.
* Tests should use an in-memory database or a test container to avoid requiring a running PostgreSQL instance during CI.

### Frontend tests

No frontend test changes are expected for this slice. Manual verification that the exercise library still loads correctly is sufficient.

## Definition of Done

* Docker Compose starts PostgreSQL locally.
* EF Core migration creates the exercises table.
* Exercise data is seeded from the existing JSON data.
* `GET /api/exercises` returns exercises from the database.
* `GET /api/exercises/{id}` returns exercise details from the database.
* Unknown exercise id returns `404 Not Found`.
* API response shape matches Slice 1 contract.
* Frontend works without meaningful changes.
* Backend tests pass and verify the existing API behavior is preserved.
* README includes local database startup instructions.
* No CRUD endpoints are added.

## Risks / Notes

* PostgreSQL adds local setup complexity. Docker Compose mitigates this but requires Docker to be installed.
* The slice does not visibly change the UI. Its value is entirely in enabling future features.
* If the existing JSON data shape does not map cleanly to a relational schema, minor adjustments to the entity model may be needed (e.g. storing instructions as a JSON column or a separate table).
* EF Core migrations should be committed to source control so that other developers can reproduce the database schema.
* The seed data should match what currently exists in the JSON file so that tests and manual verification remain consistent.
