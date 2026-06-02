using Microsoft.Extensions.Logging;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Logging.Dtos;


namespace SamaniCrm.Core.Shared.Interfaces;

public interface ISecurityLogFactory
{
    SecurityLogDto Create(SecurityEventType eventType, LogLevel sererity,
        bool IsSuccessful, System.Net.HttpStatusCode statusCode,
        string? error = null);
}
