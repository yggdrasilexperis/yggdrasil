using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

using Yggdrasil.Application.Abstractions;

namespace Yggdrasil.Infrastructure.Identity;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var sub = httpContextAccessor.HttpContext?.User
                .FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.Parse(sub!);
        }
    }
}
