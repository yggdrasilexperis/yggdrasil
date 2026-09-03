using Yggdrasil.Application.Contracts;
using Yggdrasil.Application.Contracts.Authentication;

namespace Yggdrasil.Application.Abstractions;

public interface IIdentityService
{
    Task<UserResponse?> FindByEmailAsync(
        string email, CancellationToken cancellationToken);

    Task<CreateUserResult> CreateUserAsync(
        RegisterRequest registerRequest, CancellationToken cancellationToken);

    Task<bool> CheckPasswordAsync(Guid userId, string password, CancellationToken cancellationToken);
}
