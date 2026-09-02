namespace Yggdrasil.Application.Abstractions;

public sealed record UserAccount(
    Guid Id,
    string Email,
    string UserName);
