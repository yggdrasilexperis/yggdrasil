# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## How you work with this team

Three people share this repository and this file. It is the team's rulebook, and
consistency between us matters more than any individual preference.

**Never create, edit, or delete a file unless the user asked for it in that
message.** Being asked to "add an endpoint", "look at this", or "how do I…" is a
request for an answer, not for a commit. The default deliverable is:

1. the code, in the chat, in a fenced block,
2. the exact path it belongs at and *why that project* (see the layering rules
   below),
3. a precise explanation of what it does — enough that the person pasting it
   could have written it themselves and can defend it in review,
4. anything that must happen afterwards: a migration, a DI registration, a
   `dotnet tool restore`, a secret.

If the user does ask you to write to disk — "apply it", "edit `QuizService.cs`",
"go ahead" — then do it without arguing. Permission is per request, not
permanent: finishing an approved edit does not authorise the next one.

Do not silently widen scope. If a request needs a change in another layer to
compile, say so and show that change too, but do not restructure code nobody
asked about. If something in the codebase is wrong — a layering violation, an
entity leaking through an endpoint, a hand-edited migration — point it out
plainly and let the user decide.

Prefer teaching over guessing. If a request is ambiguous (which project, which
contract, which status code), state the assumption in one line and carry on
rather than blocking.

## Commands

Run everything from the repository root.

```bash
dotnet build backend/Yggdrasil.sln
dotnet test backend/Yggdrasil.sln                    # unit + integration
dotnet test backend/Yggdrasil.Tests.Unit             # fast: mocks only, no Docker
dotnet format backend/Yggdrasil.sln                  # CI runs this with --verify-no-changes
dotnet run --project backend/Yggdrasil.Api
```

A single test class or a single test (xUnit + VSTest filters):

```bash
dotnet test backend/Yggdrasil.Tests.Unit --filter "FullyQualifiedName~QuizServiceTests"
dotnet test backend/Yggdrasil.Tests.Unit --filter "FullyQualifiedName~QuizServiceTests.UpdateAsync_WhenCallerIsNotOwner_Throws"
```

EF Core — `dotnet-ef` is pinned in `dotnet-tools.json`, so restore it once:

```bash
dotnet tool restore
dotnet ef migrations add <Name> --project backend/Yggdrasil.Infrastructure --startup-project backend/Yggdrasil.Api
dotnet ef database update --project backend/Yggdrasil.Infrastructure --startup-project backend/Yggdrasil.Api
```

The SDK is pinned by `global.json` (10.0.110, `net10.0`).
`Yggdrasil.Tests.Integration` starts a real PostgreSQL through Testcontainers,
so **Docker must be running** or those tests fail locally. CI
(`.github/workflows/ci.yml`) runs formatting, build, and the full test suite on
every PR into `main`.

## Architecture

```
Yggdrasil.Api ──────► Yggdrasil.Application ──────► Yggdrasil.Domain
      │                        ▲                            ▲
      └──► Yggdrasil.Infrastructure ──────────────────────  ┘
```

Arrows point at what a project may depend on. Domain depends on nothing.
Application depends only on Domain. Infrastructure implements interfaces that
Application declares. Api references Infrastructure for exactly one reason — so
`Program.cs` can register implementations in DI; nothing else in Api may name an
Infrastructure type.

**A reference that wants to point the other way means the code is in the wrong
project.** Move the code, do not add the reference.

| Writing… | Goes in |
|---|---|
| Entity, business enum | `Domain/Entities`, `Domain/Enums` |
| Request/response record | `Application/Contracts/<Feature>/` |
| Business rule, ownership check, orchestration | `Application/Services` |
| Interface for something the outside world does | `Application/Abstractions` |
| FluentValidation validator | `Application/Validation` |
| Anything touching the database | `Infrastructure/Persistence`, `Infrastructure/Repositories` |
| EF migration | `Infrastructure/Migrations` (generated, never hand-written) |
| Passwords, tokens, Identity | `Infrastructure/Identity` |
| HTTP route | `Api/Endpoints` |
| DI wiring, auth setup, CORS | `Api/Extensions` |
| Test with a mock | `Tests.Unit` |
| Test against real Postgres | `Tests.Integration` |

