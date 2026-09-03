namespace Yggdrasil.Application.Contracts.Quiz;

public record CategoryResponse(
    Guid CategoryId,
    string Name,
    string Slug);

