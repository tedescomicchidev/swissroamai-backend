using Swissroamai.Domain.Entities;

namespace Swissroamai.Application.Interfaces;

public interface ILocationPingDeduplicator
{
    bool IsDuplicate(LocationPing ping);
}
