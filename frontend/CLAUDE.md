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

There is no frontend test suite yet. CI (`.github/workflows/ci.yml`) gates on
`format:check`, `lint` and `build` (so `tsc` errors fail the PR too). Markdown in this
folder, this file included, goes through Prettier as well.

Backend must be running for anything real: `docker compose up -d db` then
`dotnet watch --project backend/Yggdrasil.Api` from the repo root. See the root
`README.md` for first-time setup (user-secrets, JWT key, migrations, seeding).

## Backend contract

- API base URL comes from `VITE_API_BASE_URL` in `.env`. Never hardcode it.
- API runs at `http://localhost:5172`. OpenAPI doc (dev only):
  `http://localhost:5172/openapi/v1.json` — read it before guessing a route or shape.
- CORS is an allowlist (`Cors:AllowedOrigins`). A new frontend origin must be added
  backend-side or the browser blocks every request.

**Implemented endpoints** (`src/api/types.ts` mirrors the shapes):

| Route                                      | Who            | Body / query                                       | Returns                                    |
| ------------------------------------------ | -------------- | -------------------------------------------------- | ------------------------------------------ |
| `POST /api/auth/register`                  | anyone         | `{ email, userName, password }`                    | 201 `AuthResponse`                         |
| `POST /api/auth/login`                     | anyone         | `{ email, password }`                              | 200 `AuthResponse`                         |
| `GET /api/quizzes`                         | anyone         | `?page&pageSize&sortBy&sortDirection&categorySlug` | `PagedResult<QuizSummary>`                 |
| `GET /api/quizzes/{id}`                    | anyone         | —                                                  | `QuizDetail` (quiz + questions + comments) |
| `POST /api/quizzes`                        | signed in      | `CreateQuizRequest` (≥ 1 `categoryIds`)            | 201 `QuizSummary`                          |
| `PUT /api/quizzes/{id}`                    | owner or Admin | same shape as create; replaces the whole quiz      | `QuizSummary`                              |
| `DELETE /api/quizzes/{id}`                 | owner or Admin | —                                                  | 204                                        |
| `GET /api/quizzes/{id}/questions`          | anyone         | —                                                  | `Question[]`                               |
| `POST /api/quizzes/{id}/questions`         | owner or Admin | `{ text, answerOptions }`                          | 201 `Question`                             |
| `DELETE /api/quizzes/{id}/questions/{qid}` | owner or Admin | —                                                  | 204                                        |
| `GET /api/quizzes/{id}/comments`           | anyone         | —                                                  | `Comment[]`                                |
| `GET /api/categories`                      | anyone         | —                                                  | `Category[]` (a fixed, seeded list)        |

`AuthResponse` is `{ token, expiresAt, user }`, and `user` is
`{ id, email, userName, roles }` with roles `"Admin"` / `"User"`. Auth is a JWT bearer
token — send it as `Authorization: Bearer <token>`. There is no endpoint for posting
comments yet. Seed accounts (`alva@example.com`, `jonas@example.com`,
`admin@example.com`) share the password set in `Seed:Password` when the database was
seeded (README step 7).

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

Auth and browsing, viewing, creating and deleting quizzes run against the real API (no
mocks). Routing, layout and state conventions below are settled — follow them rather than
re-deciding per page.

| Path                  | Page                                                        | Access        |
| --------------------- | ----------------------------------------------------------- | ------------- |
| `/`                   | `quiz/DiscoverPage` — first page of quizzes                 | public        |
| `/quizzes/:id`        | `quiz/QuizDetailPage` — Edit/Delete shown to owner or admin | public        |
| `/quizzes/new`        | `quiz/CreateQuizPage`                                       | `RequireAuth` |
| `/quizzes/:id/edit`   | `quiz/QuizEditStub` — placeholder, editor is its own issue  | `RequireAuth` |
| `/login`, `/register` | `auth/SignInPage`, `auth/SignUpPage`                        | public        |

**API client.** `src/api/client.ts` is the single `request()`; per-resource functions sit
beside it (`auth.ts`, `quiz.ts`, `quizzes.ts` — quiz calls are currently split across the
last two). `session.ts` persists the token; a 401 on an authenticated call signs the user
out through `setSessionExpiredHandler`. Hiding a button with `isAdmin` or an owner check
is UX only — the backend is the guard.

Known gaps: Discover shows only page 1 (no pager), and comments posted on the detail page
live in memory until the backend has an endpoint for them.

**Routing & layout.** `App.tsx` declares routes with `react-router-dom`; every route
nests under one `AppLayout` layout route (`src/layout/AppLayout.tsx`) that renders the
header/nav once and the page into its `<Outlet />` — a page component never renders its
own `<header>` or top-level `<main>`. `RequireAuth` gates protected routes and bounces to
`/login`. An unmatched path renders `NotFoundPage`, not a redirect.

**Components.** Shared, generic primitives (`Button`, `Input`, ...) live in
`src/components/`. Page components live at the top of `src/` (`NotFoundPage.tsx`) or,
once a feature has more than one file, in its own folder next to the components/hooks
only it uses (`src/auth/`, `src/quiz/`). Pull markup into a component the second time it
repeats, not the first.

**Data loading.** Pages fetch in `useEffect` with a `cancelled` flag in the cleanup (see
`DiscoverPage`). The `react-hooks` lint rules reject a synchronous `setState` in an
effect body, so set state in the promise callbacks.

**State.** Cross-page state goes through React Context plus a `useX` hook — see
`AuthContext`/`AuthProvider`/`useAuth`. Reach for this only when more than one page
needs the state; keep everything else local with `useState`. No state-management
library.
