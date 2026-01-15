using Swissroamai.Application.Agents;
using Swissroamai.Application.Interfaces;
using Swissroamai.Application.Models;
using Swissroamai.Domain.Entities;

namespace Swissroamai.Application.Services;

public sealed class InsightService(
    IAgent<GeoContextInput, GeoContext> geoContextAgent,
    IAgent<EventsRetrievalInput, IReadOnlyList<EventListing>> eventsAgent,
    IAgent<LocalFactsInput, IReadOnlyList<LocalFact>> localFactsAgent,
    IAgent<RankingInput, IReadOnlyList<Insight>> rankingAgent)
    : IInsightService
{
    public async Task<InsightFeed> GenerateInsightsAsync(LocationPing ping, CancellationToken cancellationToken)
    {
        var geoContextResult = await geoContextAgent.ExecuteAsync(new GeoContextInput(ping), cancellationToken);
        var geoContext = geoContextResult.Payload;

        var eventsResult = await eventsAgent.ExecuteAsync(new EventsRetrievalInput(ping, geoContext), cancellationToken);
        var factsResult = await localFactsAgent.ExecuteAsync(new LocalFactsInput(ping, geoContext), cancellationToken);

        var rankingResult = await rankingAgent.ExecuteAsync(
            new RankingInput(ping, geoContext, eventsResult.Payload, factsResult.Payload),
            cancellationToken);

        var insights = rankingResult.Payload
            .Select(insight => insight with
            {
                Sources = insight.Sources
                    .Concat(BuildAgentSources(geoContextResult, eventsResult, factsResult, rankingResult))
                    .ToList()
            })
            .ToList();

        return new InsightFeed(ping.SessionId, insights, DateTimeOffset.UtcNow);
    }

    private static IEnumerable<SourceAttribution> BuildAgentSources(
        AgentResult<GeoContext> geoContext,
        AgentResult<IReadOnlyList<EventListing>> events,
        AgentResult<IReadOnlyList<LocalFact>> facts,
        AgentResult<IReadOnlyList<Insight>> ranking)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var source in geoContext.Citations.Sources)
        {
            yield return new SourceAttribution(source, null, now, geoContext.Confidence);
        }

        foreach (var source in events.Citations.Sources)
        {
            yield return new SourceAttribution(source, null, now, events.Confidence);
        }

        foreach (var source in facts.Citations.Sources)
        {
            yield return new SourceAttribution(source, null, now, facts.Confidence);
        }

        foreach (var source in ranking.Citations.Sources)
        {
            yield return new SourceAttribution(source, null, now, ranking.Confidence);
        }
    }
}
