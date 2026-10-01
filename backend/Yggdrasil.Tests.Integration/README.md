# Yggdrasil.Tests.Integration

Tests the whole API over HTTP against a real Postgres, which Testcontainers starts
in Docker. **Docker has to be running.**

```
Fixtures/   ApiFactory: the app plus the Postgres container
Auth/       register, login and token checks
Quiz/       quiz, question and category endpoints
Routing/    unknown /api routes give 404, everything else gets the frontend
Seeding/    the demo data seeder
```

Tests check status codes and response bodies, the way a real client sees them.
