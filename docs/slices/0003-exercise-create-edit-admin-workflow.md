# Slice 3 - Exercise Create and Edit Admin Workflow

## Status

Planned

## Context

Slice 1 proved that the Next.js frontend can fetch and render exercise data from the ASP.NET Core backend.

Slice 2 moved exercise reads from JSON files to PostgreSQL through EF Core, while preserving the existing public API contract.

The next step is to allow exercise data to be managed through the application instead of only through seed data. This slice introduces create and edit workflows for exercises through admin routes, while keeping the public exercise library read-only.

## Goal

Add fullstack create and edit workflows for exercises without changing the public exercise browsing experience.

## Non-Goals

* Deleting exercises.
* Authentication or authorization.
* Workout, program or client features.
* Search or filtering.
* Media uploads.
* Frontend automated tests.
* Advanced exercise lifecycle rules such as archiving or publishing.

## User / Developer Story

As a coach/admin-style user,
I want to create and edit exercises,
so that the exercise library can be maintained without editing seed data.

## In Scope

* Add `POST /api/exercises`.
* Add `PUT /api/exercises/{id}`.
* Generate exercise IDs as slugs from names on create.
* Keep exercise IDs stable when editing existing exercises.
* Validate required fields and compact enum values.
* Add admin frontend routes and forms for creating and editing exercises.
* Keep the public exercise library read-only.
* Add backend tests for create and edit behavior.

## Out of Scope

* Delete endpoint or delete UI.
* Authentication or authorization for admin routes.
* Workout or program builder features.
* Client accounts or roles.
* Search, filtering or sorting.
* Media uploads.
* Frontend automated tests.
* Database schema changes unless implementation reveals a missing requirement.

## Proposed Implementation Outline

1. Add request DTOs for creating and editing exercises.
2. Add validation for required fields, instructions and enum values.
3. Add slug generation from exercise name on create.
4. Add duplicate slug handling for create.
5. Add `POST /api/exercises`.
6. Add `PUT /api/exercises/{id}`.
7. Add admin frontend routes:
   * `/admin/exercises`
   * `/admin/exercises/new`
   * `/admin/exercises/[id]/edit`
8. Add create and edit forms that submit to the ASP.NET Core API with client-side `fetch`.
9. Show useful validation and conflict errors in the form UI.
10. Preserve the existing public `/exercises` and `/exercises/[id]` behavior.

## API Contract

### Create exercise

```http
POST /api/exercises
```

Expected request:

```json
{
  "name": "Overhead Press",
  "description": "A compound upper-body exercise performed by pressing a weight overhead.",
  "instructions": [
    "Stand with the bar at shoulder height.",
    "Brace your core.",
    "Press the bar overhead until your arms are extended."
  ],
  "muscleGroup": "Shoulders",
  "equipment": "Barbell",
  "difficultyLevel": "Intermediate"
}
```

Expected successful response:

```http
201 Created
```

The response body should use the exercise detail shape:

```json
{
  "id": "overhead-press",
  "name": "Overhead Press",
  "description": "A compound upper-body exercise performed by pressing a weight overhead.",
  "instructions": [
    "Stand with the bar at shoulder height.",
    "Brace your core.",
    "Press the bar overhead until your arms are extended."
  ],
  "muscleGroup": "Shoulders",
  "equipment": "Barbell",
  "difficultyLevel": "Intermediate"
}
```

If the generated ID already exists:

```http
409 Conflict
```

### Edit exercise

```http
PUT /api/exercises/{id}
```

Expected request:

```json
{
  "name": "Barbell Overhead Press",
  "description": "A compound upper-body exercise performed by pressing a barbell overhead.",
  "instructions": [
    "Stand with the bar at shoulder height.",
    "Brace your core.",
    "Press the bar overhead until your arms are extended."
  ],
  "muscleGroup": "Shoulders",
  "equipment": "Barbell",
  "difficultyLevel": "Intermediate"
}
```

Expected successful response:

```http
200 OK
```

The response body should use the exercise detail shape.

Editing an exercise name must not change the exercise ID.

If the exercise id does not exist:

```http
404 Not Found
```

### Validation errors

Invalid create or edit requests should return a ProblemDetails-style validation response.

## Validation Rules

Required fields:

* `name`
* `description`
* `muscleGroup`
* `equipment`
* `difficultyLevel`

Instruction rules:

* At least one non-empty instruction is required.
* Blank instruction rows should be rejected or normalized away before saving.

Allowed muscle groups:

* `Chest`
* `Back`
* `Legs`
* `Shoulders`
* `Arms`
* `Core`

Allowed equipment:

