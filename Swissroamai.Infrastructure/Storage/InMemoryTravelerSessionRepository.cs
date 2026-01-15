using System.Collections.Concurrent;
using Swissroamai.Application.Interfaces;
using Swissroamai.Domain.Entities;

namespace Swissroamai.Infrastructure.Storage;

public sealed class InMemoryTravelerSessionRepository : ITravelerSessionRepository
{
    private readonly ConcurrentDictionary<Guid, TravelerSession> _sessions = new();
    private readonly TimeSpan _defaultTtl = TimeSpan.FromHours(4);

    public Task<TravelerSession> GetOrCreateAsync(Guid sessionId, string? travelerAlias, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var session = _sessions.GetOrAdd(sessionId, id =>
            new TravelerSession(id, travelerAlias, now, now.Add(_defaultTtl), true));

        return Task.FromResult(session);
    }

    public Task<TravelerSession?> GetAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        _sessions.TryGetValue(sessionId, out var session);
        return Task.FromResult(session);
    }

    public Task UpdateAsync(TravelerSession session, CancellationToken cancellationToken)
    {
        _sessions[session.Id] = session;
        return Task.CompletedTask;
    }
}
