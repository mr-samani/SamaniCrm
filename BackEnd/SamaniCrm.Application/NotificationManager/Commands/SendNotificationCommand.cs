using MediatR;
using Microsoft.AspNetCore.SignalR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.NotificationManager.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.NotificationManager.Commands
{
    public class SendNotificationCommand : IRequest<Unit>
    {

        public required Guid UserId { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public NotificationPeriorityEnum Periority { get; set; } = NotificationPeriorityEnum.Normal;
    }

    public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Unit>
    {
        private readonly INotificationSender _notificationSender;
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUser;

        public SendNotificationCommandHandler(
            IApplicationDbContext dbContext,
            ICurrentUserService currentUser,
            INotificationSender notificationSender)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
            _notificationSender = notificationSender;
        }

        public async Task<Unit> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
            {
                throw new AccessDeniedException();
            }

            var currentUserId = _currentUser.UserId;
            var notify = new Notification()
            {
                RecieverUserId = request.UserId,
                Title = request.Title,
                Content = request.Content,
                Type = NotificationTypeEnum.Message,
                Periority = request.Periority,
                SenderUserId = currentUserId
            };
            await _dbContext.Notifications.AddAsync(notify, cancellationToken);
            await _dbContext.SaveChangesAsync();

            var notifyDto = new NotificationDto()
            {
                Id = notify.Id,
                Title = notify.Title,
                Type = notify.Type,
                Content = notify.Content,
                Periority = notify.Periority,
                RecieverUserId = notify.RecieverUserId,
                CreationTime = DateTime.UtcNow,
                SenderUserId = currentUserId
            };
            await _notificationSender.SendToUserAsync(request.UserId.ToString(), "ReceiveNotification", notifyDto);
            return Unit.Value;
        }
    }


}
