using Swissroamai.Domain.Entities;

namespace Swissroamai.Application.Models;

public sealed record InsightFeed(
    Guid SessionId,
    IReadOnlyList<Insight> Insights,
    DateTimeOffset GeneratedAt);
