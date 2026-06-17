# TechMove GLMS - Global Logistics Management System

A three-tier ASP.NET Core 8 application with a REST API backend, MVC frontend, JWT authentication, integration tests, and full Docker orchestration.

---

## Architecture Overview

```
┌─────────────────────┐      HTTP/JWT       ┌──────────────────────┐      EF Core      ┌──────────────┐
│  MVC Frontend       │ ─────────────────►  │  Web API Backend     │ ────────────────►  │  SQL Server  │
│  :5000              │                     │  :5180               │                    │  :1433       │
└─────────────────────┘                     └──────────────────────┘                    └──────────────┘
```

**Zero database access** in the MVC project — all data operations flow through the Web API.

---

## Running with Docker Compose (Recommended)

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Start all containers
```bash
docker compose up --build
```

| Service | URL |
|---|---|
| MVC Frontend | http://localhost:5000 |
| Web API + Swagger | http://localhost:5180/swagger |
| SQL Server | localhost:1433 |

### Stop all containers
```bash
docker compose down
```

To also remove the database volume:
```bash
docker compose down -v
```

---

## Running Locally (Without Docker)

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)

### 1. Start the API
```bash
cd TechMove.API
dotnet run
# Swagger available at: http://localhost:5180/swagger
```

### 2. Start the MVC App
```bash
cd TechMove
dotnet run
# App available at: http://localhost:5237
```

---

## API Authentication

All API endpoints (except `/api/auth/login`) require a JWT Bearer token.

**Login credentials (demo):**
- Username: `admin`
- Password: `admin123`

**Get a token:**
```bash
curl -X POST http://localhost:5180/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'
```

**Use the token:**
```bash
curl http://localhost:5180/api/contracts \
  -H "Authorization: Bearer <your-token>"
```

---

## API Endpoints

| Method | Endpoint | Description |
| `POST` | `/api/auth/login` | Get JWT token |
| `GET` | `/api/contracts` | List contracts (filterable) |
| `GET` | `/api/contracts/{id}` | Get contract by ID |
| `POST` | `/api/contracts` | Create contract (multipart/form-data) |
| `PATCH` | `/api/contracts/{id}/status` | Update contract status |
| `DELETE` | `/api/contracts/{id}` | Delete contract |
| `GET` | `/api/clients` | List all clients |
| `GET` | `/api/clients/{id}` | Get client by ID |
| `GET` | `/api/servicerequests` | List all service requests |
| `POST` | `/api/servicerequests` | Create service request |
| `GET` | `/api/dashboard` | Dashboard statistics |

---

## Running Tests

```bash
cd TechMoveTests
dotnet test --verbosity normal
```

The integration tests use an **InMemory database** — no SQL Server required to run them.

Test coverage includes:
- **Auth:** Valid/invalid login, token issuance
- **Contracts:** GET all, GET by ID, POST create, PATCH status, DELETE, Create-then-Read data integrity
- **Clients:** GET all, GET by ID, 404 handling
- **Dashboard:** Statistics shape and business rules
- **Service Requests:** Create, Create-then-Read data integrity, auth guards

---

## Project Structure

```
TechMove-final/
├── TechMove/                    # MVC Frontend (no DB access)
│   ├── ApiServices/             # HttpClient wrappers for API calls
│   │   ├── ApiTokenService.cs   # JWT token management
│   │   ├── ContractApiService.cs
│   │   ├── ClientApiService.cs
│   │   ├── ServiceRequestApiService.cs
│   │   └── DashboardApiService.cs
│   ├── Controllers/             # MVC controllers (HttpClient only)
│   └── Dockerfile               # Multi-stage build
│
├── TechMove.API/                # REST API Backend
│   ├── Controllers/             # API endpoints
│   │   ├── AuthController.cs    # JWT login
│   │   ├── ContractsController.cs
│   │   ├── ClientsController.cs
│   │   ├── ServiceRequestsController.cs
│   │   └── DashboardController.cs
│   ├── DTOs/                    # Data transfer objects
│   └── Dockerfile               # Multi-stage build
│
├── TechMoveData/                # Entity models, DbContext, migrations
├── TechMoveRepo/                # Generic Repository pattern
├── TechMoveServices/            # Business logic services
│
├── TechMoveTests/               # Test suite
│   ├── IntegrationTests/        # API integration tests (InMemory DB)
│   ├── Currency/                # Unit tests
│   ├── Files/                   # Unit tests
│   └── Workflow/                # Unit tests
│
├── docker-compose.yml           # Orchestrates all 3 containers
├── TECHNICAL_REFLECTION.md      # DevOps & CI/CD report
└── README.md
```
