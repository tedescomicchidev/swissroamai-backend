using Swissroamai.Domain.Entities;

namespace Swissroamai.Application.Interfaces;

public interface ISavedItemRepository
{
    Task AddAsync(SavedItem item, CancellationToken cancellationToken);
    Task<IReadOnlyList<SavedItem>> ListBySessionAsync(Guid sessionId, CancellationToken cancellationToken);
}
