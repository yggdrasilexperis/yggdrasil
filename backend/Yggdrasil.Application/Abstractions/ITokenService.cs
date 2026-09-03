using Yggdrasil.Application.Contracts;
using Yggdrasil.Application.Contracts.Authentication;

namespace Yggdrasil.Application.Abstractions;

public interface ITokenService
{
    AccessToken CreateAccessToken(UserResponse userAccount);
}
