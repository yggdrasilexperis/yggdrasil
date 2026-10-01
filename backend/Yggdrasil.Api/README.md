# Yggdrasil.Api

The HTTP layer, using minimal APIs. An endpoint takes the request, calls a service
and returns the result. No business logic here, and no entities in responses,
only contracts from Application.

```
Program.cs     startup; `--seed` seeds the database and exits
Endpoints/     one file per resource: auth, quizzes, categories
Extensions/    JWT auth, CORS, OpenAPI, current user
Filters/       runs the FluentValidation validators on requests
Handlers/      turns exceptions into ProblemDetails responses
Identity/      CurrentUser, reads the logged-in user from the token
```

In Development the OpenAPI document is at `/openapi/v1.json`, and
`Yggdrasil.Api.http` has ready-made requests you can fire at the API.
