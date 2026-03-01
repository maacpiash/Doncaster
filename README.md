# Todo App (ASP.NET 10 web API + Angular frontend)

## Overview

This is a **TODO list application** demonstrating modern .NET 10 architecture with:

- **Clean Architecture**: Domain, Application, Infrastructure, WebAPI
- **Minimal API** in ASP.NET 10
- **Mediator pattern ([Mediator](https://github.com/martinothamar/Mediator))** for commands/queries
- **Modern, responsive frontend** with Angular and DaisyUI
- **Aspire orchestration** (`AppHost`)
- **Observability/telemetry** through a reusable `ServiceDefaults` project
- **Testable design**: unit tests for Application, integration tests for WebAPI

The app allows users to:

- View TODO items
- Add TODO items
- Delete TODO items
- Toggle TODO items

Data is stored **in-memory** for this assignment, but the structure is ready to support a database in the future.

## How to run

The project can be run in development mode or production mode.

### Development mode

In the root directory (where Todo.slnx file is), run the following command:

```sh
dotnet restore
```

Then, run

```sh
dotnet run --project src/AppHost
```

### Production mode

First, generate SSL certificates:

```sh
./generate-certs.sh
```

Then, duplicate the env file:

```sh
cp .env.example .env
```

Finally, run the Docker Compose command:

```sh
docker compose -f docker-compose.yml -f docker-compose.local.yml up --build -d
```

The frontend should be reachable at https://localhost.

The `--build` flag is only needed the first time this command is run, or when the code has been changed since the last time the command was run with this flag.

To turn everything off:

```sh
docker compose down
```

## Architecture & Design

The project follows Clean Architecture with separate layers for domain, application, infrastructure, and presentation. Inside application layer, the commands, queries, and handlers are divided by feature slices.

### Layers and Responsibilities

- **Domain**: Core entities (`TodoItem`) and exceptions
- **Application**: Commands, Queries, Handlers, and Interfaces (e.g., `ITodoRepository`)
- **Infrastructure**: Concrete implementations of Application interfaces (`InMemoryTodoRepository`)
- **WebAPI (Presentation)**: Minimal API endpoints calling MediatR handlers

### Projects related to orchestration

- **ServiceDefaults**: A cross-cutting DI extensions: logging, metrics, OpenTelemetry. It is a library only, not executable.
- **AppHost**: This project orchestrates the application runtime and orchestrates logging, metrics, and telemetry via ServiceDefaults. It is ready for scaling to databases, multiple services, or background jobs.

### Dependency Flow

```
AppHost -> WebAPI
WebAPI -> Application + Infrastructure + ServiceDefaults
Infrastructure -> Application
Application -> Domain
```

## Production

The production build includes:

- PostgreSQL and Entity Framework Core for persistence
- Docker and Docker Compose for orchestration
- YARP for reverse proxy
- Grafana, OTel Collector, Prometheus, Loki, and Tempo for telemetry (logs, metrics, traces etc.)

## Testing

- **Unit tests:** `tests/Application.UnitTests`: Test Application handlers (AddTodo, DeleteTodo, GetTodos)
- **Integration tests:** `tests/WebAPI.IntegrationTests`: Test Minimal API endpoints with `WebApplicationFactory`

To run the tests, run this command from the project root:

```sh
dotnet test
```

## Next Steps / Extensibility

- Swap in **EF Core / SQL Server** by implementing `ITodoRepository` in Infrastructure and wiring DI in AppHost.
- Add a gateway such as YARP for a reverse proxy to serve the frontend as static assets.
- Expand AppHost orchestration to include background jobs or multiple APIs.
- Add authentication and authorisation.
