using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core.Shared.Interfaces;

namespace SamaniCrm.Application.DashboardManager;

public record GetAllDashboardsQuery() : IRequest<List<DashboardDto>>;




public class GetAllDashboardsQueryHandler : IRequestHandler<GetAllDashboardsQuery, List<DashboardDto>>
{
    private readonly ILocalizer L;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllDashboardsQueryHandler(
        ILocalizer l,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        L = l;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<DashboardDto>> Handle(GetAllDashboardsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new AccessDeniedException();
        }
        var userId = Guid.Parse(_currentUser.UserId);

        var result = await _context.Dashboards
               .AsNoTracking()
               .Where(w => w.UserId == userId || w.IsPublic)
               .Select(s => new DashboardDto()
               {
                   Id = s.Id,
                   UserId=s.UserId,
                   Title = s.Title,
                   IsPublic = s.IsPublic,
                   Order = s.Order
               })
               .OrderBy(x => x.Order)
               .ToListAsync(cancellationToken);
        return result;
    }
}
