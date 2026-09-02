using Yggdrasil.Application.Contracts.Authentication;

namespace Yggdrasil.Application.Abstractions;

// Placement TBD?
// Only returned by IIdentityService, so for now I have it here to minimize clutter
// If used by other services later it should maybe be under Contracts/ instead?
public sealed record CreateUserResult(
    bool Success,
    UserResponse? User,
    IReadOnlyList<string> Errors);
