using Microsoft.Extensions.DependencyInjection;
using Swissroamai.Application.Interfaces;
using Swissroamai.Application.Services;

namespace Swissroamai.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IInsightService, InsightService>();
        services.AddScoped<IItineraryService, ItineraryService>();
        return services;
    }
}
