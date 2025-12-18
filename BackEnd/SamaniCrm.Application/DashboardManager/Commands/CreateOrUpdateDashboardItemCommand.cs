using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities.Dashboard;

namespace SamaniCrm.Application.DashboardManager
{

    public class CreateOrUpdateDashboardItemCommand : DashboardItemDto, IRequest<bool>;

    public class CreateOrUpdateDashboardItemCommandHandler : IRequestHandler<CreateOrUpdateDashboardItemCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUser;

        public CreateOrUpdateDashboardItemCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
        {
            this._dbContext = dbContext;
            _currentUser = currentUser;
        }



        public async Task<bool> Handle(CreateOrUpdateDashboardItemCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_currentUser.UserId))
            {
                throw new AccessDeniedException();
            }
            DashboardItem? found = null;

            if (request.Id != null)
            {
                found = await _dbContext.DashboardItems.FindAsync(new object[] { request.Id }, cancellationToken);
            }
            if (found == null)
            {
                var newItem = new DashboardItem
                {
                    ComponentName = request.ComponentName,
                    DashboardId = request.DashboardId,
                    Data = request.Data,
                };

                var r = await _dbContext.DashboardItems.AddAsync(newItem, cancellationToken);
                found = r.Entity;
            }
            else
            {
                found.ComponentName = request.ComponentName;
                found.DashboardId = request.DashboardId;
                found.Data = request.Data;
            }
            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result > 0;
        }

    }
}
