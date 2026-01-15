using Swissroamai.Application.Agents;
using Swissroamai.Domain.Entities;

namespace Swissroamai.Providers.Agents;

public sealed class ItineraryAgent : IAgent<ItineraryInput, ItineraryPlan>
{
    public Task<AgentResult<ItineraryPlan>> ExecuteAsync(ItineraryInput input, CancellationToken cancellationToken)
    {
        var stops = input.SavedInsights.Select((insight, index) => new ItineraryStop(
            insight.Title,
            insight.Description,
            "Near your saved spot",
            index switch
            {
                0 => "Morning",
                1 => "Midday",
                _ => "Afternoon"
            })).ToList();

        if (stops.Count == 0)
        {
            stops.Add(new ItineraryStop(
                "Explore a lakeside promenade",
                "A flexible day plan based on your current location.",
                "Lakeside",
                "Anytime"));
        }

        var plan = new ItineraryPlan(
            Guid.NewGuid(),
            input.SessionId,
            input.TargetDate,
            stops,
            DateTimeOffset.UtcNow);

        var citations = new AgentCitations(["MockItineraryComposer"]);
        return Task.FromResult(new AgentResult<ItineraryPlan>(plan, 0.66, citations));
    }
}
