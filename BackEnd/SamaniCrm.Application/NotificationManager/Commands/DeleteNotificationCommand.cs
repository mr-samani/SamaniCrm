using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.NotificationManager.Commands
{
    public record class DeleteNotificationCommand(Guid Id):IRequest<bool>;


    public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public DeleteNotificationCommandHandler(IApplicationDbContext context)
        {
            _dbContext = context;
        }

        public async Task<bool> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Notifications.FindAsync(request.Id);
            if (entity == null)
                throw new NotFoundException("Notification not found."); 

            entity.IsDeleted = true;
            entity.DeletedTime = DateTime.UtcNow;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }

}
