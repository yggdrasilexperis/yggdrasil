namespace Yggdrasil.Application.Abstractions;

// Placement TBD?
// Only returned by ITokenService, so for now I have it here to minimize clutter
// If used by other services later it should maybe be under Contracts/ instead?
public sealed record AccessToken(
    string Value,
    DateTimeOffset ExpiresAt);
