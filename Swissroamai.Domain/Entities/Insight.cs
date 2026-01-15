namespace Swissroamai.Domain.Entities;

public sealed record Insight(
    Guid Id,
    Guid SessionId,
    string Title,
    string Description,
    string Category,
    double Score,
    string Freshness,
    IReadOnlyList<SourceAttribution> Sources,
    DateTimeOffset GeneratedAt);
