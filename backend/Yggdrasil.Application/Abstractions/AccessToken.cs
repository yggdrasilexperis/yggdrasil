namespace Yggdrasil.Application.Abstractions;

public sealed record AccessToken(
    string Value,
    DateTimeOffset ExpiresAt);
