using FluentValidation;

using Microsoft.AspNetCore.Identity;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Contracts;
using Yggdrasil.Application.Contracts.Authentication;
using Yggdrasil.Domain.Constants;

namespace Yggdrasil.Infrastructure.Identity;

public class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    public async Task<UserResponse?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByEmailAsync(email);

        return user is null ? null : await ToResponseAsync(user);
    }

    public async Task<CreateUserResult> CreateUserAsync(RegisterRequest registerRequest, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = registerRequest.Email,
            UserName = registerRequest.UserName
        };
        var result = await userManager.CreateAsync(user, registerRequest.Password);
        
        if(result.Succeeded)
            await userManager.AddToRolesAsync(user, [Roles.User]);

        return result.Succeeded
            ? new CreateUserResult(Success: true, User: await ToResponseAsync(user), Errors: [])
            : new CreateUserResult(
                Success: false,
                User: null,
                Errors: result.Errors.Select(e =>
                    new CreateUserResult.Error(e.Code, e.Description)).ToArray());
    }

    public async Task<bool> CheckPasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByIdAsync(userId.ToString());

        return user is not null && await userManager.CheckPasswordAsync(user, password);
    }

    // Private helper
    private async Task<UserResponse> ToResponseAsync(ApplicationUser user)
        => new(Id: user.Id, Email: user.Email!, UserName: user.UserName!, Roles:
            [.. await userManager.GetRolesAsync(user)]
        );
}
