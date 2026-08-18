# Backend

Six projects. The split exists so that the rules of the application can be
tested without starting a web server or a database.

```
Yggdrasil.Api ──────► Yggdrasil.Application ──────► Yggdrasil.Domain
      │                        ▲                            ▲
      └──► Yggdrasil.Infrastructure ──────────────────────  ┘
```

Read that as: **arrows point at things you are allowed to depend on.** Domain
depends on nothing. Application depends only on Domain. Infrastructure
implements interfaces that Application declares. Api references Infrastructure
for one reason only — so `Program.cs` can register the concrete classes in DI.
Nothing else in Api may mention an Infrastructure type.

If you ever want to add a reference that points the other way — Domain needing
something from Infrastructure, say — that is the design telling you the code is
in the wrong project. Move the code, don't add the reference.

## Where does my code go?

| I am writing… | It goes in |
|---|---|
| An entity or an enum that describes the business | `Domain/Entities`, `Domain/Enums` |
| A request or response shape the API exposes | `Application/Contracts` |
| A business rule, an ownership check, orchestration | `Application/Services` |
| An interface describing something the outside world does | `Application/Abstractions` |
| A FluentValidation validator | `Application/Validation` |
| Anything that touches the database | `Infrastructure/Persistence`, `Infrastructure/Repositories` |
| An EF migration | `Infrastructure/Migrations` (generated, never hand-edited) |
| Anything about passwords, tokens or Identity | `Infrastructure/Identity` |
| An HTTP route | `Api/Endpoints` |
| DI wiring, auth setup, CORS | `Api/Extensions` |
| A test with a mock in it | `Tests.Unit` |
| A test that talks to a real Postgres | `Tests.Integration` |

## The shape of a feature

Every feature lands as the same four moves, in this order:

1. **Domain** — add or extend the entity, if the feature needs one.
2. **Application** — add the DTOs, the service interface, the service, the
   validator. This is where the feature actually lives.
3. **Infrastructure** — implement whatever the service asked for: a repository,
   an EF configuration, a migration.
4. **Api** — add an endpoint file that does three things and nothing else:
   take the request, call the service, return the result.

If step 4 is more than a few lines per route, logic has leaked upward into the
transport layer. Push it back down into a service.

For a worked example that names every single file, see
[`docs/example-slice-auth.md`](../docs/example-slice-auth.md), which walks
through register-and-login end to end.

## Commands

```bash
dotnet build backend/Yggdrasil.sln
dotnet test backend/Yggdrasil.sln
```

Run them from the repository root, not from this folder.
