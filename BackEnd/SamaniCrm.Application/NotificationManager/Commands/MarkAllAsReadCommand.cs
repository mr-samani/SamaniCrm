using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.NotificationManager.Commands
{
    public record class MarkAllAsReadCommand() : IRequest<bool>;

    public class MarkAllAsReadCommanddHandler : IRequestHandler<MarkAllAsReadCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        public MarkAllAsReadCommanddHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _dbContext = context;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
        {
            Guid.TryParse(_currentUserService.UserId, out var currentUserId);
            var list = await _dbContext.Notifications.Where(x => x.RecieverUserId == currentUserId && x.Read == false).ToListAsync(cancellationToken);
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    item.Read = true;
                }
            }
            _dbContext.Notifications.UpdateRange(list);
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
