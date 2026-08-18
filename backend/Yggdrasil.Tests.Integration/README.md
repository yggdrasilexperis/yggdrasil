# Yggdrasil.Tests.Integration

Card #20: the API tested end to end against a real PostgreSQL, started in a
container by Testcontainers. Slower than the unit tests, and worth it — this is
the layer that catches the things mocks lie about.

**References:** Api (through `WebApplicationFactory`), Testcontainers.PostgreSql,
Shouldly.

**Requires Docker to be running.** That is the one thing that makes these tests
fail on a laptop that has not set it up.

## Layout

```
Fixtures/     ApiFactory — WebApplicationFactory + the Postgres container
Auth/         RegisterEndpointTests, LoginEndpointTests
Quizzes/      QuizEndpointTests
```

## What to test here

Whole requests, through the real pipeline, against the real schema:

- `POST /api/auth/register` returns 201 and the user can then log in.
- Registering a duplicate email returns 400, not 500.
- Requesting a protected route without a token returns 401.
- Updating someone else's quiz returns 403.
- Deleting a quiz really does remove its questions (cascade).
- The response body never contains a password hash.

Assert on status codes and response bodies, the way a client sees them. Do not
reach into the `DbContext` to check state — if the API does not expose it, the
test should not know about it.

## Rules

- Each test starts from a known database state. Reset between tests rather than
  letting them share leftovers, or you get failures that depend on ordering.
- These tests must run in CI (card #7), so they cannot depend on anything
  installed by hand on a particular machine.
