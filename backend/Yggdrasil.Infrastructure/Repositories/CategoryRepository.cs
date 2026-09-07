using Microsoft.EntityFrameworkCore;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Domain.Entities;
using Yggdrasil.Infrastructure.Persistence;

namespace Yggdrasil.Infrastructure.Repositories;

public class CategoryRepository(YggdrasilDbContext dbContext) : ICategoryRepository
{
    private readonly YggdrasilDbContext _dbContext = dbContext;

    public async Task<List<Category>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .Where(c => ids.Contains(c.Id))
            .ToListAsync(cancellationToken);
    }
}
