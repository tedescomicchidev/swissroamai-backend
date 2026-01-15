namespace Swissroamai.Api.Contracts;

public sealed record SavedItemRequest(
    Guid InsightId,
    string Note);
