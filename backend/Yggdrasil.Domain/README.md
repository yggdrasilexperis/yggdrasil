# Yggdrasil.Domain

The entities and nothing else. It doesn't reference any other project.

```
Entities/    Quiz, Question, AnswerOption, Category, Comment
Enums/       Difficulty
Constants/   role names and category slugs
```

There's no user entity here. `ApplicationUser` is an Identity class, so it lives in
Infrastructure, and entities just store the user's id as a plain `Guid`. There are
no validation attributes either: requests are validated in Application and the
table setup is in Infrastructure.
