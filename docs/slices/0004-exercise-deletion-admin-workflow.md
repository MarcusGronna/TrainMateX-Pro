# Slice 4 - Exercise Deletion Admin Workflow

## Status

Planned

## Context

Slice 1 introduced the public exercise library.

Slice 2 moved exercise persistence to PostgreSQL through EF Core.

Slice 3 added admin-style workflows for listing, creating and editing exercises. Exercises can now be maintained through the application, but removing obsolete or test data still requires direct database access.

This slice completes exercise CRUD by adding a focused hard-delete workflow before workout or program relationships introduce referential and lifecycle constraints.

## Goal

Allow an admin-style user to permanently delete an exercise through the application and receive clear feedback when the exercise does not exist or the operation fails.

## Non-Goals

* Bulk deletion or a delete-all operation.
* Database reset tooling.
* Soft deletion, archiving, publishing or recovery.
* Authentication or authorization.
* Workout, program or client features.
* Handling references from future workout or program entities.
* Changing startup seeding behavior.
* Frontend automated tests.

## User / Developer Story

As a coach/admin-style user,
I want to delete exercises that are no longer needed,
so that I can maintain and clean the exercise library without direct database access.

## In Scope

* Add `DELETE /api/exercises/{id}`.
* Permanently remove an existing exercise from PostgreSQL through EF Core.
* Return `204 No Content` after a successful deletion.
* Return `404 Not Found` when the exercise does not exist.
* Return `bool` from the service deletion method to represent the two current outcomes.
* Add a delete action to the admin exercise management page.
* Require explicit user confirmation before sending the delete request.
* Show pending and failure states in the delete UI.
* Refresh the admin exercise list after a successful deletion.
* Add backend service and endpoint tests for delete behavior.
* Preserve the existing startup seeding behavior.

## Out of Scope

* `DELETE /api/exercises` for deleting multiple or all exercises.
* A dedicated database cleanup or reset endpoint.
* Undo or recovery after deletion.
* Foreign-key conflict handling for future workout references.
* Cascading deletion.
* Schema changes or EF Core migrations unless implementation reveals an unexpected requirement.
* Protecting the delete endpoint with authentication or authorization.
* Frontend test framework setup.

## Proposed Implementation Outline

1. Add `DeleteExerciseAsync` to `ExerciseService`.
2. Find the exercise by ID using EF Core.
3. Return `false` without saving when the exercise does not exist.
4. Remove the existing exercise, save the change and return `true`.
5. Add `DELETE /api/exercises/{id}` to the Minimal API endpoints.
6. Map a `true` service result to `204 No Content`.
7. Map a `false` service result to `404 Not Found`.
8. Add a typed frontend API function for deleting an exercise.
9. Extract the interactive admin exercise list into a Client Component while keeping list fetching in the Server Component page.
10. Add per-exercise delete controls with confirmation, pending state and error feedback.
11. Refresh the server-rendered admin list after successful deletion.
12. Verify that existing read, create and edit workflows remain unchanged.

## API Contract

### Delete exercise

```http
DELETE /api/exercises/{id}
```

The request has no body.

If the exercise exists and is deleted:

```http
204 No Content
```

The successful response has no body.

If the exercise does not exist:

```http
404 Not Found
```

The not-found response does not need a custom body in this slice.

## Service Contract

The service should expose a method equivalent to:

```csharp
public async Task<bool> DeleteExerciseAsync(
    string id,
    CancellationToken cancellationToken = default)
```

Return values:

* `true` means the exercise existed and was deleted successfully.
* `false` means no exercise with the supplied ID existed.

A boolean is sufficient because deletion currently has exactly two expected domain outcomes. A dedicated result type should be introduced later if exercise relationships add another meaningful outcome, such as a conflict caused by workout references.

## Deletion Rules

* Deletion is permanent.
* The backend remains the authority for whether an exercise exists.
* A successful deletion must be persisted before returning `true`.
* Deleting an unknown ID is not treated as success in this API; it returns `404 Not Found`.
* The endpoint must not return the deleted entity.
* Deleting one exercise must not modify any other exercise.
* No cascade behavior is required because no entities currently reference exercises.

## Frontend Architecture

The `/admin/exercises` page should remain a Server Component responsible for fetching the exercise list.

The interactive list or delete control should be a Client Component responsible for:

* Asking the user to confirm deletion.
* Sending the client-side `DELETE` request to the ASP.NET Core API.
* Tracking which exercise is currently being deleted.
* Preventing duplicate submissions while deletion is pending.
* Showing an actionable error when deletion fails.
* Calling `router.refresh()` after success so the Server Component fetches the authoritative list again.

