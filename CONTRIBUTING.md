# Contributing

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

Put secrets in `.env`. Update `.env.example` if necessary
Locally, you may have to use `dotnet user-secrets` to use secrets in C#

## Migrations

Generate migrations with EF core, dont write them by hand. Do not edit migrations that are already merged in git.
