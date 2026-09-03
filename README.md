# Yggdrasil

An app to create and view quizzes

## Prerequisites

- **.Net SDK**: use the version specified in `global.json`
- **Node.js 24+**
- **Docker** with Compose, for the database

## First-time setup

**0. Clone the repo**

```bash
git clone git@github.com:yggdrasilexperis/yggdrasil.git
```

**1. Environment file.** Copy the example and set your own password.

```bash
cp .env.example .env
```

**2. Start the database.**

```bash
docker compose up -d db
```

**3. Restore the local tools**

```bash
dotnet tool restore
```

**4. Tell the API how to reach the database.** The connection string is a
secret, so it lives outside the repository. Into the command insert the password you have in `.env`

```bash
dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Port=5432;Database=yggdrasil;Username=yggdrasil;Password=<your .env password>" --project backend/Yggdrasil.Api
```

**5. Configure the JWT issuer signing key.** The API signs every token it issues
with this key and refuses to start without one. It is per-developer. Generate
your own, never commit or share it.

> [!IMPORTANT]
> **Prerequisite:** `openssl`
>> Bundled with Git for Windows (use Git Bash), preinstalled on macOS and most Linux distributions.

```bash
dotnet user-secrets set "Jwt:IssuerSigningKey" "$(openssl rand -hex 48)" --project backend/Yggdrasil.Api
```

**6. Create the schema.**

```bash
dotnet ef database update --project backend/Yggdrasil.Infrastructure --startup-project backend/Yggdrasil.Api
```



**7. Seed database.**

```
dotnet run --project backend/Yggdrasil.Api -- --seed
```

The test users all share the same password:

**8. Install frontend dependencies.**

```bash
npm --prefix frontend install
```

## Running it

Run each command in seperate terminals

```bash
docker compose up -d db
```

```bash
dotnet watch --project backend/Yggdrasil.Api
```

```bash
npm --prefix frontend run dev
```

- API lives at <http://localhost:5172>
- OpenAPI documents live at <http://localhost:5172/openapi/v1.json>
- Frontend lives at: <http://localhost:5173>

`backend/Yggdrasil.Api/Yggdrasil.Api.http` has ready requests you can run without needing frontend

## Everyday commands

```bash
# tests
dotnet test backend/Yggdrasil.sln

# formatting
dotnet format backend/Yggdrasil.sln --exclude backend/Yggdrasil.Infrastructure/Migrations
npm --prefix frontend run format
npm --prefix frontend run lint

# a new migration
dotnet ef migrations add <Name> --project backend/Yggdrasil.Infrastructure --startup-project backend/Yggdrasil.Api
dotnet ef database update --project backend/Yggdrasil.Infrastructure --startup-project backend/Yggdrasil.Api
```

Stop the db with `docker compose down`. This DOES keep your data.

Destroy the volume with `docker compose down -v`. This DOES NOT keep your data.

## Troubleshooting

**`Could not find the global property 'UserSecretsId'`**: you are running the
command from the wrong project. Pass `--project backend/Yggdrasil.Api`.

**Connection refused from the API**: the database container is not running.
`docker compose ps` to check, `docker compose up -d db` to start it.

**`password authentication failed`**: the password in your user-secrets
connection string does not match `POSTGRES_PASSWORD` in your `.env`.

**Permission denied on the Docker socket**: your user was added to the `docker`
group after this shell started. Open a new terminal.
