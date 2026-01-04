using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities.Dashboard;

namespace SamaniCrm.Application.DashboardManager
{

    public class CreateOrUpdateDashboardCommand : DashboardDto, IRequest<bool>;

    public class CreateOrUpdateDashboardCommandHandler : IRequestHandler<CreateOrUpdateDashboardCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUser;

        public CreateOrUpdateDashboardCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
        {
            this._dbContext = dbContext;
            _currentUser = currentUser;
        }



        public async Task<bool> Handle(CreateOrUpdateDashboardCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_currentUser.UserId))
            {
                throw new AccessDeniedException();
            }
            Dashboard? found = null;

            if (request.Id != null)
            {
                found = await _dbContext.Dashboards.FindAsync(new object[] { request.Id }, cancellationToken);
            }
            if (found == null)
            {
                var userId = Guid.Parse(_currentUser.UserId);
                var newItem = new Dashboard
                {
                    Title = request.Title,
                    IsPublic = request.IsPublic,
                    Order = request.Order,
                    UserId = userId,
                };

                var r = await _dbContext.Dashboards.AddAsync(newItem, cancellationToken);
                found = r.Entity;
            }
            else
            {
                found.Title = request.Title;
                found.IsPublic = request.IsPublic;
                found.Order = request.Order;
            }
            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result > 0;
        }

    }
}
