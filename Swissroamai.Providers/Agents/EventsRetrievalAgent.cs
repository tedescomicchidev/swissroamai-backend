using Swissroamai.Application.Agents;
using Swissroamai.Application.Models;

namespace Swissroamai.Providers.Agents;

public sealed class EventsRetrievalAgent : IAgent<EventsRetrievalInput, IReadOnlyList<EventListing>>
{
    public Task<AgentResult<IReadOnlyList<EventListing>>> ExecuteAsync(
        EventsRetrievalInput input,
        CancellationToken cancellationToken)
    {
        var events = new List<EventListing>
        {
            new(
                $"{input.GeoContext.City} Night Market",
                "Local artisan stalls and food vendors.",
                DateOnly.FromDateTime(DateTime.UtcNow),
                "Old Town",
                "Event"),
            new(
                $"{input.GeoContext.Region} Panorama Walk",
                "Guided walk with scenic viewpoints.",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                "Lakeside",
                "Tour")
        };

        var citations = new AgentCitations(["MockEventsProvider"]);
        return Task.FromResult(new AgentResult<IReadOnlyList<EventListing>>(events, 0.68, citations));
    }
}
