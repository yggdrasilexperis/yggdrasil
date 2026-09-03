namespace Yggdrasil.Application.Contracts.Authentication;

public sealed record AuthResponse(
    string Token,
    DateTimeOffset ExpiresAt,
    UserResponse User);
