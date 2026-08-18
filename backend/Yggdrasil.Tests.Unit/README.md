# Yggdrasil.Tests.Unit

The service layer, tested with mocks. No database, no web host, no
network. These run in milliseconds and should be the tests you run constantly.

**References:** Application, Domain. NSubstitute for mocks, Shouldly for
assertions.

## Layout

Mirror the structure of the project under test:

```
Services/     QuizServiceTests, CommentServiceTests
Validation/   CreateQuizRequestValidatorTests
```

## What to test here

The decisions, not the plumbing:

- A user who does not own a quiz cannot update it.
- A user cannot comment on their own quiz.
- A question must have at least two options with exactly one correct.
- The validator rejects an empty title and accepts a valid request.

## Shape

```csharp
[Fact]
public async Task UpdateAsync_WhenCallerIsNotOwner_Throws()
{
    var repository = Substitute.For<IQuizRepository>();
    repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
              .Returns(new Quiz { Id = 1, OwnerId = someoneElse });

    var service = new QuizService(repository, currentUser);

    await Should.ThrowAsync<ForbiddenException>(
        () => service.UpdateAsync(1, request, CancellationToken.None));
}
```

Name tests `Method_Scenario_ExpectedResult`. When one fails in CI, the name
alone should tell the reviewer what broke.

## What not to test here

Anything whose behaviour depends on EF actually running — cascade deletes,
unique constraints, query translation. A mocked repository will happily agree
with a wrong assumption. Those belong in `Tests.Integration`.
