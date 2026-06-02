using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Features.Logging.Queries;
using SamaniCrm.Core.Shared.Logging.Dtos;

namespace SamaniCrm.Application.Features.Logging.Interfaces;


public interface ISecurityLogService
{
    Task<PaginatedResult<SecurityLogDto>> GetSecurityLogs(GetSecurityLogsQuery filter, CancellationToken cancellation);
    Task<List<LastLoginDto>> GetLastLoginInfo(GetLastLoginInfoQuery request, CancellationToken cancellation);
}
