namespace Yggdrasil.Application.Contracts.Quiz;

public record CommentResponse(
    Guid Id,
    Guid AuthorId,
    String AuthorUsername,
    string Body,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
