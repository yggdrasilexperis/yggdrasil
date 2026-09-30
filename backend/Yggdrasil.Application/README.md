# Yggdrasil.Application

The business rules, like who's allowed to change what. It knows nothing about HTTP
or EF Core, which is why the unit tests can cover it with mocks.

```
Abstractions/  interfaces that Infrastructure and Api implement
Contracts/     request and response records
Exceptions/    NotFound, Forbidden, Conflict and so on, mapped to status codes by Api
Options/       JWT settings
Services/      AuthService and QuizService
Validation/    one FluentValidation validator per request
```

Only a quiz's owner or an admin can edit or delete it and its questions. Only a
comment's author can edit it, and the author or an admin can delete it.
