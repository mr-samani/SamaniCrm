using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities.Dashboard;

namespace SamaniCrm.Application.DashboardManager
{

    public record UpdateDashboardItemsCommand(List<DashboardItemDto> list) : IRequest<bool>;

    public class UpdateDashboardItemsCommandHandler : IRequestHandler<UpdateDashboardItemsCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUser;

        public UpdateDashboardItemsCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
        {
            this._dbContext = dbContext;
            _currentUser = currentUser;
        }



        public async Task<bool> Handle(UpdateDashboardItemsCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_currentUser.UserId))
            {
                throw new AccessDeniedException();
            }
            List<DashboardItem> range = [];
            foreach (var item in request.list)
            {
                DashboardItem? found = await _dbContext.DashboardItems.FindAsync(new object[] { item.Id! }, cancellationToken);
                if (found != null)
                {
                    found.ComponentName = item.ComponentName;
                    found.DashboardId = item.DashboardId;
                    found.Data = item.Data;
                    found.Position = item.Position;
                    range.Add(found);
                }
            }
            if (range.Count > 0)
            {
                _dbContext.DashboardItems.UpdateRange(range);
            }

            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result > 0;
        }

    }
}
