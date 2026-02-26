# Todo App (Clean Architecture + Minimal API + Aspire)

## Overview

This is a **TODO list application** demonstrating modern .NET 10 architecture
with:

- **Clean Architecture**: Domain, Application, Infrastructure, Presentation
- **Minimal API** in .NET 10
- **Mediator pattern (MediatR)** for commands/queries
- **Aspire orchestration** (`AppHost`)
- **Observability / telemetry** through a reusable `ServiceDefaults` project
- **Testable design**: unit tests for Application, integration tests for WebAPI

The app allows users to:

- View TODO items
- Add TODO items
- Delete TODO items

Data is stored **in-memory** for this assignment, but the structure is ready to
support a database in the future.

## Project Structure

```
.
├── Todo.slnx
├── src/
│ ├── Domain/
│ │ ├── Models/TodoItem.cs
│ │ └── Exceptions/TodoNotFoundException.cs
│ │
│ ├── Application/
│ │ ├── Commands/
│ │ │ ├── AddTodoCommand.cs
│ │ │ └── DeleteTodoCommand.cs
│ │ ├── Queries/GetTodosQuery.cs
│ │ └── Handlers/
│ │ │ ├── AddTodoHandler.cs
│ │ │ ├── DeleteTodoHandler.cs
│ │ | └── GetTodosHandler.cs
│ │ └── Interfaces/ITodoRepository.cs
│ │
│ ├── Infrastructure/
│ │ └── Persistence/InMemoryTodoRepository.cs
│ │
│ ├── WebAPI/
│ │ ├── Program.cs # Minimal API endpoints and DI for Application
│ │ ├── Todo.WebAPI.csproj
│ │ └── appsettings.json
│ │
│ ├── ServiceDefaults/
│ │ ├── Extensions.cs # Reusable DI extensions: logging, metrics, OpenTelemetry
│ │ └── Todo.ServiceDefaults.csproj
│ │
│ └── AppHost/
│ ├── Program.cs # Orchestrates WebAPI + ServiceDefaults + Infrastructure
│ ├── Todo.AppHost.csproj
│ └── appsettings.json
│
└── tests/
├── Application.UnitTests/
│ ├── Todo.Application.Tests.csproj
│ └── UnitTest1.cs # Unit tests for Application layer handlers
│
└── WebAPI.IntegrationTests/
├── Todo.Presentation.IntegrationTests.csproj
└── UnitTest1.cs # Integration tests for Minimal API endpoints
```

## Architecture & Design

### **Layers and Responsibilities**

- **Domain**
  - Core entities (`TodoItem`) and exceptions
  - No dependencies on other layers

- **Application**
  - Commands, Queries, Handlers
  - Interfaces (e.g., `ITodoRepository`)
  - References **Domain** only

- **Infrastructure**
  - Concrete implementations of Application interfaces
    (`InMemoryTodoRepository`)
  - References **Domain** + **Application**

- **WebAPI (Presentation)**
  - Minimal API endpoints calling MediatR handlers
  - References **Application** only
  - Does **not** reference Infrastructure directly (DI is wired in AppHost)

- **ServiceDefaults**
  - Cross-cutting DI extensions: logging, metrics, OpenTelemetry
  - Library only, no executable

- **AppHost (Aspire)**
  - Orchestrates the application runtime
  - Wires DI for Infrastructure implementations
  - Configures ServiceDefaults for observability
  - References WebAPI + ServiceDefaults + Infrastructure

### **Dependency Flow**

```
AppHost --> WebAPI + ServiceDefaults + Infrastructure (DI wiring)
WebAPI --> Application
Infrastructure --> Application + Domain
Application --> Domain
```

- **Arrows point inward**, following Clean Architecture principles.
- WebAPI **does not know about Infrastructure**; AppHost handles all concrete
  service wiring.

---

## Testing

- **Unit tests:** `tests/Application.UnitTests`
  - Test Application handlers (AddTodo, DeleteTodo, GetTodos)
  - No dependency on WebAPI or Infrastructure

- **Integration tests:** `tests/WebAPI.IntegrationTests`
  - Test Minimal API endpoints with `WebApplicationFactory`
  - Includes DI wiring from AppHost if needed

---

## Observability & Aspire

- **AppHost** orchestrates logging, metrics, and telemetry via
  **ServiceDefaults**.
- Provides a centralized, reusable approach for cross-cutting concerns.
- Ready for scaling to multiple services or background jobs.

---

## Next Steps / Extensibility

- Swap in **EF Core / SQL Server** by implementing `ITodoRepository` in
  Infrastructure and wiring DI in AppHost.
- Add **Angular frontend** in a separate folder (e.g., `frontend/`) served
  either via `ng build` static files or reverse proxy.
- Expand AppHost orchestration to include background jobs or multiple APIs.

---

**Senior-level signals in this solution:**

- Clean Architecture with decoupled layers
- Minimal API + MediatR + DI via AppHost
- Observability handled via reusable library (`ServiceDefaults`)
- Testable design (unit + integration tests)
- Forward-looking orchestration pattern suitable for multiple services
