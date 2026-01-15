using Microsoft.Extensions.DependencyInjection;
using Swissroamai.Application.Interfaces;
using Swissroamai.Infrastructure.Storage;

namespace Swissroamai.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ITravelerSessionRepository, InMemoryTravelerSessionRepository>();
        services.AddSingleton<ILocationPingDeduplicator, InMemoryLocationPingDeduplicator>();
        services.AddSingleton<ISavedItemRepository, InMemorySavedItemRepository>();
        return services;
    }
}
