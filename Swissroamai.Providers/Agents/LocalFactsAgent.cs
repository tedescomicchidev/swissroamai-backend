using Swissroamai.Application.Agents;
using Swissroamai.Application.Models;

namespace Swissroamai.Providers.Agents;

public sealed class LocalFactsAgent : IAgent<LocalFactsInput, IReadOnlyList<LocalFact>>
{
    public Task<AgentResult<IReadOnlyList<LocalFact>>> ExecuteAsync(
        LocalFactsInput input,
        CancellationToken cancellationToken)
    {
        var facts = new List<LocalFact>
        {
            new("Transit Tip", "Validate tickets before boarding trams.", "Transit"),
            new("Nice-to-know", "Shops close early on Sundays.", "Culture"),
            new("POI", $"{input.GeoContext.City} riverside trail", "POI")
        };

        var citations = new AgentCitations(["MockLocalFactsProvider"]);
        return Task.FromResult(new AgentResult<IReadOnlyList<LocalFact>>(facts, 0.64, citations));
    }
}
