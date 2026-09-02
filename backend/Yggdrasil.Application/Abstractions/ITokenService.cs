namespace Yggdrasil.Application.Abstractions;

public interface ITokenService
{
    AccessToken CreateAccessToken(UserAccount userAccount);
}
