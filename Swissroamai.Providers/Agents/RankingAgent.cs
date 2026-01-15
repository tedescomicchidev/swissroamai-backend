using Swissroamai.Application.Agents;
using Swissroamai.Domain.Entities;

namespace Swissroamai.Providers.Agents;

public sealed class RankingAgent : IAgent<RankingInput, IReadOnlyList<Insight>>
{
    public Task<AgentResult<IReadOnlyList<Insight>>> ExecuteAsync(
        RankingInput input,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var insights = new List<Insight>();

        var baseScore = 0.85;
        insights.AddRange(input.Events.Select((item, index) =>
            new Insight(
                Guid.NewGuid(),
                input.Ping.SessionId,
                item.Title,
                item.Description,
                item.Category,
                baseScore - index * 0.05,
                "Today",
                Array.Empty<SourceAttribution>(),
                now)));

        insights.AddRange(input.LocalFacts.Select((fact, index) =>
            new Insight(
                Guid.NewGuid(),
                input.Ping.SessionId,
                fact.Title,
                fact.Detail,
                fact.Category,
                0.6 - index * 0.05,
                "Static",
                Array.Empty<SourceAttribution>(),
                now)));

        var citations = new AgentCitations(["DeterministicRanker"]);
        return Task.FromResult(new AgentResult<IReadOnlyList<Insight>>(insights, 0.7, citations));
    }
}
