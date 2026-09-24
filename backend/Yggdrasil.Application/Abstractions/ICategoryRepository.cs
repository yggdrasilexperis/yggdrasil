using Yggdrasil.Domain.Entities;

namespace Yggdrasil.Application.Abstractions;

public interface ICategoryRepository
{
    Task<List<Category>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken);
    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken);
}
