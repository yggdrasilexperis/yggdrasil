namespace Yggdrasil.Application.Contracts.Quiz;

public record CommentResponse(
    Guid QuizId,
    Guid AuthorId,
    string Body,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
    
