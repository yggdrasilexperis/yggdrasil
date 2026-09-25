# Contributing

Getting a fresh clone running is in [README.md](README.md). This file is about
how we work once it is.

## Where code goes

```
Api ──► Application ──► Domain
 └────► Infrastructure ──► Application, Domain
```

Arrows point at what you may depend on. `Api` references `Infrastructure` only
so `Program.cs` can register implementations in DI. A reference wanting to point
the other way means the code is in the wrong project.

`backend/README.md` has the "where does my code go" table; each project has its
own `README.md`.

## Branches and commits

Branch off `main`, one branch per issue: `feat/`, `fix/`, `chore/`, `docs/`
(e.g. `feat/create-quiz`). Never push to `main`

Keep commits small and explain **why** in the message. "Return 400 instead of
500 when an email is already registered", not "fix bug".

## Pull requests

Fill in the template, link the issue with `Closes #12`, set yourself as assignee
and a someone else as reviewer.

## Naming

**C#** — `PascalCase` types/methods/properties, `camelCase` locals, `_camelCase`
private fields, `I`-prefixed interfaces. One class per file, named after the
class. Minimal APIs, not controllers: one `XEndpoints` class per resource, and
handlers stay thin — request in, service call, result out. Never return an
entity from an endpoint; DTOs only.

**React** — components `PascalCase.tsx`, hooks `useThing.ts`, one per file. All
HTTP through the single typed API client, never `fetch` in a component.

## Formatting

Use `.editorconfig` for formatting.

```bash
dotnet format backend/Yggdrasil.sln
npm --prefix frontend run lint
```

## Secrets

Never commit a real secret. Each kind of setting has exactly one home:

| Setting | Locally | In the cloud |
|---|---|---|
| Postgres container (`POSTGRES_*`) | `.env`, copied from `.env.example` | not used |
| API: connection string, JWT key, seed password | `dotnet user-secrets --project backend/Yggdrasil.Api` | environment variables, e.g. `ConnectionStrings__Postgres` |
| Frontend (`VITE_*`) | `frontend/.env`, copied from `frontend/.env.example` | set in the build job |
| Deployment credentials | – | GitHub Actions secrets |

Add GitHub Actions secrets under **Settings → Secrets and variables → Actions**,
and reference them in a workflow as `${{ secrets.NAME }}`. Never paste a value
into a `.yml` file.

Adding a setting? Add it to the matching `.env.example` with a placeholder, and
make the app fail at startup with a clear message if it is missing.

## Migrations

Generate migrations with EF core, dont write them by hand. Do not edit migrations that are already merged in git.
