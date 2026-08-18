# Yggdrasil.Application

The rules live here

**References:** Domain, FluentValidation. Nothing about HTTP, nothing about EF.

## Layout

```
Abstractions/   interfaces this layer needs someone else to implement
                (IQuizRepository, IAuthService, ICurrentUser)
Contracts/      request and response records, grouped per feature
                (Contracts/Quizzes/CreateQuizRequest.cs, QuizResponse.cs …)
Services/       the implementations of the business rules
Validation/     FluentValidation validators, one per request contract
```

## What belongs here

- **Services.** `QuizService.UpdateAsync` checking that the caller owns the quiz
  before touching it — that check is the single most important line in the
  codebase for card #27, and it lives here, not in an endpoint.
- **Contracts.** Every shape crossing the API boundary in either direction.
  These are `record` types. Entities are never returned to a client, so a
  service returns a `QuizResponse`, never a `Quiz`.
- **Abstractions.** When a service needs to load data or hash a password, it
  declares an interface here and lets Infrastructure supply the implementation.
  That inversion is what makes these services mockable.

## What does not

- `DbContext`, `DbSet`, LINQ-to-Entities, connection strings.
- `HttpContext`, `IResult`, status codes, route strings.

## Rule of thumb

If you cannot unit-test a class in this project without spinning up a database
or a web host, it has a dependency it should not have. Find the dependency,
turn it into an interface in `Abstractions`, and move the implementation to
Infrastructure.
