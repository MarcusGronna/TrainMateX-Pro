# Slice 5 - Workout Template Create and View

## Status

Planned

## Context

Slices 1 through 4 established a PostgreSQL-backed exercise library with public read routes and an admin-style full CRUD workflow.

Exercises are currently independent records. The application does not yet let a coach combine exercises into a reusable training structure. Workout templates are the next small product capability that can build on the exercise library while introducing the first persisted relationship in the domain.

This slice adds creation and viewing of workout templates. It intentionally stops before editing, deleting, assigning, scheduling or completing workouts.

Because workout templates will reference exercises, this slice must also revise exercise deletion behavior. An exercise referenced by a workout template must not be permanently deleted.

## Goal

Allow a coach/admin-style user to create a reusable workout template from existing exercises and view the saved template in exercise order.

## Non-Goals

* Editing or deleting workout templates.
* Assigning workout templates to clients.
* Scheduling workouts.
* Recording workout completion, results or history.
* Authentication or authorization.
* Client accounts or coach/client roles.
* Supersets, circuits or exercise groups.
* Rest timers, tempo, RPE, weight targets or progression rules.
* Search or filtering.
* Frontend automated test framework setup.

## User / Developer Story

As a coach/admin-style user,
I want to create a reusable workout template from exercises in the library,
so that I can define and review an ordered training session.

## In Scope

* Persist workout templates in PostgreSQL through EF Core.
* Persist an ordered relationship between workout templates and exercises.
* Store sets and reps for each exercise in a template.
* Generate workout template IDs on the backend as UUIDs.
* Add workout template list, detail and create API endpoints.
* Add an admin-style workout template management page.
* Add a workout template creation form.
* Add a workout template detail page.
* Validate the workout template and all referenced exercises on the backend.
* Preserve the exercise order supplied by the client.
* Prevent deletion of exercises referenced by workout templates.
* Return `409 Conflict` when deletion is blocked by a workout-template reference.
* Add backend service and endpoint tests for the new behavior.

## Out of Scope

* `PUT` or `PATCH` endpoints for workout templates.
* A workout template delete endpoint.
* Soft deletion or archiving of workout templates.
* Duplicate, copy or version-template workflows.
* Client-specific workout instances.
* Dates, calendars or recurring schedules.
* Performed sets, weights, repetitions or completion status.
* Exercise alternatives or substitutions.
* Drag-and-drop ordering. Simple add, remove and move controls are sufficient.
* Changing exercise details from the workout template form.
* Authentication or route protection.

## Proposed Implementation Outline

1. Add `WorkoutTemplate` and `WorkoutTemplateExercise` EF Core entities.
2. Configure their relationship and deletion behavior in `AppDbContext`.
3. Add and apply an EF Core migration.
4. Add request and response DTOs for workout template list, detail and creation.
5. Add backend validation for template fields and exercise selections.
6. Add a focused workout template service for create and read behavior.
7. Add `GET /api/workout-templates`.
8. Add `GET /api/workout-templates/{id}`.
9. Add `POST /api/workout-templates`.
10. Change exercise deletion results to represent deleted, not-found and conflict outcomes.
11. Return `409 Conflict` when an exercise is referenced by a workout template.
12. Add typed frontend workout-template contracts and API functions.
13. Add `/admin/workout-templates` and `/admin/workout-templates/new`.
14. Add `/workout-templates/{id}`.
15. Keep initial data fetching in Server Components and form interaction in a Client Component.
16. Verify the complete flow and preserve existing exercise behavior.

## Domain Model

### WorkoutTemplate

```text
Id: Guid
Name: string
Description: string
Exercises: collection of WorkoutTemplateExercise
```

### WorkoutTemplateExercise

```text
WorkoutTemplateId: Guid
ExerciseId: string
Position: int
Sets: int
Reps: int
```

`WorkoutTemplateExercise` is an explicit join entity because the relationship contains domain data: exercise order, sets and reps.

