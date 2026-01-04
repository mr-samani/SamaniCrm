using MediatR;
using Microsoft.AspNetCore.SignalR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.NotificationManager.Commands
{
    public class BroadCastNotificationsCommand : IRequest<long>
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public NotificationPeriorityEnum Periority { get; set; } = NotificationPeriorityEnum.Normal;
    }

    public class BroadCastNotificationsCommandHandler : IRequestHandler<BroadCastNotificationsCommand, long>
    {
        private readonly INotificationHubService _hubService;
        private readonly IApplicationDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;
        public BroadCastNotificationsCommandHandler(
            INotificationHubService hubService,
            IApplicationDbContext dbContext,
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _hubService = hubService;
            _dbContext = dbContext;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<long> Handle(BroadCastNotificationsCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId == null)
            {
                throw new AccessDeniedException();
            }
            var users = await _identityService.GetAllActiveUsersIds(cancellationToken);
            var currentUserId = Guid.Parse(_currentUserService.UserId);
            var count = 0;
            List<Notification> notifyList = [];
            foreach (var userId in users)
            {
                var notify = new Notification()
                {
                    RecieverUserId = userId,
                    Title = request.Title,
                    Content = request.Content,
                    Type = NotificationTypeEnum.Message,
                    Periority = request.Periority,
                    SenderUserId = currentUserId
                };
                notifyList.Add(notify);

                var notifyDto = new NotificationDto()
                {
                    Id = notify.Id,
                    Title = notify.Title,
                    Content = notify.Content,
                    Type = notify.Type,
                    Periority = notify.Periority,
                    RecieverUserId = notify.RecieverUserId,
                    SenderUserId = currentUserId,
                    CreationTime = DateTime.UtcNow,
                };
                await _hubService.SendToUserAsync(userId, notifyDto);
                count++;
            }
            await _dbContext.Notifications.AddRangeAsync(notifyList, cancellationToken);
            await _dbContext.SaveChangesAsync();

            return count;
        }
    }


}
