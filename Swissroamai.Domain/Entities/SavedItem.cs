namespace Swissroamai.Domain.Entities;

public sealed record SavedItem(
    Guid Id,
    Guid SessionId,
    Guid InsightId,
    string Note,
    DateTimeOffset SavedAt);
