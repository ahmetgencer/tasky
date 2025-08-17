![CI](https://github.com/ahmetgencer/tasky/actions/workflows/ci.yml/badge.svg)

# Tasky

**Full‑stack Task Manager** built with **.NET 9 (ASP.NET Core + EF Core 9 + MediatR)** and **React + Vite + Tailwind v4**. Clean architecture style with Domain/Application/Infrastructure/API layers, JWT auth, CQRS, Kanban drag & drop, and task comments.

---

## Elevator pitch

A pragmatic, interview‑ready project that demonstrates:

- **Layered architecture** (Domain / Application / Infrastructure / API, plus web client)
- **CQRS** with MediatR and FluentValidation
- **EF Core 9 + SQL Server** with repository abstraction
- **JWT authentication** (register / login / whoami) and current user context
- **Kanban board** (drag & drop between columns) and **task comments**
- **Developer experience**: Swagger, CORS, central error handling, Dockerized SQL

---

## Tech stack

- **Backend**: .NET 9, ASP.NET Core, EF Core 9, MediatR, FluentValidation
- **Auth**: JWT (HS256), BCrypt for password hashing
- **DB**: SQL Server 2022 (Docker)
- **Frontend**: React 18, Vite, TypeScript, Tailwind CSS v4, Zustand, axios

---

## Solution structure

```
Tasky.sln
 ├─ src/
 │   ├─ Tasky.Domain/            # Entities, enums, domain logic
 │   ├─ Tasky.Application/       # CQRS (commands/queries), DTOs, validators, abstractions
 │   ├─ Tasky.Infrastructure/    # EF Core (DbContext, repository), Security (JWT, hashing, CurrentUser)
 │   └─ Tasky.Api/               # Controllers, DI, Auth config, CORS, Swagger, middleware
 └─ web/
     └─ tasky-web/               # React (Vite + TS + Tailwind v4)
```

---

## Features

- **Auth**: `/api/auth/register`, `/api/auth/login`, `/api/auth/me`
- **Projects**: CRUD subset — create + list (scoped to current user)
- **Tasks**: create / list / update / delete; **status** = Todo/InProgress/Done
- **Kanban**: drag & drop between columns (status persisted); in-column order is client-side
- **Comments**: list/add/delete per task

---

## Getting started (dev)

### Prerequisites

- .NET SDK **9**
- Node **18+** (recommended 20)
- Docker Desktop (for SQL Server)

### 1) Start SQL Server in Docker

```bash
docker run -d --name tasky-mssql \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=P@ssw0rd!" \
  -p 1433:1433 \
  -v tasky_mssqldata:/var/opt/mssql \
  mcr.microsoft.com/mssql/server:2022-latest
```

### 2) Configure API secrets

Use a **32+ byte** JWT key (HS256):

```bash
cd src/Tasky.Api
dotnet user-secrets init
# Option A: raw text key (>=32 chars)
dotnet user-secrets set "Jwt:Key" "mysupersecretkeymysupersecretkey!!"
# Option B: base64 key (recommended)
# dotnet user-secrets set "Jwt:Key" "<BASE64_32_BYTES>"
# dotnet user-secrets set "Jwt:Issuer" "tasky"
# dotnet user-secrets set "Jwt:Audience" "tasky.web"
```

Make sure `appsettings.Development.json` has the connection string:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost,1433;Database=Tasky;User ID=sa;Password=P@ssw0rd!;Encrypt=True;TrustServerCertificate=True"
  }
}
```

### 3) Run the API

```bash
# from repo root
dotnet restore
# apply EF migrations
dotnet ef database update --project src/Tasky.Infrastructure --startup-project src/Tasky.Api
# run
dotnet run --project src/Tasky.Api
```

Swagger: `http://localhost:5033/swagger` (port may vary; match your output). If using HTTPS, trust dev certs:

```bash
dotnet dev-certs https --trust
```

### 4) Run the web client

```bash
cd web/tasky-web
echo "VITE_API_URL=http://localhost:5033" > .env
npm i
npm run dev
```

Visit `http://localhost:5173`.

---

## API quick tour

### Auth

- `POST /api/auth/register` → `{ email, password }` → `{ id }`
- `POST /api/auth/login` → `{ email, password }` → `{ token }`
- `GET  /api/auth/me` → Bearer token → `{ id, email }`

### Projects

- `POST /api/projects` → `{ name }` → `{ id }` (owner = current user)
- `GET  /api/projects` → list current user’s projects

### Tasks

- `POST   /api/tasks` → `{ projectId, title, status }`
- `GET    /api/tasks?projectId={id}` → list tasks in project
- `PUT    /api/tasks/{id}` → update task (incl. status)
- `DELETE /api/tasks/{id}` → delete task

### Comments

- `GET    /api/tasks/{taskId}/comments` → list comments for task
- `POST   /api/tasks/{taskId}/comments` → `{ content }` → created comment
- `DELETE /api/comments/{id}` → delete comment (author only)

### Sample `requests.http`

```http
### Register
POST http://localhost:5033/api/auth/register
Content-Type: application/json

{ "email": "demo@tasky.local", "password": "P@ssw0rd!" }

### Login
POST http://localhost:5033/api/auth/login
Content-Type: application/json

{ "email": "demo@tasky.local", "password": "P@ssw0rd!" }

### Get my profile
GET http://localhost:5033/api/auth/me
Authorization: Bearer {{token}}

### Create project
POST http://localhost:5033/api/projects
Authorization: Bearer {{token}}
Content-Type: application/json

{ "name": "Interview Demo" }

### List projects
GET http://localhost:5033/api/projects
Authorization: Bearer {{token}}

### Create task
POST http://localhost:5033/api/tasks
Authorization: Bearer {{token}}
Content-Type: application/json

{ "projectId": "{{projectId}}", "title": "First task", "status": 0 }
```

---

## Frontend quick tour

- **Login page** → saves JWT in Zustand (persisted)
- **Projects page** → list + create project (server uses
