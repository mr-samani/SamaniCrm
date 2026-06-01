using SamaniCrm.Core.Shared.Logging.Dtos;

namespace SamaniCrm.Core.Shared.Interfaces;
public interface ISecurityLogQueue
{
    ValueTask EnqueueAsync(SecurityLogDto log);
}
