using MediatR;
using Microsoft.AspNetCore.SignalR;
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

        public Guid UserId { get; set; } = Guid.Parse("3e454814-1ed0-498c-02ae-08dda1200962");
        public string Message { get; set; } = "این بک پیام تستی می باشد ";
    }

    public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Unit>
    {
        private readonly INotificationHubService _hubService;
        private readonly IApplicationDbContext _dbContext;
        public SendNotificationCommandHandler(INotificationHubService hubService, IApplicationDbContext dbContext)
        {
            _hubService = hubService;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            var notify = new Notification()
            {
                RecieverUserId = request.UserId,
                Title = request.Message,
                Type = NotificationTypeEnum.Message,
                Periority = NotificationPeriorityEnum.Info,
            };
            await _dbContext.Notifications.AddAsync(notify, cancellationToken);
            await _dbContext.SaveChangesAsync();

            var notifyDto = new NotificationDto()
            {
                Id = notify.Id,
                Title = notify.Title,
                Type = notify.Type,
                Periority = notify.Periority,
                RecieverUserId = notify.RecieverUserId,
                CreationTime = DateTime.UtcNow,
            };
            await _hubService.SendToUserAsync(request.UserId, notifyDto);
            return Unit.Value;
        }
    }


}
