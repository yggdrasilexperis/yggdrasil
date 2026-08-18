# Yggdrasil.Domain

The nouns of the application, and nothing else.

**References:** nothing.

## Layout

```
Entities/     Quiz, Question, AnswerOption, Category, Comment
Enums/        Difficulty
```

## What belongs here

- Entity classes: properties, navigation properties, and rules that are true
  about the thing itself regardless of how it is stored or transported.
- Enums that describe the business (`Difficulty`).

## What does not

- **`[Required]`, `[MaxLength]` and friends.** Validation of incoming requests
  is Application's job; the shape of the database is configured in
  Infrastructure with EF's fluent API. Keeping attributes out means the entity
  doesn't quietly become a description of a table.
- **The user account.** `ApplicationUser` inherits from ASP.NET Core Identity,
  which is an Infrastructure concern, so it lives in `Infrastructure/Identity`.
  Entities here refer to a user with a plain `Guid OwnerId` and no navigation
  property. That is deliberate, not an oversight.
- DTOs. Those are Application's `Contracts`.
