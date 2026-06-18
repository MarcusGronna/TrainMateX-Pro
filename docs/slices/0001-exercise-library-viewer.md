# Slice 1 - Exercise Library Viewer

## Goal

Build the first read-only fullstack slice for TrainMateX-Pro.

A client should be able to open an exercise library, view a list of exercises, select one exercise and read instructions for how it should be performed.

The main goal of this slice is to prove that the Next.js frontend can fetch and render exercise data from the ASP.NET Core backend.

## User Story

As a client,
I want to browse an exercise library and open an exercise detail page,
so that I can understand how an exercise should be performed.

## In Scope

* Display a list of exercises.
* Display basic exercise information in the list.
* Allow the client to select an exercise.
* Display a detail page for the selected exercise.
* Show exercise instructions on the detail page.
* Fetch exercise data from the ASP.NET Core API.
* Use seeded or hardcoded exercise data in the backend for the first version.
* Handle a missing exercise with a simple not-found response/state.

## Out of Scope

* Creating exercises.
* Editing exercises.
* Deleting exercises.
* Authentication.
* Client accounts.
* Coach/admin roles.
* Database persistence.
* Workout plans.
* Completed workout tracking.
* Media uploads.
* Advanced filtering.
* Search.
* Dashboards.
* Styling polish beyond a clean basic layout.

## User Flow

1. The client opens the exercise library page.
2. The frontend requests the exercise list from the backend API.
3. The client sees a list of available exercises.
4. The client selects one exercise.
5. The frontend opens the exercise detail route.
6. The frontend requests the selected exercise from the backend API.
7. The client sees instructions for how the exercise should be performed.

## API Contract

### Get all exercises

```http
GET /api/exercises
```

Expected response:

```json
[
  {
    "id": "bench-press",
    "name": "Bench Press",
    "muscleGroup": "Chest",
    "difficultyLevel": "Intermediate"
  }
]
```

### Get exercise by id

```http
GET /api/exercises/{id}
```

Expected response:

```json
{
  "id": "bench-press",
  "name": "Bench Press",
  "description": "A compound upper-body exercise performed with a barbell.",
  "instructions": [
    "Lie on the bench with your eyes under the bar.",
    "Grip the bar slightly wider than shoulder-width.",
    "Lower the bar under control to your chest.",
    "Press the bar upward until your arms are extended."
  ],
  "muscleGroup": "Chest",
  "equipment": "Barbell",
  "difficultyLevel": "Intermediate"
}
```

### Not found response

If the exercise id does not exist:

```http
404 Not Found
```

## Frontend Routes

```text
/exercises
/exercises/[id]
```

### `/exercises`

The exercise library page.

Responsibilities:

* Fetch exercise list from the backend.
* Render exercise cards or list items.
* Link each exercise to its detail page.

### `/exercises/[id]`

The exercise detail page.

Responsibilities:

* Fetch one exercise by id from the backend.
* Render name, description, instructions, muscle group, equipment and difficulty level.
* Show a reasonable not-found state if the exercise does not exist.

## Backend Tasks

* Add an Exercises feature area.
* Define an exercise model or DTO for list items.
* Define an exercise model or DTO for exercise details.
* Add seeded exercise data.
* Add `GET /api/exercises`.
* Add `GET /api/exercises/{id}`.
* Return `404 Not Found` when an exercise id does not exist.
* Keep the first version read-only.
* Avoid database persistence in this slice.

## Frontend Tasks

* Create the `/exercises` route.
* Create the `/exercises/[id]` route.
* Define TypeScript types for exercise list items and exercise details.
* Create an exercise list component.
* Create an exercise card or list item component.
* Create an exercise detail component.
* Fetch exercise list from the ASP.NET Core API.
* Fetch exercise details from the ASP.NET Core API.
* Add basic loading and error handling if needed.
* Add simple navigation back to the exercise library.

## Tests

### Backend tests

* `GET /api/exercises` returns `200 OK`.
* `GET /api/exercises` returns a list of exercises.
* `GET /api/exercises/{id}` returns `200 OK` for an existing exercise.
* `GET /api/exercises/{id}` returns the expected exercise details.
* `GET /api/exercises/{id}` returns `404 Not Found` for an unknown exercise id.

### Frontend tests

Frontend tests are not required for the first version of this slice.

Manual verification is enough for the first slice, but automated frontend tests can be added later when the UI structure stabilizes.

## Definition of Done

* Backend exposes `GET /api/exercises`.
* Backend exposes `GET /api/exercises/{id}`.
* Frontend has `/exercises` route.
* Frontend has `/exercises/[id]` route.
* Exercise list is fetched from ASP.NET Core API.
* Exercise detail is fetched from ASP.NET Core API.
* Unknown exercise id shows a reasonable not-found state.
* At least basic backend tests exist.
* README or slice doc explains how to run the slice.

