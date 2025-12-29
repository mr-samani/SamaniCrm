using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core.Shared.Interfaces;

namespace SamaniCrm.Application.DashboardManager;

public record GetAllDashboardItemsQuery(Guid DashboardId) : IRequest<List<DashboardItemDto>>;




public class GetAllDashboardItemsQueryHandler : IRequestHandler<GetAllDashboardItemsQuery, List<DashboardItemDto>>
{
    private readonly ILocalizer L;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllDashboardItemsQueryHandler(
        ILocalizer l,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        L = l;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<DashboardItemDto>> Handle(GetAllDashboardItemsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new AccessDeniedException();
        }
        var userId = Guid.Parse(_currentUser.UserId);

        var result = await _context.DashboardItems
               .AsNoTracking()
               .Where(w => w.DashboardId == request.DashboardId && (w.Dashboard.UserId == userId || w.Dashboard.IsPublic))
               .Select(s => new DashboardItemDto()
               {
                   Id = s.Id,
                   ComponentName = s.ComponentName,
                   Position = s.Position,
                   Data = s.Data,
                   DashboardId = s.DashboardId
               })
               .ToListAsync(cancellationToken);
        return result;
    }
}
