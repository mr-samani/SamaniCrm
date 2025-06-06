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


    public record class MarkAsReadCommand(Guid Id) : IRequest<bool>;


    public class MarkAsReadCommanddHandler : IRequestHandler<MarkAsReadCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public MarkAsReadCommanddHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _dbContext = context;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            Guid.TryParse(_currentUserService.UserId, out var currentUserId);
            var entity = await _dbContext.Notifications.Where(x=>x.Id == request.Id && x.RecieverUserId == currentUserId).FirstOrDefaultAsync(cancellationToken);
            if (entity == null)
                throw new NotFoundException("Notification not found.");

            entity.Read = true;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }





}
