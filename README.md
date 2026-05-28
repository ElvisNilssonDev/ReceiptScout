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
Infrastructure  ← EF Core, Identity, JWT, AI client. Implements Application interfaces.
Api             ← Controllers, middleware, composition root.
```

Business invariants live in **rich domain entities** — invalid states are unrepresentable. A `Receipt` with a negative amount or an `ExpenseReport` approved while still in `Draft` cannot be constructed.

The AI categorization service is abstracted behind `IAiCategorizationService` in the Application layer, so the provider (Google Gemini) can be swapped for a local LLM such as Ollama with a single DI line — without touching domain or application code. This matters for handling sensitive customer data under GDPR.

## Tech Stack

| Layer | Tech |
|---|---|
| Runtime | .NET 10 |
| API | ASP.NET Core Web API (controllers + services) |
| Persistence | Entity Framework Core 10, SQL Server Express, code-first migrations |
| Auth | ASP.NET Core Identity, JWT bearer + refresh tokens |
| Validation | FluentValidation |
| API documentation | Scalar (modern alternative to Swagger UI) |
| Testing | xUnit, NSubstitute, Shouldly |
| Frontend | React + Vite *(in progress)* |
| AI | Google Gemini API, swappable via interface *(in progress)* |

## Key Design Decisions

- **Rich domain model.** Entities validate their own invariants in the constructor; navigation through state changes happens via methods like `ExpenseReport.Submit()` and `ExpenseReport.Approve(adminId)` that enforce the state machine. Business rules cannot be bypassed regardless of how an entity is created.

- **Defense in depth.** `FluentValidation` catches bad input early with user-friendly error responses (`ProblemDetails`, RFC 7807). Domain constructors are the last line of defense — even if validation is bypassed or buggy, the domain protects itself.

- **State enforcement in the entity, authorization in the service.** `ExpenseReport.Approve()` enforces *"is this state transition legal?"*; the service layer enforces *"does this user have permission?"* via resource-based authorization handlers. Two distinct kinds of "can I do this?" kept cleanly separate.

- **Cascade-aware FK strategy.** User-owned financial records use `Restrict` on delete to avoid accidental data loss. Optional relationships (`Category`, `ExpenseReport` on `Receipt`) use `SetNull`. The double FK from `ExpenseReport` to `ApplicationUser` (owner + approver) is configured to avoid SQL Server's multiple-cascade-path error.

- **Swappable AI provider.** The Application layer depends only on `IAiCategorizationService`. The Gemini implementation lives in Infrastructure with a single DI registration. Switching providers is a one-line change.

- **Generic + specific repositories.** Generic repository removes CRUD duplication; specific repositories add typed queries with `Include()` that the generic cannot express cleanly.

## Security

Defense-in-depth applied throughout:

- ASP.NET Core Identity with strict password policy and lockout
- JWT access tokens (short-lived) + refresh tokens in `HttpOnly; Secure; SameSite=Strict` cookies
- Rate limiting on `/auth/*` endpoints
- Resource-based authorization (owner-or-admin) via `IAuthorizationHandler`
- DTOs prevent mass-assignment / overposting
- Global exception handler returns `ProblemDetails` without leaking stack traces
- Security headers middleware (X-Content-Type-Options, X-Frame-Options, Referrer-Policy)
- HTTPS-only + HSTS in production
- Secrets in `dotnet user-secrets` (dev) and environment variables (prod) — never in `appsettings.json`

## Status

🚧 **In active development.**

- ✅ Domain layer (entities, invariants, state machine, tests)
- ✅ Persistence (DbContext, configurations, cascade strategy, initial migration)
- ✅ Repositories, current user service, layered DI
- 🔨 Application services and DTOs *(in progress)*
- ⬜ Controllers, authentication, authorization
- ⬜ Security hardening (rate limiting, headers, exception handler)
- ⬜ React frontend
- ⬜ CI/CD and deployment
- ⬜ Gemini AI integration

## Getting Started

Setup, run, and deployment instructions will be added once the API reaches a runnable feature-complete state.

## License

TBD
