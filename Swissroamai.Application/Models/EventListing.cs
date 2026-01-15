namespace Swissroamai.Application.Models;

public sealed record EventListing(
    string Title,
    string Description,
    DateOnly Date,
    string? Venue,
    string Category);
