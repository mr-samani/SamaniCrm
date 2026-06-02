using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Logging.Queries;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Dynamic.Core;
using SamaniCrm.Application.Features.Logging.Interfaces;

namespace SamaniCrm.Infrastructure.Loging.SecurityLogs;

public class SecurityLogService : ISecurityLogService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public SecurityLogService(IApplicationDbContext dbContext,
        ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<PaginatedResult<SecurityLogDto>> GetSecurityLogs(GetSecurityLogsQuery filter, CancellationToken cancellation)
    {
        var baseQuery = _dbContext.SecurityLogEntries
            .AsNoTracking();

        if (filter.TenantId.HasValue && _currentUser.IsHost)
        {
            baseQuery = baseQuery
                 .IgnoreQueryFilters()
                 .Where(l => l.TenantId == filter.TenantId);
        }



        if (filter.Severity != null)
            baseQuery = baseQuery.Where(l => l.Severity == filter.Severity);
        if (string.IsNullOrEmpty(filter.CorrelationId) == false)
            baseQuery = baseQuery.Where(l => l.CorrelationId == filter.CorrelationId);
        if (filter.FromDate.HasValue)
            baseQuery = baseQuery.Where(l => l.CreatedAt >= filter.FromDate.Value);
        if (filter.ToDate.HasValue)
            baseQuery = baseQuery.Where(l => l.CreatedAt <= filter.ToDate.Value);
        if (filter.UserId.HasValue)
            baseQuery = baseQuery.Where(l => l.UserId == filter.UserId.Value);
        if (!string.IsNullOrEmpty(filter.Search))
            baseQuery = baseQuery.Where(l =>
                                        (!string.IsNullOrWhiteSpace(l.Message) && l.Message.Contains(filter.Search)) ||
                                        (!string.IsNullOrWhiteSpace(l.UserAgent) && l.UserAgent.Contains(filter.Search)) ||
                                        (!string.IsNullOrWhiteSpace(l.Username) && l.Username.Contains(filter.Search)) ||
                                        (!string.IsNullOrWhiteSpace(l.IpAddress) && l.IpAddress.Contains(filter.Search)) ||
                                        (!string.IsNullOrWhiteSpace(l.CorrelationId) && l.CorrelationId.Contains(filter.Search)) ||
                                        (!string.IsNullOrWhiteSpace(l.Action) && l.Action.Contains(filter.Search))
                                         );



        // گروه‌بندی بر اساس CorrelationId و محاسبه Duration
        var groupedQuery = baseQuery
            .Select(s => new SecurityLogDto
            {
                CorrelationId = s.CorrelationId,
                Action = s.Action,
                CreatedAt = s.CreatedAt,
                CreatedBy = s.CreatedBy,
                EventType = s.EventType,
                Id = s.Id,
                IpAddress = s.IpAddress,
                IsSuccessful = s.IsSuccessful,
                Message = s.Message,
                Severity = s.Severity,
                Resource = s.Resource,
                StatusCode = s.StatusCode,
                TenantId = s.TenantId,
                UserAgent = s.UserAgent,
                UserId = s.UserId,
                Username = s.Username

            });

        var total = await groupedQuery.CountAsync(cancellation);


        // Sorting
        if (!string.IsNullOrEmpty(filter.SortBy))
        {
            var sortString = $"Start{filter.SortBy} {filter.SortDirection}";
            groupedQuery = groupedQuery.OrderBy(sortString);
        }
        else
        {
            groupedQuery.OrderByDescending(x => x.CreatedAt);
        }


        // صفحه‌بندی
        var items = await groupedQuery
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellation);


        return new PaginatedResult<SecurityLogDto>(items, total, filter.PageNumber, filter.PageSize);

    }

}
