namespace Swissroamai.Domain.Entities;

public sealed record SourceAttribution(
    string Name,
    string? Url,
    DateTimeOffset RetrievedAt,
    double Confidence);