The join entity should use a composite key equivalent to:

```text
WorkoutTemplateId + ExerciseId
```

This prevents the same exercise from appearing more than once in a template for this slice. Repeated exercises can be reconsidered later if real workout design requirements justify them.

`Position` should have a unique constraint or index within each workout template if the chosen EF Core/PostgreSQL mapping can express it cleanly. Backend validation remains responsible for producing useful request errors.

## Identifier Decision

Workout template IDs will be backend-generated UUIDs.

Unlike exercise names, workout-template names such as `Push Day` or `Full Body` are reasonably expected to repeat. A UUID allows duplicate display names without introducing slug-collision rules or making names part of identity.

The API and frontend should represent the UUID as a string at the HTTP boundary.

## API Contract

### List workout templates

```http
GET /api/workout-templates
```

Expected response:

```json
[
  {
    "id": "6041ac13-95f1-4ab4-a42f-f17fa9642898",
    "name": "Push Day",
    "description": "A chest, shoulder and triceps workout.",
    "exerciseCount": 3
  }
]
```

### Get workout template details

```http
GET /api/workout-templates/{id}
```

Expected response:

```json
{
  "id": "6041ac13-95f1-4ab4-a42f-f17fa9642898",
  "name": "Push Day",
  "description": "A chest, shoulder and triceps workout.",
  "exercises": [
    {
      "position": 1,
      "sets": 4,
      "reps": 8,
      "exercise": {
        "id": "bench-press",
        "name": "Bench Press",
        "muscleGroup": "Chest",
        "equipment": "Barbell",
        "difficultyLevel": "Intermediate"
      }
    }
  ]
}
```

If the workout template does not exist:

```http
404 Not Found
```

An invalid UUID route value should also produce a deliberate `404 Not Found` or `400 Bad Request` response. The implementation should choose one behavior and cover it with an endpoint test.

### Create workout template

```http
POST /api/workout-templates
```

Expected request:

```json
{
  "name": "Push Day",
  "description": "A chest, shoulder and triceps workout.",
  "exercises": [
    {
      "exerciseId": "bench-press",
      "sets": 4,
      "reps": 8
    },
    {
      "exerciseId": "overhead-press",
      "sets": 3,
      "reps": 10
    }
  ]
}
```

The array order is authoritative. The backend assigns persisted positions starting at `1`; clients do not send a separate position value.

Expected successful response:

```http
201 Created
Location: /api/workout-templates/{id}
```

The response body should use the workout template detail shape.

Invalid requests should return a ProblemDetails-style validation response:

```http
400 Bad Request
```

## Validation Rules

* `name` is required and cannot contain only whitespace.
* `description` is required and cannot contain only whitespace.
* At least one exercise is required.
* Every `exerciseId` must identify an existing exercise.
* An exercise may appear only once in a workout template in this slice.
* `sets` must be a positive integer.
* `reps` must be a positive integer.
* The backend assigns contiguous positions from the request array order.
* A failed request must not persist a partial workout template or partial exercise list.

The backend remains the source of truth. Frontend controls may prevent obvious invalid values for user experience, but they do not replace backend validation.

## Exercise Deletion Contract Change

Once workout template references exist, exercise deletion has three expected outcomes:

```text
Deleted
NotFound
Conflict
```

The current boolean service result is no longer expressive enough and should be replaced with a small result enum or result record.

### Delete an unreferenced exercise

```http
DELETE /api/exercises/{id}
```

```http
204 No Content
```

### Delete an unknown exercise

```http
404 Not Found
```

### Delete a referenced exercise

```http
409 Conflict
```

The conflict response should provide a stable, useful message explaining that the exercise is used by one or more workout templates.

EF Core should configure the exercise relationship with restrictive deletion behavior. The service should detect expected references explicitly so the API can return a deliberate conflict instead of exposing a provider exception.

No cascade deletion from an exercise to workout template entries is allowed.

