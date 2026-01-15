namespace Swissroamai.Domain.Entities;

public sealed record ItineraryPlan(
    Guid Id,
    Guid SessionId,
    DateOnly TargetDate,
    IReadOnlyList<ItineraryStop> Stops,
    DateTimeOffset GeneratedAt);

public sealed record ItineraryStop(
    string Title,
    string Description,
    string? LocationHint,
    string TimeWindow);
