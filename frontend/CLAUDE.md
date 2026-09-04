# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

The React frontend for **Yggdrasil**, an app to create and view quizzes. The real
application lives in `../backend` (.NET, clean architecture). This project is a client:
it renders the backend's data and calls its HTTP API. Business rules belong in the
backend — do not reimplement or duplicate them here.

The repo root is `../` (git root). This directory is one half of it.

## Commands

Run from `frontend/`:

```bash
npm run dev          # vite dev server, http://localhost:5173
npm run build        # tsc -b && vite build
npm run lint         # eslint
npm run format       # prettier --write .
npm run format:check # what CI enforces
```

There is no frontend test suite yet. CI (`.github/workflows/ci.yml`) runs `npm ci` and
`format:check` only — `lint` is not gated, but keep it clean anyway.

Backend must be running for anything real: `docker compose up -d db` then
`dotnet watch --project backend/Yggdrasil.Api` from the repo root. See the root
`README.md` for first-time setup (user-secrets, JWT key, migrations, seeding).

## Backend contract

- API base URL comes from `VITE_API_BASE_URL` in `.env`. Never hardcode it.
- API runs at `http://localhost:5172`. OpenAPI doc (dev only):
  `http://localhost:5172/openapi/v1.json` — read it before guessing a route or shape.
- CORS is an allowlist (`Cors:AllowedOrigins`). A new frontend origin must be added
  backend-side or the browser blocks every request.

**Implemented endpoints** (only these exist so far — quiz endpoints are not built yet):

| Route                     | Body                            | Returns                          |
| ------------------------- | ------------------------------- | -------------------------------- |
| `POST /api/auth/register` | `{ email, userName, password }` | 201 `{ token, expiresAt, user }` |
| `POST /api/auth/login`    | `{ email, password }`           | 200 `{ token, expiresAt, user }` |

`user` is `{ id, email, userName }`. Auth is a JWT bearer token — send it as
`Authorization: Bearer <token>`.

**Errors are always RFC 7807 ProblemDetails**: `{ status, title, detail, instance }`.
Validation failures (400) add an `errors` extension: `{ [fieldName]: string[] }`.
Handle errors off this shape, not off ad-hoc guesses. Status codes in use: 400, 401,
403, 404, 409, 500.

**Registration rules** are enforced server-side (mirror them in the UI, don't invent
stricter ones): email must be a valid address; username 3–32 chars, `[a-zA-Z0-9_-]`
only; password 8–32 chars.

**Domain shapes** (from `backend/Yggdrasil.Domain/Entities`) — a Quiz has a title,
optional description, a `Difficulty` (`Easy | Normal | Hard | Expert`), an owner, many
Questions, many Comments and many Categories. A Question has text and many
AnswerOptions; an AnswerOption has text and `isCorrect`. Note the backend never returns
entities directly, only DTOs, so treat these as a guide to the domain and confirm the
actual JSON against the OpenAPI doc.

## Working rules

**Stay lean.** This is a small quiz client. Prefer the plain solution over the general
one. No state-management library, no data-fetching library, no component framework, no
abstraction layer added "for later" — React state, `useEffect`, and the API client
cover this app. Add a dependency only when a concrete need can't be met without it, and
say why.

**All HTTP goes through one typed API client** in `src/api/`. Never call `fetch`
directly from a component. The client owns the base URL, the bearer token, JSON
parsing, and turning ProblemDetails into a typed error. One place to change when the
backend changes.

**Styling is Tailwind v4** (via `@tailwindcss/vite`, no `tailwind.config.js` — v4
configures in CSS). Utility classes in the markup; no CSS modules, no styled components,
no per-component `.css` files. `src/index.css` is the only stylesheet: the Tailwind
import plus an `@theme` block of global tokens. Extract a component, not a CSS class,
when markup repeats.

Tokens live in `@theme` and become utilities automatically — `--color-accent` gives you
`bg-accent`, `text-accent`, `ring-accent`. Add a token only when a value is genuinely
global; a one-off colour belongs inline.

**`DESIGN.md` is the visual authority** — palette, type scale, spacing, component
recipes, and what's deliberately excluded. Read it before writing UI. It describes the
tokens already defined in `src/index.css`; if the two disagree, the CSS is right and the
doc needs fixing.

**Modular, but only when earned.** Shared primitives (Button, Input, Card, etc.) go in
`src/components/`, feature-specific components next to their feature. Build a reusable
component the second time you need it, not the first — speculative props and variants
are the bloat to avoid. Keep props small and typed; no `any`.

## Conventions

From the repo's `CONTRIBUTING.md`:

- Components `PascalCase.tsx`, hooks `useThing.ts`, one per file.
- Branch off `main`, one branch per issue: `feat/`, `fix/`, `chore/`, `docs/`.
  **Never push to `main`.** Commit messages explain _why_.
- PRs use the template, link the issue with `Closes #12`, assign yourself and a reviewer.

Prettier config is committed (`.prettierrc`): single quotes, semicolons, 100 col,
trailing commas. Let the formatter decide — don't hand-format.

TypeScript is strict-ish via `tsconfig.app.json` (`noUnusedLocals`,
`noUnusedParameters`, `verbatimModuleSyntax`, `erasableSyntaxOnly`). Type-only imports
need `import type`.

## Current state

Scaffolding only. The Vite template markup, CSS and assets have been stripped;
`App.tsx` is a placeholder landing page and the first real work replaces it.
`react-router-dom` is installed but not wired up, and `src/api/` and `src/components/`
do not exist yet — create them when the first caller needs them.