* `Barbell`
* `Dumbbell`
* `Bodyweight`
* `Machine`
* `Cable`
* `Kettlebell`

Allowed difficulty levels:

* `Beginner`
* `Intermediate`
* `Advanced`

Slug generation rules:

* Generate the ID from the exercise name on create.
* Convert to lowercase.
* Trim whitespace.
* Replace non-alphanumeric groups with `-`.
* Trim leading and trailing `-`.
* Reject the request if the generated ID is empty.
* Return `409 Conflict` if the generated ID already exists.

## Frontend Routes

```text
/admin/exercises
/admin/exercises/new
/admin/exercises/[id]/edit
```

### `/admin/exercises`

The admin exercise management list.

Responsibilities:

* Show existing exercises.
* Link to create a new exercise.
* Link to edit an existing exercise.

### `/admin/exercises/new`

The create exercise page.

Responsibilities:

* Render the exercise form with empty fields.
* Submit a create request to the ASP.NET Core API.
* Show validation or conflict errors.
* Redirect to the created exercise detail page after success.

### `/admin/exercises/[id]/edit`

The edit exercise page.

Responsibilities:

* Fetch the current exercise details.
* Render the exercise form with existing values.
* Submit an edit request to the ASP.NET Core API.
* Show validation errors.
* Redirect to the exercise detail page after success.
* Show a reasonable not-found state if the exercise does not exist.

## Backend Tasks

* Add create and edit request DTOs.
* Add validation for required fields, instructions and enum values.
* Add slug generation for new exercise IDs.
* Add duplicate ID handling for create.
* Add `POST /api/exercises`.
* Add `PUT /api/exercises/{id}`.
* Return ProblemDetails-style validation errors.
* Return `409 Conflict` for duplicate generated IDs.
* Return `404 Not Found` when editing an unknown exercise ID.
* Keep existing read endpoints unchanged.

## Frontend Tasks

* Add admin exercise management route.
* Add create exercise route.
* Add edit exercise route.
* Add TypeScript types for create and edit requests.
* Add API functions for create and edit.
* Add a reusable exercise form component.
* Add enum select controls for muscle group, equipment and difficulty.
* Add instruction row add/remove behavior.
* Add loading and error states for form submissions.
* Redirect after successful create and edit.

## Tests

### Backend tests

* `POST /api/exercises` returns `201 Created` for a valid request.
* `POST /api/exercises` stores the created exercise.
* `POST /api/exercises` returns `409 Conflict` for a duplicate generated ID.
* `POST /api/exercises` returns validation errors for missing required fields.
* `POST /api/exercises` returns validation errors for invalid enum values.
* `PUT /api/exercises/{id}` returns `200 OK` for a valid edit request.
* `PUT /api/exercises/{id}` updates editable fields.
* `PUT /api/exercises/{id}` keeps the existing ID stable when the name changes.
* `PUT /api/exercises/{id}` returns `404 Not Found` for an unknown exercise ID.
* Existing read endpoint tests continue to pass.

### Frontend tests

Frontend automated tests are not required for this slice.

Manual verification is enough for this slice, but automated frontend tests can be added later when the frontend testing setup is introduced.

## Acceptance Criteria

* A user can open the admin exercise management page.
* A user can create an exercise from the admin UI.
* The created exercise appears in the public exercise library.
* The created exercise detail page works.
* A user can edit an existing exercise from the admin UI.
* Editing an exercise name does not change the exercise ID.
* Invalid create and edit submissions show useful errors.
* Creating an exercise with a duplicate generated slug returns a conflict.
* Existing public exercise list and detail pages continue to work.
* No delete endpoint or delete UI is added.
* No authentication or authorization is added.
* Backend tests verify create and edit behavior.

## Definition of Done

* Backend exposes `POST /api/exercises`.
* Backend exposes `PUT /api/exercises/{id}`.
* Backend validates create and edit requests.
* Backend generates readable slug IDs on create.
* Backend keeps IDs stable on edit.
* Frontend has `/admin/exercises` route.
* Frontend has `/admin/exercises/new` route.
* Frontend has `/admin/exercises/[id]/edit` route.
* Admin forms can create and edit exercises through the ASP.NET Core API.
* Public exercise browsing behavior is preserved.
* Backend tests pass.
* No delete, auth or workout/program features are introduced.

## Risks / Notes

* `/admin` is route organization only. It does not provide security.
* Real authentication and authorization should be added in a later slice.
* The compact enum values may need to expand as the exercise library grows.
* Stable IDs avoid broken links, but edited exercise names may no longer match the original slug.
* No schema migration is expected unless implementation reveals a missing database requirement.

