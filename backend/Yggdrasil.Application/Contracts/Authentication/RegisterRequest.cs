namespace Yggdrasil.Application.Contracts.Authentication;

public sealed record RegisterRequest(
    string Email,
    string UserName,
    string Password);
