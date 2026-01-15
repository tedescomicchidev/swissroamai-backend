namespace Swissroamai.Domain.Entities;

public sealed record LocationPing(
    Guid Id,
    Guid SessionId,
    double Latitude,
    double Longitude,
    double AccuracyMeters,
    DateTimeOffset OccurredAt,
    DateTimeOffset ReceivedAt,
    string DedupeKey);
