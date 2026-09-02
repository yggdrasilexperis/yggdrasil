namespace Yggdrasil.Application.Contracts.Authentication;

public sealed record UserResponse(
    Guid Id,
    string Email,
    string UserName);
