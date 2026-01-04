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
    public class SendNotificationCommand : IRequest<Unit>
    {

        public required Guid UserId { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public NotificationPeriorityEnum Periority { get; set; } = NotificationPeriorityEnum.Normal;
    }

    public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Unit>
    {
        private readonly INotificationHubService _hubService;
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public SendNotificationCommandHandler(INotificationHubService hubService, IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _hubService = hubService;
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId == null)
            {
                throw new AccessDeniedException();
            }

            var currentUserId = Guid.Parse(_currentUserService.UserId);
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
            await _hubService.SendToUserAsync(request.UserId, notifyDto);
            return Unit.Value;
        }
    }


}
