# Yggdrasil.Api

The HTTP layer. Minimal APIs, not controllers.

**References:** Application (for services and contracts), Infrastructure (only
so `Program.cs` can register implementations in DI).

## Layout

```
Program.cs      short: build, add services, map endpoints, run
Endpoints/      one file per resource — AuthEndpoints, QuizEndpoints, …
Extensions/     AddApiServices, AddJwtAuth, MapApiEndpoints
Filters/        ValidationFilter<T>
Handlers/       GlobalExceptionHandler → ProblemDetails (card #12)
appsettings.json  non-secret defaults only
```

## How an endpoint file looks

One static class per resource, exposing one extension method that builds a
route group:

```csharp
public static class QuizEndpoints
{
    public static IEndpointRouteBuilder MapQuizEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/quizzes")
                       .WithTags("Quizzes")
                       .RequireAuthorization();

        group.MapGet("/{id:int}", GetById);
        group.MapPost("/", Create).AddEndpointFilter<ValidationFilter<CreateQuizRequest>>();

        return app;
    }

    private static async Task<Results<Ok<QuizResponse>, NotFound>> GetById(
        int id, IQuizService quizzes, CancellationToken ct)
    {
        var quiz = await quizzes.GetByIdAsync(id, ct);
        return quiz is null ? TypedResults.NotFound() : TypedResults.Ok(quiz);
    }
}
```

`Program.cs` then calls `app.MapQuizEndpoints()` — or a single
`app.MapApiEndpoints()` in `Extensions` that calls each group in turn, which is
what we do once there is more than one.

## Rules

- **Handlers are three lines.** Take the request, call a service, return a
  result. A handler containing an `if` about business rules is a bug in the
  wrong place — that belongs in an Application service.
- **Never return an entity.** Contracts only. This is graded, and it is also
  how password hashes leak.
- Every async handler takes a `CancellationToken` and passes it down.
- Use `TypedResults`, not `Results`. It gives you the status codes in the method
  signature, which OpenAPI reads for free.
- `appsettings.json` holds non-secret defaults. Secrets — the JWT signing key,
  the connection string — come from user-secrets locally and environment
  variables everywhere else.
