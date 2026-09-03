using Yggdrasil.Application.Contracts.Authentication;

namespace Yggdrasil.Application.Contracts;

public sealed record CreateUserResult(
    bool Success,
    UserResponse? User,
    IReadOnlyList<CreateUserResult.Error> Errors
)
{
    public sealed record Error(string Code, string Description);
}
