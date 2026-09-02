namespace Yggdrasil.Application.Abstractions;

public sealed record CreateUserResult(
    bool Success,
    UserAccount? User,
    IReadOnlyList<string> Errors);
