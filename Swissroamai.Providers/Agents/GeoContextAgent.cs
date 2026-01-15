using Swissroamai.Application.Agents;
using Swissroamai.Application.Models;

namespace Swissroamai.Providers.Agents;

public sealed class GeoContextAgent : IAgent<GeoContextInput, GeoContext>
{
    public Task<AgentResult<GeoContext>> ExecuteAsync(GeoContextInput input, CancellationToken cancellationToken)
    {
        var region = input.Ping.Latitude switch
        {
            > 47.2 => "Northern Alps",
            < 46.3 => "Southern Alps",
            _ => "Swiss Plateau"
        };

        var canton = input.Ping.Longitude switch
        {
            > 8.5 => "Graubünden",
            < 6.1 => "Geneva",
            _ => "Zurich"
        };

        var city = canton == "Geneva" ? "Geneva" : canton == "Graubünden" ? "Chur" : "Zurich";

        var context = new GeoContext(region, canton, city, "CH");
        var citations = new AgentCitations(["MockGeoContextProvider"]);

        return Task.FromResult(new AgentResult<GeoContext>(context, 0.72, citations));
    }
}
