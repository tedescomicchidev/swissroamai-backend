using Swissroamai.Application.Models;
using Swissroamai.Domain.Entities;

namespace Swissroamai.Application.Interfaces;

public interface IInsightService
{
    Task<InsightFeed> GenerateInsightsAsync(LocationPing ping, CancellationToken cancellationToken);
}
