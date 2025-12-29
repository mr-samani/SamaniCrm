
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.DashboardManager
{

    public record DeleteDashboardItemCommand(Guid Id) : IRequest<bool>;

    public class DeleteDashboardItemCommandHandler : IRequestHandler<DeleteDashboardItemCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUser;

        public DeleteDashboardItemCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
        {
            this._dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(DeleteDashboardItemCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_currentUser.UserId))
            {
                throw new AccessDeniedException();
            }
            var userId = Guid.Parse(_currentUser.UserId);

            await _dbContext.DashboardItems.Where(
                x => x.Dashboard.UserId == userId &&
                x.Id == request.Id).ExecuteDeleteAsync();

            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}