namespace Yggdrasil.Application.Contracts.Quiz;

public record CommentResponse(
    Guid Id,
    Guid AuthorId,
    string Body,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

