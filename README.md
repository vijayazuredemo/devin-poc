# Task Manager

A beginner-friendly full-stack app: a **.NET 8 Web API** backend with in-memory storage and a **React (Vite)** frontend.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (`dotnet --list-sdks` should show `8.0.x`)
- [Node.js 20+](https://nodejs.org/) (`node -v`)

## Run it

Use two terminals.

**Terminal 1: backend** (http://localhost:5211, Swagger UI at http://localhost:5211/swagger)

```bash
cd backend/TaskManager.Api
dotnet run --launch-profile http
```

**Terminal 2: frontend** (http://localhost:5173)

```bash
cd frontend
npm install      # first time only
npm run dev
```

Open http://localhost:5173. The Vite dev server forwards every `/api/...` request to the backend.

> Data is stored in memory, so it is reset every time the backend restarts.

## Test it

```bash
# Automated API integration tests (xUnit)
cd backend
dotnet test

# Frontend lint + production build
cd frontend
npm run lint
npm run build
```

### Manual API testing

- **Swagger UI:** open http://localhost:5211/swagger and use "Try it out" on each endpoint.
- **.http file:** open `backend/TaskManager.Api/TaskManager.Api.http` in VS Code (REST Client extension), Visual Studio, or Rider and click "Send Request".
- **curl:**

```bash
curl -i http://localhost:5211/api/tasks                                   # 200
curl -i -X POST http://localhost:5211/api/tasks \
  -H "Content-Type: application/json" \
  -d '{"title":"Learn Devin","description":"Build a task manager"}'      # 201
curl -i -X POST http://localhost:5211/api/tasks \
  -H "Content-Type: application/json" -d '{"title":""}'                  # 400
curl -i http://localhost:5211/api/tasks/1                                 # 200
curl -i -X PUT http://localhost:5211/api/tasks/1 \
  -H "Content-Type: application/json" \
  -d '{"title":"Learn Devin","description":"Done!","isCompleted":true}'  # 200
curl -i -X DELETE http://localhost:5211/api/tasks/1                       # 204
curl -i http://localhost:5211/api/tasks/1                                 # 404
```

## API

| Method | Route             | Success          | Errors                 |
|--------|-------------------|------------------|------------------------|
| GET    | `/api/tasks`      | 200 + list       |                        |
| GET    | `/api/tasks/{id}` | 200 + task       | 404                    |
| POST   | `/api/tasks`      | 201 + task       | 400 (validation)       |
| PUT    | `/api/tasks/{id}` | 200 + task       | 400 (validation), 404  |
| DELETE | `/api/tasks/{id}` | 204              | 404                    |

Validation rules: `title` is required (not blank) and at most 100 characters; `description` is optional and at most 500 characters.

## Project structure

```
TaskManager/
├── backend/
│   ├── TaskManager.sln
│   ├── TaskManager.Api/
│   │   ├── Program.cs
│   │   ├── Models/TaskItem.cs
│   │   ├── Dtos/CreateTaskRequest.cs
│   │   ├── Dtos/UpdateTaskRequest.cs
│   │   ├── Repositories/ITaskRepository.cs
│   │   ├── Repositories/InMemoryTaskRepository.cs
│   │   ├── Controllers/TasksController.cs
│   │   ├── Properties/launchSettings.json
│   │   └── TaskManager.Api.http
│   └── TaskManager.Api.Tests/
│       └── TasksApiTests.cs
└── frontend/
    ├── vite.config.js
    ├── index.html
    └── src/
        ├── main.jsx
        ├── App.jsx
        ├── index.css
        ├── api/tasksApi.js
        └── components/
            ├── TaskForm.jsx
            ├── TaskList.jsx
            └── TaskRow.jsx
```

### What each file does

**Backend**

- `Program.cs`: the app's entry point. It registers services for dependency injection (controllers, Swagger, and the repository as a **singleton** so every request shares one task list), then sets up the HTTP pipeline.
- `Models/TaskItem.cs`: the domain model (`Id`, `Title`, `Description`, `IsCompleted`, `CreatedAt`).
- `Dtos/*Request.cs`: what clients are allowed to send. Keeping these separate from the model stops clients from setting `Id` or `CreatedAt`, and holds the validation attributes (`[Required]`, `[StringLength]`).
- `Repositories/ITaskRepository.cs`: the data-access contract. The controller depends on this interface, so storage can later be swapped (for example, to a database) without changing the controller.
- `Repositories/InMemoryTaskRepository.cs`: a thread-safe `ConcurrentDictionary` implementation that assigns IDs with `Interlocked.Increment`.
- `Controllers/TasksController.cs`: the five CRUD endpoints. `[ApiController]` automatically returns **400** with error details when validation fails.
- `Properties/launchSettings.json`: sets the local URL (`http://localhost:5211`) used by `dotnet run`.
- `TaskManager.Api.http`: ready-made requests for testing the API by hand.

**Tests**

- `TasksApiTests.cs`: integration tests that use `WebApplicationFactory<Program>` to start the real API in memory and call every endpoint over HTTP, checking the status codes and response bodies. Each test gets a fresh app, so tests don't affect each other.

**Frontend**

- `vite.config.js`: Vite configuration. Its `server.proxy` forwards `/api` to the backend, so no CORS setup is needed during development.
- `src/main.jsx`: mounts `<App />` into `index.html`.
- `src/App.jsx`: holds the app's state (`tasks`, `isLoading`, `error`), loads tasks on startup, and passes add/update/delete handlers down to child components.
- `src/api/tasksApi.js`: every `fetch` call in one place. It turns API validation errors into readable messages.
- `src/components/TaskForm.jsx`: the "add task" form.
- `src/components/TaskList.jsx`: renders the list, or an empty-state message.
- `src/components/TaskRow.jsx`: one task with a complete checkbox, Edit (inline Save/Cancel), and Delete.
- `src/index.css`: simple styling.

## Not included (yet)

Authentication, Docker, Azure deployment, and a real database are intentionally out of scope for this first version.
