using Swissroamai.Domain.Entities;

namespace Swissroamai.Api.Contracts;

public sealed record ItineraryRequest(DateOnly TargetDate);

public sealed record ItineraryResponse(
    Guid Id,
    Guid SessionId,
    DateOnly TargetDate,
    IReadOnlyList<ItineraryStop> Stops,
    DateTimeOffset GeneratedAt)
{
    public static ItineraryResponse FromDomain(ItineraryPlan plan) =>
        new(plan.Id, plan.SessionId, plan.TargetDate, plan.Stops, plan.GeneratedAt);
}
