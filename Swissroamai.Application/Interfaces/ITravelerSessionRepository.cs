using Swissroamai.Domain.Entities;

namespace Swissroamai.Application.Interfaces;

public interface ITravelerSessionRepository
{
    Task<TravelerSession> GetOrCreateAsync(Guid sessionId, string? travelerAlias, CancellationToken cancellationToken);
    Task<TravelerSession?> GetAsync(Guid sessionId, CancellationToken cancellationToken);
    Task UpdateAsync(TravelerSession session, CancellationToken cancellationToken);
}
