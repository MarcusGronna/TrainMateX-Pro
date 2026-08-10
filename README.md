# TrainMateX-Pro

A fullstack fitness application for browsing and managing exercise libraries. Built with ASP.NET Core and Next.js as a learning project to strengthen .NET and React/Next.js skills.

## Tech Stack

| Layer    | Technology                        |
| -------- | --------------------------------- |
| Backend  | ASP.NET Core (.NET 10), C#       |
| Frontend | Next.js 16, React 19, TypeScript |
| Styling  | Tailwind CSS 4                   |

## Project Structure

```text
TrainMateX-Pro/
├── backend/
│   ├── src/TrainMateX.Api/       # ASP.NET Core Web API
│   └── tests/TrainMateX.Api.Tests/
├── frontend/                     # Next.js application
│   └── src/
│       ├── app/                  # App router pages
│       └── features/             # Feature modules
└── docs/
    ├── decisions/                # Architecture Decision Records
    └── slices/                   # Vertical slice specifications
```

## Current Features

### Exercise Library Viewer (Slice 1)

A read-only exercise library allowing users to browse exercises and view detailed instructions.

- `GET /api/exercises` — list all exercises
- `GET /api/exercises/{id}` — get exercise details
- `/exercises` — exercise library page
- `/exercises/[id]` — exercise detail page

### Exercise Persistence (Slice 2)

Exercise data is stored in PostgreSQL and accessed through Entity Framework Core while preserving the public read API contract.

- PostgreSQL development database through Docker Compose
- EF Core persistence and migrations
- Seeded exercise data for local development

### Exercise Administration (Slice 3)

An admin-style workflow for creating and editing exercises while keeping the public exercise library read-only.

- `POST /api/exercises` — create an exercise with a generated slug ID
- `PUT /api/exercises/{id}` — update an exercise while preserving its ID
- `/admin/exercises` — exercise management page
- `/admin/exercises/new` — create exercise page
- `/admin/exercises/[id]/edit` — edit exercise page
- Validation and duplicate-slug conflict handling

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS recommended)
- Docker Desktop or Docker Engine

### Backend

From the repository root, move into the backend folder:

```bash
cd backend
```

Create a local backend environment file:

```bash
cp .env.example .env
```

Start PostgreSQL:

```bash
docker compose up -d
```

Apply EF Core migrations:

```bash
dotnet ef database update --project src/TrainMateX.Api
```

Run the API:

```bash
dotnet run --project src/TrainMateX.Api
```

The API runs at `http://localhost:5193` for the default HTTP launch profile.

The local PostgreSQL connection string is configured in `backend/src/TrainMateX.Api/appsettings.Development.json`.
Docker Compose reads database container values from `backend/.env`.

Useful backend endpoints:

- `GET http://localhost:5193/api/exercises`
- `GET http://localhost:5193/api/exercises/{id}`
- `POST http://localhost:5193/api/exercises`
- `PUT http://localhost:5193/api/exercises/{id}`

### Frontend

```bash
cd frontend
npm install
npm run dev
```

The app runs at [http://localhost:3000](http://localhost:3000).

### Verification

```bash
# Backend tests
cd backend
dotnet test

# Frontend production build
cd frontend
npm run build
```

## Commit Messages

### type(scope): message

```text
- feat(frontend): add login form
- fix(backend): validate request body
- chore(root): update gitignore rules
```

### Types

```text
- feat:      new feature
- fix:       bug fix
- chore:     maintenance, config or tooling
- docs:      documentation only
- refactor:  code restructuring without behavior change
- test:      tests
- style:     formatting only
- build:     dependencies or build setup
- ci:        CI/CD workflows
```
