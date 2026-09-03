namespace Yggdrasil.Application.Contracts;

public sealed record AccessToken(
    string Value,
    DateTimeOffset ExpiresAt);
