using Swissroamai.Application.Agents;
using Swissroamai.Application.Interfaces;
using Swissroamai.Domain.Entities;

namespace Swissroamai.Application.Services;

public sealed class ItineraryService(
    ISavedItemRepository savedItemRepository,
    IAgent<ItineraryInput, ItineraryPlan> itineraryAgent)
    : IItineraryService
{
    public async Task<ItineraryPlan> BuildItineraryAsync(Guid sessionId, DateOnly targetDate, CancellationToken cancellationToken)
    {
        var savedItems = await savedItemRepository.ListBySessionAsync(sessionId, cancellationToken);
        var insights = savedItems
            .Select(item => new Insight(
                item.InsightId,
                item.SessionId,
                $"Saved insight {item.InsightId}",
                item.Note,
                "Saved",
                0.5,
                "Saved",
                Array.Empty<SourceAttribution>(),
                item.SavedAt))
            .ToList();

        var result = await itineraryAgent.ExecuteAsync(
            new ItineraryInput(sessionId, targetDate, insights),
            cancellationToken);

        return result.Payload;
    }
}
