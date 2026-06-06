# ReceiptScout

> A receipt-tracking API with AI-assisted BAS account suggestion for Swedish small businesses.

## About

Small business owners in Sweden spend hours sorting receipts at year-end, mapping each one to the correct **BAS account** (the Swedish chart of accounts). ReceiptScout is a backend API with a planned React frontend that lets users register receipts, get AI-suggested categorization against the BAS standard, and bundle them into expense reports for approval.

Built as a portfolio project demonstrating Clean Architecture, secure-by-default API design, and a swappable AI provider interface for GDPR-sensitive deployments.

## Architecture

Four-layer **Clean Architecture** with the dependency rule enforced via project references:

```
Domain          ← Entities, enums, exceptions. No outward dependencies.
Application     ← Services, interfaces, DTOs, validators. Depends only on Domain.
Infrastructure  ← EF Core, Identity, JWT, AI provider. Implements Application interfaces.
Api             ← Controllers, middleware, composition root.
```

Business invariants live in **rich domain entities** — invalid states are unrepresentable. A `Receipt` with a negative amount or an `ExpenseReport` approved while still in `Draft` cannot be constructed.

The AI categorization service is abstracted behind `IAiCategorizationService` in the Application layer, so the provider can be swapped without touching domain or application code. Today a keyword-based stub implements the interface; the real Google Gemini client is planned, with a local LLM such as Ollama as a future option for handling sensitive customer data under GDPR. Switching providers is a single DI registration change.

## Tech Stack

| Layer | Tech |
|---|---|
| Runtime | .NET 10 |
| API | ASP.NET Core Web API (controllers + services) |
| Persistence | Entity Framework Core 10, SQL Server Express, code-first migrations |
| Auth | ASP.NET Core Identity (`AddIdentityCore` + roles), JWT bearer access tokens |
| Validation | FluentValidation (+ SharpGrip auto-validation) |
| API documentation | Swagger UI (Swashbuckle.AspNetCore) at `/swagger` |
| Health | ASP.NET Core health checks with EF Core DB probe at `/health` |
| Testing | xUnit, NSubstitute, Shouldly (13 tests) |
| AI | Provider behind `IAiCategorizationService` — stub today, Google Gemini planned |
| Frontend | React + Vite *(planned)* |

## Key Design Decisions

- **Rich domain model.** Entities validate their own invariants in the constructor; state changes go through methods like `ExpenseReport.Submit()` and `ExpenseReport.Approve(adminId)` that enforce the state machine. Business rules cannot be bypassed regardless of how an entity is created.

- **Defense in depth.** `FluentValidation` catches bad input early with user-friendly error responses (`ProblemDetails`, RFC 7807). Domain constructors are the last line of defense — even if validation is bypassed or buggy, the domain protects itself.

- **State enforcement in the entity, authorization in the service.** `ExpenseReport.Approve()` enforces *"is this state transition legal?"*; the service layer enforces *"does this user have permission?"* via owner-or-admin checks against the current user. Two distinct kinds of "can I do this?" kept cleanly separate.

- **Cascade-aware FK strategy.** User-owned financial records use `Restrict` on delete to avoid accidental data loss. Optional relationships (`Category`, `ExpenseReport` on `Receipt`) use `SetNull`. The double FK from `ExpenseReport` to `ApplicationUser` (owner + approver) is configured to avoid SQL Server's multiple-cascade-path error.

- **Swappable AI provider.** The Application layer depends only on `IAiCategorizationService`. The request is grounded with the user's existing categories so the model is constrained to known BAS accounts. The implementation lives in Infrastructure behind a single DI registration — swapping the stub for Gemini changes one line.

- **AI-ready, idempotent seeding.** On startup the app applies pending migrations and seeds the Admin/User roles, a bootstrap admin account, and the standard Swedish BAS expense categories — only inserting what is missing, so it is safe to run on every boot in every environment.

- **Generic + specific repositories.** Generic repository removes CRUD duplication; specific repositories add typed queries with `Include()` that the generic cannot express cleanly.

## Security

Defense-in-depth applied throughout:

- ASP.NET Core Identity with password policy (length + complexity) and account lockout
- Short-lived JWT access tokens (15 min)
- Rate limiting on `/api/Auth/*` endpoints (fixed window, 5 requests/min)
- Owner-or-admin authorization enforced in the service layer; admin-only endpoints for category management and report approval
- DTOs prevent mass-assignment / overposting
- Global exception handler returns `ProblemDetails` without leaking stack traces
- Security headers middleware (X-Content-Type-Options, X-Frame-Options, Referrer-Policy)
- HTTPS redirection + HSTS in production
- Secrets in `dotnet user-secrets` (dev) and environment variables (prod) — never in `appsettings.json`

## Status

🚧 **In active development.**

- ✅ Domain layer (entities, invariants, state machine, tests)
- ✅ Persistence (DbContext, configurations, cascade strategy, migrations)
- ✅ Repositories, current user service, layered DI
- ✅ Application services, DTOs, FluentValidation
- ✅ Controllers, JWT authentication, role-based authorization
- ✅ Security hardening (rate limiting, headers, global exception handler, CORS, HSTS)
- ✅ Startup seeding (admin + BAS categories) and `/health` endpoint
- ✅ AI categorization seam (`IAiCategorizationService`) with stub provider + endpoint
- ⬜ Google Gemini implementation of the AI seam
- ⬜ React frontend
- ⬜ CI/CD and deployment (GitHub Actions → Railway)

**Planned / not yet implemented:** refresh tokens, Gemini integration, frontend, CI/CD.

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server Express (or LocalDB) — the default connection string targets `.\SQLEXPRESS`

### Setup

All commands run from the `api/` folder.

```bash
# 1. Set the JWT signing key (dev secret, never committed)
dotnet user-secrets set "Jwt:Key" "<a-long-random-secret-at-least-32-chars>" --project src/ReceiptScout.Api

# 2. Run the API — migrations are applied and the database is seeded automatically
dotnet run --project src/ReceiptScout.Api
```

Swagger UI opens at `https://localhost:7161/swagger`. The health endpoint is at `https://localhost:7161/health`.

### Seeded admin

On first run a bootstrap admin is created so you can log in immediately — no manual role assignment needed:

| Field | Value |
|---|---|
| Email | `admin@receiptscout.local` |
| Password | `Admin123!` |

Override these in production with the `Seed__AdminEmail` and `Seed__AdminPassword` environment variables. Users who register through `/api/Auth/register` receive the **User** role.

### Tests

```bash
dotnet test   # 13 passing
```

## License

TBD
