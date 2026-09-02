using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Contracts.Authentication;
using Yggdrasil.Application.Options;

namespace Yggdrasil.Infrastructure.Identity;

public class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _jwtOptions = options.Value;
    public AccessToken CreateAccessToken(UserResponse userAccount)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationInMinutes);
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = expiresAt,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, userAccount.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userAccount.Email),
                new Claim(JwtRegisteredClaimNames.Name, userAccount.UserName)
            ]),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.IssuerSigningKey)),
                SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        return new AccessToken(token, expiresAt);
    }
}
