# Backend

A .NET 10 solution with four app projects and two test projects. The split keeps
the business rules in plain C# that you can test without a web server or a
database. Which project may reference which is in
[CONTRIBUTING.md](../CONTRIBUTING.md#where-code-goes).

| Project | What's in it |
|---|---|
| [Yggdrasil.Api](Yggdrasil.Api) | HTTP endpoints, auth setup, error handling |
| [Yggdrasil.Application](Yggdrasil.Application) | Business rules, request and response types, validation |
| [Yggdrasil.Domain](Yggdrasil.Domain) | Entities |
| [Yggdrasil.Infrastructure](Yggdrasil.Infrastructure) | EF Core and Postgres, Identity, JWT, demo data |
| [Yggdrasil.Tests.Unit](Yggdrasil.Tests.Unit) | Service and validator tests with mocks |
| [Yggdrasil.Tests.Integration](Yggdrasil.Tests.Integration) | API tests against a real Postgres |