This keeps data fetching on the server while moving only the interactive mutation boundary to the client.

## Confirmation Behavior

* The user must explicitly confirm before the request is sent.
* Cancelling confirmation must leave the exercise unchanged.
* The confirmation text should identify the exercise by name.
* The delete control should be disabled while its request is pending.
* A failed request should preserve the exercise in the visible list and display an error.

A native browser confirmation is acceptable for this focused slice. A custom modal can be introduced later if the application adopts a shared dialog system or requires richer accessible confirmation behavior.

## Startup Seeding Behavior

The existing startup seeder remains unchanged.

Its current behavior is:

* If at least one exercise exists, startup seeding does nothing.
* If the exercise table is empty when the backend starts, the seed exercises are inserted again.

Therefore, deleting individual exercises persists across restarts while other exercises remain. Deleting every exercise does not keep the table empty across a backend restart. This is accepted for the current local learning workflow and is not changed by this slice.

## Backend Tasks

* Add `DeleteExerciseAsync` to `ExerciseService` with a `Task<bool>` return type.
* Use EF Core to find and remove the requested exercise.
* Save changes only when an exercise is found.
* Add `DELETE /api/exercises/{id}`.
* Return `204 No Content` for a successful deletion.
* Return `404 Not Found` for an unknown exercise ID.
* Add service tests for found and missing exercises.
* Add endpoint tests for successful and missing deletions.
* Keep existing endpoints and seeding behavior unchanged.

## Frontend Tasks

* Add a typed `deleteExercise` API function.
* Introduce a Client Component at the interactive admin-list boundary.
* Add a delete control for each exercise.
* Confirm destructive intent before calling the API.
* Track and display deletion progress.
* Prevent repeated deletion requests while pending.
* Show useful request failures.
* Refresh the admin list after successful deletion.
* Preserve create and edit navigation.

## Tests

### Backend service tests

* `DeleteExerciseAsync` returns `true` for an existing exercise.
* `DeleteExerciseAsync` removes the exercise from persistence.
* `DeleteExerciseAsync` returns `false` for an unknown exercise ID.
* Deleting an unknown exercise does not modify existing exercises.

### Backend endpoint tests

* `DELETE /api/exercises/{id}` returns `204 No Content` for an existing exercise.
* A deleted exercise subsequently returns `404 Not Found` from `GET /api/exercises/{id}`.
* `DELETE /api/exercises/{id}` returns `404 Not Found` for an unknown exercise ID.
* Other exercises remain available after one exercise is deleted.
* Existing read, create and edit endpoint tests continue to pass.

### Frontend tests

Frontend automated tests are not required for this slice.

Manual verification should cover confirmation, cancellation, successful deletion, pending behavior, request failure and list refresh.

## Acceptance Criteria

* An admin-style user can see a delete action for each exercise.
* Cancelling confirmation does not delete the exercise.
* Confirming deletion permanently removes the selected exercise.
* The admin list refreshes and no longer displays the deleted exercise.
* The public exercise library no longer displays the deleted exercise.
* Opening the deleted exercise detail route shows the existing not-found state.
* Deleting an unknown exercise returns `404 Not Found`.
* A failed delete request shows useful feedback without removing the item from the UI.
* Deleting one exercise does not affect other exercises.
* Existing create and edit workflows continue to work.
* Backend tests verify service and endpoint delete behavior.
* Startup seeding behavior remains unchanged.

## Definition of Done

* Backend exposes `DELETE /api/exercises/{id}`.
* `ExerciseService.DeleteExerciseAsync` returns `bool`.
* Successful deletion returns `204 No Content`.
* Missing deletion targets return `404 Not Found`.
* Deletion is persisted through EF Core.
* The admin exercise list provides a confirmed delete workflow.
* Pending and error states are visible and prevent duplicate actions.
* The admin list reflects successful deletion without a full browser reload.
* Existing exercise CRUD behavior remains intact.
* Backend tests pass.
* Frontend production build passes.
* Manual frontend verification passes.
* No authentication, bulk delete, soft delete or workout features are introduced.
* Startup seeding remains unchanged.

## Risks / Notes

* Hard deletion is irreversible.
* The `/admin` route and delete endpoint remain unprotected and must not be treated as secure administration functionality.
* Deleting all exercises causes seed data to return on the next backend startup.
* Future workout references will require the deletion policy to be reconsidered.
* A future foreign-key relationship may require `409 Conflict`, archive behavior or another lifecycle rule instead of unconditional hard deletion.
* A boolean service result should be replaced with a richer result type if deletion gains more than two expected outcomes.
