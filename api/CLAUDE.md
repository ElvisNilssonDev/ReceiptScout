# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All commands run from `api/`.

```bash
# Build
dotnet build

# Run the API (http: localhost:5251, https: localhost:7161)
dotnet run --project src/ReceiptScout.Api

# Run all tests
dotnet test

# Run a single test class or method
dotnet test tests/ReceiptScout.Application.Tests --filter "FullyQualifiedName~<TestClassName>"

# EF Core migrations (run from api/)
dotnet ef migrations add <Name> --project src/ReceiptScout.Infrastructure --startup-project src/ReceiptScout.Api
dotnet ef database update --project src/ReceiptScout.Infrastructure --startup-project src/ReceiptScout.Api
```

## Architecture

Clean Architecture with four layers. Dependency rule: inner layers never reference outer ones.

```
Domain → Application → Infrastructure
                  ↘              ↘
                        Api (wires everything)
```

**Domain** (`src/ReceiptScout.Domain`) — no dependencies. Entities, value objects, domain interfaces.

**Application** (`src/ReceiptScout.Application`) — depends on Domain. Use cases, commands/queries, FluentValidation validators, and interfaces that Infrastructure implements. DI registration via `Microsoft.Extensions.DependencyInjection.Abstractions`.

**Infrastructure** (`src/ReceiptScout.Infrastructure`) — depends on Domain + Application. EF Core with SQL Server (`AppDbContext`), ASP.NET Identity, repository implementations, and `HttpClient`-based external services. Implements interfaces defined in Application.

**Api** (`src/ReceiptScout.Api`) — depends on Application + Infrastructure. Minimal API / controller entry point. Registers services from both Application and Infrastructure. JWT Bearer auth and OpenAPI via Scalar (available at `/scalar` in Development).

**Tests** (`tests/ReceiptScout.Application.Tests`) — xUnit, NSubstitute for mocking, Shouldly for assertions, EF Core InMemory for persistence. Tests target the Application layer only; Infrastructure is substituted.
