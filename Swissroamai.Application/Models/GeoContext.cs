namespace Swissroamai.Application.Models;

public sealed record GeoContext(
    string Region,
    string Canton,
    string? City,
    string? CountryCode);
