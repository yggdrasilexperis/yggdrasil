namespace Yggdrasil.Application.Abstractions;

public interface IIdentityService
{
    Task<UserAccount?> FindByEmailAsync(
        string email, CancellationToken cancellationToken);
    
    Task<CreateUserResult> CreateUserAsync(
        string email, string userName, string password, CancellationToken cancellationToken);
    
    Task<bool> CheckPasswordAsync(Guid userId, string password, CancellationToken cancellationToken);
}