A feature always lands in this order: **Domain** (entity, if one is needed) →
**Application** (contracts, abstraction, service, validator — this is where the
feature actually lives) → **Infrastructure** (repository, EF configuration,
migration) → **Api** (endpoint: take request, call service, return result).

Each project has its own `README.md` with the detail; `backend/README.md` is the
overview. Read the relevant one before proposing code for that layer.

## Rules that get code sent back in review

**Api** — Minimal APIs, never controllers. One `XEndpoints` static class per
resource, exposing one `MapXEndpoints` extension that builds a route group.
Handlers are about three lines: request in, service call, result out; a handler
containing an `if` about a business rule belongs in a service. Use
`TypedResults`, not `Results`. Every async handler takes a `CancellationToken`
and passes it down. **Never return an entity** — contracts only; that is how
password hashes leak. Request validation goes through `ValidationFilter<T>`, not
inline in the handler.

**Application** — No `DbContext`, `DbSet`, LINQ-to-Entities, `HttpContext`,
`IResult`, status codes, or route strings. Contracts are `record` types. If a
class here cannot be unit-tested without a database or a web host, the
dependency belongs behind an interface in `Abstractions`.

**Domain** — No `[Required]`, `[MaxLength]`, or other data annotations; request
validation is Application's job and table shape is EF fluent configuration in
Infrastructure. No `ApplicationUser` — it inherits from ASP.NET Core Identity
and lives in `Infrastructure/Identity`; entities refer to a user as a plain
`Guid OwnerId` with no navigation property. That is deliberate.

**Infrastructure** — Migrations are append-only: once pushed, never edit one,
add a new one. Generate them with `dotnet ef`, never by hand. Configuration
comes from `IConfiguration`; no connection string is ever written into a file
here. Keep Npgsql-specific types from leaking through repository interfaces.

**Tests** — Name them `Method_Scenario_ExpectedResult`, so a CI failure explains
itself. Unit tests cover decisions (ownership, validation rules) with
NSubstitute mocks and Shouldly assertions. Anything whose behaviour depends on
EF actually running — cascades, unique constraints, query translation — belongs
in `Tests.Integration`, asserted through status codes and response bodies the
way a client sees them, never by reaching into the `DbContext`.

**Naming** — `PascalCase` types/methods/properties, `camelCase` locals,
`_camelCase` private fields, `I`-prefixed interfaces. One class per file, file
named after the class. Formatting comes from `.editorconfig`, enforced by
`dotnet format`.

**Secrets** — `appsettings.json` holds non-secret defaults only. The JWT signing
key and the connection string come from `dotnet user-secrets` locally and
environment variables everywhere else. Keep `.env.example` current when a new
setting is added.

## Git and pull requests

Branch off `main`, one branch per issue: `feat/`, `fix/`, `chore/`, `docs/`.
Never push to `main`. Keep commits small and let the message explain **why**
("Return 400 instead of 500 when an email is already registered", not "fix
bug"). Fill in the PR template, link the issue with `Closes #12`, set yourself
as assignee and someone else as reviewer.

When asked for a commit message or a PR body, write it in the chat. Do not run
`git commit`, `git push`, or open a PR unless explicitly told to.

## Current state

The solution is still largely scaffolding: `Class1.cs` placeholders in
Application, Domain, and Infrastructure; `Program.cs` is the untouched weather
forecast template; both test projects hold only `UnitTest1.cs`. The directories
in the table above and in the project READMEs describe the **target** structure
— most do not exist yet, so create them as features land rather than assuming
they are there. `backend/README.md` links to `docs/example-slice-auth.md`, which
has not been written yet.
