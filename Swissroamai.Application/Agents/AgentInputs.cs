using Swissroamai.Application.Models;
using Swissroamai.Domain.Entities;

namespace Swissroamai.Application.Agents;

public sealed record GeoContextInput(LocationPing Ping);

public sealed record EventsRetrievalInput(LocationPing Ping, GeoContext GeoContext);

public sealed record LocalFactsInput(LocationPing Ping, GeoContext GeoContext);

public sealed record RankingInput(
    LocationPing Ping,
    GeoContext GeoContext,
    IReadOnlyList<EventListing> Events,
    IReadOnlyList<LocalFact> LocalFacts);

public sealed record ItineraryInput(
    Guid SessionId,
    DateOnly TargetDate,
    IReadOnlyList<Insight> SavedInsights);
