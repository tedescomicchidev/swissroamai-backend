using Swissroamai.Domain.Entities;

namespace Swissroamai.Application.Interfaces;

public interface IItineraryService
{
    Task<ItineraryPlan> BuildItineraryAsync(Guid sessionId, DateOnly targetDate, CancellationToken cancellationToken);
}
