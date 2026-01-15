namespace Swissroamai.Domain.Entities;

public sealed record TravelerSession(
    Guid Id,
    string? TravelerAlias,
    DateTimeOffset StartedAt,
    DateTimeOffset ExpiresAt,
    bool IsActive)
{
    public static TravelerSession CreateNew(string? travelerAlias, TimeSpan ttl, DateTimeOffset now)
    {
        var id = Guid.NewGuid();
        return new TravelerSession(id, travelerAlias, now, now.Add(ttl), true);
    }
}
