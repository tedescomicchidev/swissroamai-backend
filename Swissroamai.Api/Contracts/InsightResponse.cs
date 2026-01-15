using Swissroamai.Domain.Entities;

namespace Swissroamai.Api.Contracts;

public sealed record InsightResponse(
    Guid Id,
    string Title,
    string Description,
    string Category,
    double Score,
    string Freshness,
    IReadOnlyList<SourceAttribution> Sources,
    DateTimeOffset GeneratedAt)
{
    public static InsightResponse FromDomain(Insight insight) =>
        new(
            insight.Id,
            insight.Title,
            insight.Description,
            insight.Category,
            insight.Score,
            insight.Freshness,
            insight.Sources,
            insight.GeneratedAt);
}

public sealed record InsightFeedResponse(
    Guid SessionId,
    IReadOnlyList<InsightResponse> Insights,
    DateTimeOffset GeneratedAt);
