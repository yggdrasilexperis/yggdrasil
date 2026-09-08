using Yggdrasil.Domain.Enums;

namespace Yggdrasil.Application.Contracts.Quiz;

public record UpdateQuizRequest(
    string Title,
    string Description,
    Difficulty Difficulty,
    ICollection<Guid> CategoryIds);
