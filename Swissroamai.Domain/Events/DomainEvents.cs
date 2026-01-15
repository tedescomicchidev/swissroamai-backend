namespace Swissroamai.Domain.Events;

public sealed record LocationReceived(Guid SessionId, Guid LocationPingId, DateTimeOffset ReceivedAt);

public sealed record InsightsComputed(Guid SessionId, int InsightCount, DateTimeOffset ComputedAt);

public sealed record ItemSaved(Guid SessionId, Guid SavedItemId, DateTimeOffset SavedAt);

public sealed record ItineraryRequested(Guid SessionId, DateOnly TargetDate, DateTimeOffset RequestedAt);