## Backend Architecture

Minimal API endpoints should remain in the API project and delegate behavior to a focused service, following the current exercise feature pattern.

Responsibilities:

* Minimal API endpoints own HTTP request and response mapping.
* DTOs define the HTTP contract.
* Validation and service behavior own creation rules and expected outcomes.
* EF Core entities and `AppDbContext` own persistence mapping.
* Mapper extensions may be used where they keep entity-to-DTO conversion clear.

Do not introduce repository, command-handler or multi-project architecture layers for this slice. The current single-project backend remains sufficient.

Creation should load and validate all referenced exercises before adding the workout template. A single `SaveChangesAsync` call should persist the complete aggregate so invalid requests cannot leave partial state.

Read queries should use `AsNoTracking` and project only the data required by their response DTOs where practical.

## Frontend Routes

```text
/admin/workout-templates
/admin/workout-templates/new
/workout-templates/{id}
```

### `/admin/workout-templates`

Responsibilities:

* Fetch and display existing workout templates.
* Show template name, description and exercise count.
* Link each template to its detail page.
* Link to the creation page.

### `/admin/workout-templates/new`

Responsibilities:

* Fetch available exercises on the server.
* Pass serializable exercise choices into a Client Component form.
* Capture template name and description.
* Add and remove exercise rows.
* Select one existing exercise per row.
* Capture positive sets and reps values.
* Allow rows to be moved up and down.
* Submit the array in visible order.
* Display backend validation and unexpected request errors.
* Redirect to the created workout template detail page after success.

### `/workout-templates/{id}`

Responsibilities:

* Fetch the workout template detail on the server.
* Show name and description.
* Render exercises in persisted position order.
* Show exercise name, sets and reps for every row.
* Link exercise names to their existing public detail pages.
* Show a reasonable not-found state for an unknown template.

## Frontend State Model

The create form should keep one state object for scalar template fields and one ordered array for exercise rows.

Each client-side row needs a temporary UI key that is separate from `exerciseId`. This keeps React row identity stable while the user changes exercise selections or reorders rows.

The temporary key must not be sent in the API request.

Avoid duplicating derived state. The visible array order should determine the submitted order, and available exercise details should be read from the server-provided choices.

## Backend Tasks

* Add `WorkoutTemplate` and `WorkoutTemplateExercise` entities.
* Add their `DbSet` properties and EF Core mapping.
* Configure required fields, composite key, relationship and restrictive deletion.
* Add a migration for the workout template tables and constraints.
* Add create, list and detail DTOs.
* Add workout template validation.
* Add workout template create and read service methods.
* Add list, detail and create endpoints.
* Return `201 Created` with a `Location` header after creation.
* Return `404 Not Found` for an unknown workout template.
* Replace the exercise deletion boolean with a three-outcome result.
* Return `409 Conflict` when an exercise is referenced.
* Preserve `204` and `404` exercise deletion behavior for the existing cases.
* Add service and endpoint tests.

## Frontend Tasks

* Add workout template TypeScript contracts.
* Add centralized workout template API functions.
* Add the admin workout template list route.
* Add the workout template creation route.
* Add a Client Component form for ordered exercise rows.
* Add add, remove, move-up and move-down controls.
* Add pending and error submission states.
* Redirect to the detail route after successful creation.
* Add the workout template detail route.
* Update exercise deletion error handling to display the new conflict response.
* Preserve all existing exercise routes and workflows.

## Tests

### Backend service tests

* Creating a valid workout template returns the created template.
* Creation persists the template and every selected exercise row.
* Creation preserves request-array order as contiguous persisted positions.
* Creation allows duplicate template names.
* Creation rejects a blank name.
* Creation rejects a blank description.
* Creation rejects an empty exercise list.
* Creation rejects an unknown exercise ID.
* Creation rejects duplicate exercise IDs.
* Creation rejects non-positive sets or reps.
* A rejected creation does not persist partial data.
* Getting an existing template returns exercises in position order.
* Getting an unknown template returns the expected not-found service outcome.
* Deleting an exercise referenced by a template returns conflict and preserves the exercise.
* Deleting an unreferenced exercise still succeeds.
* Deleting an unknown exercise still returns not found.

