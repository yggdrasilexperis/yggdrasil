using Yggdrasil.Domain.Enums;

namespace Yggdrasil.Application.Contracts.Quiz;

public record QuizResponse(
    Guid Id,
    string Title,
    string Description,
    Guid OwnerId,
    Difficulty Difficulty,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IEnumerable<CategoryResponse> Categories);
