# Yggdrasil

A quiz app. You sign up, build quizzes out of multiple-choice questions, tag them
with categories, browse and filter everyone's quizzes, and comment on them. Admins
can clean up any quiz or comment.

**Live:** <https://yggdrasil-experis.azurewebsites.net>

## Run it locally

You need Docker and `openssl`. Copy the env file, then open `.env` and set your own
`POSTGRES_PASSWORD` and `SEED_PASSWORD`:

```bash
cp .env.example .env
```

Generate a JWT signing key into `.env`:

```bash
sed -i.bak "s/^JWT_SIGNING_KEY=.*/JWT_SIGNING_KEY=$(openssl rand -hex 48)/" .env && rm .env.bak
```

Start everything:

```bash
docker compose up --build
```

This starts Postgres, runs the migrations, seeds demo data and serves the app on
<http://localhost:8080>. Log in as `alva@example.com`, `jonas@example.com` or
`admin@example.com` with your `SEED_PASSWORD`. Your data survives
`docker compose down`, and `docker compose down -v` wipes it.

## Run the tests

You need the .NET SDK version from `global.json`, and Docker has to be running
because the integration tests start their own Postgres container.

```bash
dotnet test backend/Yggdrasil.sln
```

The frontend has no tests. CI checks it with lint, formatting and a build.

## Environment variables

These live in `.env`:

| Variable | What it's for |
|---|---|
| `POSTGRES_PASSWORD` | Database password. Required. |
| `JWT_SIGNING_KEY` | Signs login tokens. Required, at least 32 characters. |
| `SEED_PASSWORD` | Password for the three demo users. Required. |
| `POSTGRES_DB`, `POSTGRES_USER` | Database name and user. Both default to `yggdrasil`. |
| `POSTGRES_PORT`, `APP_PORT` | Ports on your machine. Default `5432` and `8080`. |

Compose hands these to the app as `ConnectionStrings__Postgres`,
`Jwt__IssuerSigningKey`, `Seed__Password` and `Cors__AllowedOrigins__0`. Those are
the names to set anywhere else, like on Azure. The frontend also needs
`VITE_API_BASE_URL` at build time, which Docker and CI set for you.

## Architecture

- **Frontend:** React and TypeScript, built with Vite. More in
  [frontend/README.md](frontend/README.md).
- **Backend:** ASP.NET Core minimal API on .NET 10, with EF Core, ASP.NET Core
  Identity and JWT login. Split into Api, Application, Domain and Infrastructure,
  see [backend/README.md](backend/README.md).
- **Database:** PostgreSQL 17. A Docker container locally, Azure Database for
  PostgreSQL in production.
- **Deployment:** The API serves the built frontend, so it all runs as one app.
  GitHub Actions checks formatting, builds, runs the tests and smoke-tests the
  Docker image on every PR. Merging to `main` deploys to Azure App Service.

How we work (branches, PRs, secrets) is in [CONTRIBUTING.md](CONTRIBUTING.md).

## Team

- Izaak Krystian Sarnecki
- Fredrik Andreas Wiik
- Markus Anglero

## Domain mapping

The assignment is written against five placeholders. This is what we picked:

| Placeholder | Ours |
| --- | --- |
| `[USER]` | User, handled by ASP.NET Core Identity |
| `[PRIMARY]` | Quiz, owned by one user |
| `[CHILD]` | Question, belongs to one quiz and has answer options |
| `[TAG]` | Category, many-to-many with quizzes |
| `[INTERACTION]` | Comment, left by a user on a quiz |
