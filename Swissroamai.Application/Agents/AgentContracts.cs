namespace Swissroamai.Application.Agents;

public sealed record AgentCitations(IReadOnlyList<string> Sources);

public sealed record AgentResult<T>(
    T Payload,
    double Confidence,
    AgentCitations Citations);

public interface IAgent<in TInput, TOutput>
{
    Task<AgentResult<TOutput>> ExecuteAsync(TInput input, CancellationToken cancellationToken);
}