### Backend endpoint tests

* `GET /api/workout-templates` returns `200 OK` and list DTOs.
* `GET /api/workout-templates/{id}` returns the complete ordered detail DTO.
* `GET /api/workout-templates/{id}` returns `404 Not Found` for an unknown ID.
* `POST /api/workout-templates` returns `201 Created` for a valid request.
* A successful create response contains the expected `Location` header.
* The created template is subsequently available through the detail endpoint.
* Invalid create requests return ProblemDetails-style `400 Bad Request` responses.
* Referenced exercise deletion returns `409 Conflict`.
* Referenced exercise deletion does not remove the exercise or workout template.
* Existing exercise read, create, edit and unreferenced-delete endpoint tests continue to pass.

EF Core InMemory tests provide fast service and HTTP-contract confidence but do not prove PostgreSQL foreign-key behavior. The migration and restrictive relationship should also be verified against the local PostgreSQL database.

### Frontend tests

Frontend automated tests are not required for this slice.

Manual verification should cover adding, removing and reordering rows; validation errors; pending submission; successful creation; detail rendering; not-found rendering; and blocked deletion of a referenced exercise.

## Acceptance Criteria

* An admin-style user can open the workout template management page.
* Existing workout templates are listed with exercise counts.
* A user can open the create page and see available exercises.
* A user can enter a name and description.
* A user can add, remove and reorder exercise rows.
* A user can assign positive sets and reps to every selected exercise.
* Invalid submissions display useful errors and persist nothing.
* A valid submission creates one workout template with the selected exercises.
* The saved exercise order matches the order shown at submission time.
* Successful creation redirects to the new template detail page.
* The detail page displays the complete ordered workout template.
* Exercise names link to their existing public detail pages.
* Unknown workout template IDs show the intended not-found state.
* An exercise referenced by a workout template cannot be deleted.
* Referenced exercise deletion returns `409 Conflict` and useful UI feedback.
* Unreferenced exercise deletion continues to work.
* Existing exercise library and administration workflows continue to work.
* Backend tests cover workout creation, reads, validation and deletion conflicts.

## Definition of Done

* PostgreSQL schema includes workout templates and ordered exercise relationships.
* The EF Core migration is committed and applies successfully.
* Backend exposes workout template list, detail and create endpoints.
* Workout template creation validates all input and references before persistence.
* Creation persists the template and rows atomically.
* Backend-generated UUIDs identify workout templates.
* Workout template names are allowed to repeat.
* Frontend exposes the management list, creation form and detail routes.
* The form supports ordered exercise selection with sets and reps.
* Successful creation redirects to a correctly rendered detail page.
* Referenced exercise deletion returns `409 Conflict` without deleting data.
* Existing unreferenced exercise deletion behavior remains intact.
* Backend tests pass.
* Frontend lint and production build pass.
* PostgreSQL migration and relationship behavior are manually verified.
* Manual frontend verification passes.
* No edit, delete, assignment, scheduling, completion or authentication features are introduced.

## Risks / Notes

* This slice creates the first persisted relationship to exercises, so exercise lifecycle behavior becomes more constrained.
* UUID route values are less readable than exercise slugs but allow duplicate template names and stable identity.
* The explicit join entity is necessary because sets, reps and order belong to the relationship rather than to the exercise.
* The database constraint protects integrity, while explicit service validation provides useful API errors.
* EF Core InMemory does not reproduce every PostgreSQL constraint behavior.
* The `/admin` routes and mutation endpoints remain unprotected and must not be treated as secure production administration functionality.
* Sets and reps are deliberately simple integers. Richer prescriptions should wait for evidence from later workout requirements.

