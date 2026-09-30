# Yggdrasil.Tests.Unit

Tests for the services and validators in Application, using NSubstitute for mocks
and Shouldly for assertions. No database and no network, so the whole suite runs in
under a second.

```
Services/     AuthServiceTests, QuizServiceTests
Validation/   one test class per validator
```

Test names follow `Method_Scenario_ExpectedResult`, for example
`DeleteAsync_WhenCallerIsNotOwner_ThrowsForbidden`.
