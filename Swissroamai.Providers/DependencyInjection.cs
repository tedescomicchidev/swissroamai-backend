using Microsoft.Extensions.DependencyInjection;
using Swissroamai.Application.Agents;
using Swissroamai.Application.Models;
using Swissroamai.Domain.Entities;
using Swissroamai.Providers.Agents;

namespace Swissroamai.Providers;

public static class DependencyInjection
{
    public static IServiceCollection AddProviders(this IServiceCollection services)
    {
        services.AddSingleton<IAgent<GeoContextInput, GeoContext>, GeoContextAgent>();
        services.AddSingleton<IAgent<EventsRetrievalInput, IReadOnlyList<EventListing>>, EventsRetrievalAgent>();
        services.AddSingleton<IAgent<LocalFactsInput, IReadOnlyList<LocalFact>>, LocalFactsAgent>();
        services.AddSingleton<IAgent<RankingInput, IReadOnlyList<Insight>>, RankingAgent>();
        services.AddSingleton<IAgent<ItineraryInput, ItineraryPlan>, ItineraryAgent>();
        return services;
    }
}
