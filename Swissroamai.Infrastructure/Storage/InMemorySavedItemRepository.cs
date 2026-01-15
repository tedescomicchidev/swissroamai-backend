using System.Collections.Concurrent;
using Swissroamai.Application.Interfaces;
using Swissroamai.Domain.Entities;

namespace Swissroamai.Infrastructure.Storage;

public sealed class InMemorySavedItemRepository : ISavedItemRepository
{
    private readonly ConcurrentDictionary<Guid, List<SavedItem>> _items = new();

    public Task AddAsync(SavedItem item, CancellationToken cancellationToken)
    {
        var list = _items.GetOrAdd(item.SessionId, _ => new List<SavedItem>());
        lock (list)
        {
            list.Add(item);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<SavedItem>> ListBySessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        if (_items.TryGetValue(sessionId, out var list))
        {
            lock (list)
            {
                return Task.FromResult<IReadOnlyList<SavedItem>>(list.ToList());
            }
        }

        return Task.FromResult<IReadOnlyList<SavedItem>>(Array.Empty<SavedItem>());
    }
}
