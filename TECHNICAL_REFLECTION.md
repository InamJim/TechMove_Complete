# TechMove GLMS — Technical Reflection Report

## 1. Automated Testing in CI/CD Pipelines

### Why Automated Testing Is Critical

In a CI/CD (Continuous Integration / Continuous Deployment) pipeline, code changes are merged and deployed multiple times a day. Without automated tests, every deployment is a gamble. Automated testing acts as the safety net that catches breaking changes *before* they ever reach production.

**The gate mechanism:** When a developer pushes code, the pipeline triggers a test run. If any test fails, the pipeline halts and the deployment is blocked — automatically, instantly, and without human intervention. This means a developer who accidentally breaks the contract creation endpoint at 11 PM is caught before anyone's customer sees a 500 error at 8 AM.

**Prevents regressions:** A regression is when a working feature breaks due to an unrelated change elsewhere. Integration tests like `CreateThenRead_DataIntegrity_ContractAppearsInList` verify the entire chain — HTTP → Controller → Service → Repository → Database — after every single commit. A developer who refactors the `ContractService` but breaks JSON serialisation will see it immediately.

**Documents expected behaviour:** Each test method name is a living specification:
- `GetContracts_WithoutToken_Returns401Unauthorized` — security requirement
- `UpdateContractStatus_WithInvalidStatus_Returns400` — validation requirement
- `ActiveContracts_NotGreaterThanTotal` — business rule

These tests are far more reliable than written documentation because they execute and verify themselves.

**Speed of feedback:** A CI runner executing 20 integration tests takes roughly 10 seconds. A manual tester running the same checks takes 20 minutes. At 10 deployments per day, automation saves over 3 hours of human time — and is more thorough.

---

## 2. Docker and the "Works on My Machine" Problem

### The Root Cause

Before containers, applications were deployed onto servers that had been configured by hand over months or years. Machine A (development laptop) might have .NET 8.0.5 and SQL Server 2019. Machine B (QA server) might have .NET 8.0.1 and SQL Server 2022. Machine C (production) might have an environment variable named `CONNECTION_STRING` while the developer assumed `DefaultConnection`. The result: software that works perfectly on one machine silently fails on another.

### How Docker Solves It

Docker packages the application alongside its *entire runtime environment* into an image. The `FROM mcr.microsoft.com/dotnet/aspnet:8.0` base image guarantees the exact same .NET runtime version on every machine that runs it.

The TechMove solution uses a **multi-stage build pattern**:

```
Stage 1 (sdk:8.0)      — Compiles and publishes the application
Stage 2 (aspnet:8.0)   — Runs only the compiled output (smaller, more secure)
```

This means the final image contains no compiler toolchain, only the minimal runtime needed.

### Docker Compose — Environment Parity

The `docker-compose.yml` defines the entire TechMove ecosystem declaratively:

| Container | Role |
|---|---|
| `sql-server-db` | SQL Server 2022, same version everywhere |
| `glms-backend-api` | Web API, configured via environment variables |
| `glms-frontend-web` | MVC App, points to API by Docker service name |

**Environment variables** replace all hard-coded connection strings. The MVC frontend finds the API at `http://glms-backend-api:8080/` — a Docker internal DNS name that resolves automatically within the `techmove-net` bridge network. This is identical in development, QA, and production.

**Health checks** ensure the API container waits for SQL Server to be ready before attempting database migrations — eliminating the race condition that causes containers to crash on first boot.

**Result:** A developer cloning the repository and running `docker compose up` gets a fully working, three-tier application in under two minutes. No SQL Server installation. No .NET SDK setup. No manual configuration. The environment is defined in code, version-controlled, and identical everywhere.

---

## 3. Architecture Summary

The TechMove GLMS follows a strict **three-tier separation**:

```
┌─────────────────────┐      HTTP/JSON      ┌──────────────────────┐
│  MVC Frontend       │ ──────────────────► │  Web API Backend     │
│  (Presentation)     │   JWT Auth Bearer   │  (Business Logic)    │
│  ContractController │                     │  ContractsController │
│  HomeController     │                     │  AuthController      │
│  ServiceRequest...  │                     │  DashboardController │
└─────────────────────┘                     └────────┬─────────────┘
                                                     │
                                           Repository Pattern
                                                     │
                                            ┌────────▼─────────────┐
                                            │  SQL Server Database  │
                                            │  (Data Layer)         │
                                            └──────────────────────┘
```

**No database access in the MVC project.** All persistence goes through the API, which enforces authentication and business rules centrally. This means the presentation layer can be replaced (e.g., a mobile app) without touching business logic.

**JWT authentication** ensures every API call is authorised. The MVC app obtains a token on startup and attaches it as a `Bearer` header to every request. Unauthenticated requests receive `401 Unauthorized` — verified by the integration test suite.
