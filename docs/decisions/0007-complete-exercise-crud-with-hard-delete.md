# 0007 - Complete Exercise CRUD with Hard Delete

## Status

Accepted

## Context

TrainMateX-Pro has a PostgreSQL-backed exercise library with read, create and edit workflows.

Exercises can be maintained through the application, but obsolete exercises and temporary development data cannot be removed without direct database access. Workout and program entities do not yet exist, so exercises currently have no domain references that constrain deletion.

The project needs to decide whether to complete exercise CRUD now, how deletion should be represented in the service layer, and whether deletion should change the existing startup seeding behavior.

## Decision

The next product slice will add permanent exercise deletion through the admin-style workflow.

The decision includes:

- `DELETE /api/exercises/{id}`
- hard deletion through EF Core
- `204 No Content` when an exercise is deleted
- `404 Not Found` when the exercise does not exist
- a confirmed delete action on the admin exercise management page
- a `bool` return type from `ExerciseService.DeleteExerciseAsync`
- unchanged startup seeding behavior

Bulk deletion, soft deletion, archiving, authentication and workout-reference handling are not part of this decision.

## Reasoning

Completing exercise CRUD makes the existing administration workflow more useful and lets developers remove temporary or obsolete records without direct database access.

Hard deletion is acceptable at this stage because no workouts, programs or other persisted entities reference exercises. Implementing it before those relationships exist keeps the operation and its tests understandable. The deletion policy must be reconsidered once referential constraints or historical records are introduced.

The service method will return `bool` because there are currently only two expected outcomes: the exercise was deleted or it was not found. A dedicated result object or enum would add ceremony without representing additional information. The method can evolve to a richer result type later if references introduce a conflict outcome.

Successful deletion returns `204 No Content` because the operation succeeds without needing to return a representation. An unknown exercise returns `404 Not Found`, making the API outcome explicit to its administrative client.

Startup seeding remains unchanged to keep this slice focused on product CRUD behavior. The current seeder restores seed exercises only when the exercise table is completely empty at backend startup. This behavior is accepted even though it means the delete UI is not a permanent delete-all or database-reset mechanism.

## Alternatives Considered

| Alternative | Reason Not Selected |
|---|---|
| Build workout templates next | More product-domain value, but completing CRUD first makes exercise data easier to maintain before relationships constrain deletion. |
| Soft delete or archive exercises | Better for referenced or historical data, but adds lifecycle state, query filtering and UI rules before the domain requires them. |
| Add bulk delete or delete all | Convenient for cleanup, but increases destructive risk and turns the slice toward database administration tooling. |
| Return a dedicated delete result type | Extensible for future conflict outcomes, but unnecessary while only deleted and not-found outcomes exist. |
| Return the deleted exercise | Provides extra response data, but the client does not need it and `204 No Content` communicates the operation cleanly. |
| Treat missing deletion as successful | Makes DELETE idempotent at the HTTP outcome level, but gives the admin client less information about an incorrect or stale ID. |
| Change or disable startup seeding | Would permit a persistently empty table, but mixes data-initialization policy into the focused CRUD slice. |
| Add authentication first | Necessary before public deployment, but larger than this local learning slice and does not complete exercise management. |

## Consequences

### Positive

- Exercise management becomes full CRUD.
- Obsolete and temporary exercises can be removed through the application.
- The project gains practical experience with destructive HTTP operations and confirmation UX.
- The backend gains service and endpoint coverage for deletion.
- Deletion is introduced before relational references make the policy more complex.
- The boolean service contract remains small and explicit for the current outcomes.

### Trade-offs

- Hard deletion is irreversible.
- The delete endpoint remains unprotected until authentication and authorization are added.
- The service contract will need to evolve if future relationships introduce deletion conflicts.
- The delete UI is not a true database-reset mechanism.
- Deleting every exercise causes seed data to return when the backend next starts.
- Future workout or history data may require archive behavior instead of hard deletion.

## Notes

- Successful deletion should return `204 No Content` with an empty response body.
- Deleting an unknown exercise should return `404 Not Found`.
- The frontend should require explicit confirmation before sending the request.
- The admin list should refresh from the authoritative backend state after success.
- Existing startup seeding remains unchanged.
- No database migration is expected because the schema does not change.
- The unprotected endpoint must not be exposed as secure production administration functionality.

## Revisit Criteria

This decision should be reconsidered if:

- Workouts, programs, logs or other records reference exercises.
- Regulations or product requirements require recovery or audit history.
- Users need archive and restore workflows.
- Deletion gains an expected conflict or forbidden outcome.
- Startup seeding prevents a required development or deployment workflow.
- The application is deployed publicly and requires authentication and authorization.
