namespace Swissroamai.Api.Contracts;

public sealed record LocationPingRequest(
    double Latitude,
    double Longitude,
    double AccuracyMeters,
    DateTimeOffset? OccurredAt,
    string? TravelerAlias);
