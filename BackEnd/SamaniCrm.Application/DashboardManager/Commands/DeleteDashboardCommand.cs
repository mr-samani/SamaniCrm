
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.DashboardManager
{

    public record DeleteDashboardCommand(Guid Id) : IRequest<bool>;

    public class DeleteDashboardCommandHandler : IRequestHandler<DeleteDashboardCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUser;

        public DeleteDashboardCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
        {
            this._dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(DeleteDashboardCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_currentUser.UserId))
            {
                throw new AccessDeniedException();
            }
            var userId = Guid.Parse(_currentUser.UserId);

            var entity = await _dbContext.Dashboards.Where(x => x.UserId == userId && x.Id == request.Id).FirstAsync();
            if (entity == null)
                throw new NotFoundException("Dashboard not found.");
            var result = await _dbContext.Dashboards.ExecuteDeleteAsync(cancellationToken);
            return result > 0;
        }
    }
}