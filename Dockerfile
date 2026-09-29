# One image for the whole app, shaped like the Azure deploy: the API serves the
# built frontend from wwwroot, so the browser only ever talks to one origin.

# --- frontend: static files only ---
FROM node:24-alpine AS frontend
WORKDIR /src/frontend

COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci

COPY frontend/ ./
# Vite bakes this in at build time; it must be the origin the browser opens.
ARG VITE_API_BASE_URL=http://localhost:8080
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
RUN npm run build

# --- backend: restore from the project files first so code edits keep the cache ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend
WORKDIR /src

COPY global.json dotnet-tools.json Directory.Build.props ./
COPY backend/Yggdrasil.Api/Yggdrasil.Api.csproj backend/Yggdrasil.Api/
COPY backend/Yggdrasil.Application/Yggdrasil.Application.csproj backend/Yggdrasil.Application/
COPY backend/Yggdrasil.Domain/Yggdrasil.Domain.csproj backend/Yggdrasil.Domain/
COPY backend/Yggdrasil.Infrastructure/Yggdrasil.Infrastructure.csproj backend/Yggdrasil.Infrastructure/
RUN dotnet restore backend/Yggdrasil.Api/Yggdrasil.Api.csproj

COPY backend/ backend/
RUN dotnet publish backend/Yggdrasil.Api/Yggdrasil.Api.csproj \
    --configuration Release --no-restore --output /app /p:UseAppHost=false

# --- migrations: an EF bundle, so the runtime needs neither the SDK nor dotnet-ef ---
FROM backend AS bundle
# dotnet-ef boots Program.cs to find the DbContext, and startup refuses to run
# without these. Nothing connects at build time, so placeholders are enough; they
# are scoped to this one command and never reach an image.
RUN dotnet tool restore \
    && ConnectionStrings__Postgres="Host=build-placeholder" \
    Jwt__IssuerSigningKey="build-placeholder-not-a-real-signing-key" \
    dotnet ef migrations bundle \
    --project backend/Yggdrasil.Infrastructure \
    --startup-project backend/Yggdrasil.Api \
    --configuration Release --output /efbundle

# Run by Compose before the app starts. The bundle boots Program.cs to find the
# DbContext, so it needs appsettings.json and the same environment as the app.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS migrator
WORKDIR /app
COPY --from=backend /app/appsettings.json ./
COPY --from=bundle /efbundle ./
USER $APP_UID
ENTRYPOINT ["./efbundle"]

# --- app: the default target, what `docker build .` produces ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS app
WORKDIR /app
COPY --from=backend /app ./
COPY --from=frontend /src/frontend/dist ./wwwroot/
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Yggdrasil.Api.dll"]