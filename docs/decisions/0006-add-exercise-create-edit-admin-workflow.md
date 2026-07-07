# 0006 - Add Exercise Create and Edit Admin Workflow

## Status

Accepted

## Context

TrainMateX-Pro now has a read-only exercise library and PostgreSQL-backed exercise persistence.

The project can display exercise data from the database, but exercise data still has to be maintained through seed data or direct database changes. As the application moves toward workout and program features, exercises need to become manageable through the application itself.

The project should add write workflows without jumping too quickly into destructive actions, authentication, coach/client roles or workout modeling.

## Decision

The next product slice will add create and edit workflows for exercises through admin routes.

The slice will include:

- `POST /api/exercises`
- `PUT /api/exercises/{id}`
- admin frontend routes for listing, creating and editing exercises
- backend-generated slug IDs on create
- stable exercise IDs on edit
- compact enum validation for muscle group, equipment and difficulty

The slice will not include delete, authentication, authorization or workout/program features.

The `/admin` route segment will be used only to organize management screens. It does not imply security until authentication and authorization are added in a later slice.

## Reasoning

Create and edit workflows are the next useful step after introducing database persistence. They turn the exercise library from seeded read-only data into manageable product data.

Deferring delete keeps the slice focused and avoids destructive workflow questions such as confirmation UX, recovery, archiving and references from future workout plans.

Admin routes separate management workflows from the public exercise library without requiring the project to introduce authentication too early.

Backend-generated slugs keep exercise URLs readable and avoid exposing technical ID entry in the form. This matches the current seeded ID style, such as `bench-press`.

Keeping IDs stable when editing names avoids broken links and prepares the project for future relationships where workouts or programs may reference exercises by ID.

Compact enum validation gives the first write workflow useful guardrails without creating a large taxonomy problem too early.

## Alternatives Considered

| Alternative | Reason Not Selected |
|---|---|
| Full CRUD including delete | More complete, but introduces destructive workflow complexity before the project has references, archive rules or authentication. |
| Create only | Smaller, but less useful because mistakes or outdated exercises could not be corrected through the UI. |
| Workout builder as the next slice | Important later, but workout and program features depend on having manageable exercise data first. |
| Authentication before admin routes | More correct for real admin security, but too large for the next slice and would delay the exercise management workflow. |
| User-entered exercise IDs | Simple to implement, but exposes a technical concern in the UI and creates more room for inconsistent IDs. |
| GUID exercise IDs | Avoids slug collisions, but produces less readable URLs and does not match the current seeded ID style. |
| Regenerate IDs when names change | Keeps URLs aligned with names, but can break existing links and future relationships. |

## Consequences

### Positive

- The project gains its first real write workflow.
- Exercise data can be maintained through the application.
- The slice builds practical experience with forms, validation, POST and PUT endpoints.
- The public exercise library can remain focused on browsing.
- The project becomes better prepared for workout and program features.

### Trade-offs

- `/admin` routes are not protected until authentication and authorization are added later.
- Delete remains unavailable, so incorrect exercises must be edited rather than removed.
- Compact enum values may need to expand as the exercise library grows.
- Stable IDs mean edited exercise names may no longer match their original slug exactly.
- Create/edit validation adds more API and UI complexity than the read-only slices.

## Notes

- `POST /api/exercises` should return `201 Created` for successful creates.
- `PUT /api/exercises/{id}` should return `200 OK` for successful edits.
- Duplicate generated slugs should return `409 Conflict`.
- Missing edit targets should return `404 Not Found`.
- Validation errors should use a ProblemDetails-style response.
- Real admin security should be handled in a later authentication/authorization slice.

## Revisit Criteria

This decision should be reconsidered if:

- Admin routes need real protection before any management UI is exposed.
- Exercise delete or archive behavior becomes necessary.
- Workout or program references require stronger exercise lifecycle rules.
- The compact enum values become too restrictive.
- The project changes direction away from coach/admin-managed exercise data.

