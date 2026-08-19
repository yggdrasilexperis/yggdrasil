using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Yggdrasil.Infrastructure.Identity;

namespace Yggdrasil.Infrastructure.Persistene;

public sealed class YggdrasilDbContext(DbContextOptions<YggdrasilDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // this needs to be first since it creates the AspNet* tables
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(YggdrasilDbContext).Assembly);
    }
}