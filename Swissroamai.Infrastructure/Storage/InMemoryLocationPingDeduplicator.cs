using System.Collections.Concurrent;
using Swissroamai.Application.Interfaces;
using Swissroamai.Domain.Entities;

namespace Swissroamai.Infrastructure.Storage;

public sealed class InMemoryLocationPingDeduplicator : ILocationPingDeduplicator
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> _seen = new();
    private readonly TimeSpan _retention = TimeSpan.FromMinutes(30);

    public bool IsDuplicate(LocationPing ping)
    {
        TrimExpired();
        return !_seen.TryAdd(ping.DedupeKey, ping.ReceivedAt);
    }

    private void TrimExpired()
    {
        var cutoff = DateTimeOffset.UtcNow - _retention;
        foreach (var pair in _seen)
        {
            if (pair.Value < cutoff)
            {
                _seen.TryRemove(pair.Key, out _);
            }
        }
    }
}
