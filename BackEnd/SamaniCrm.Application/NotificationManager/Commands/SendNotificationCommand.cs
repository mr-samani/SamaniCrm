using MediatR;
using Microsoft.AspNetCore.SignalR;
using SamaniCrm.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.NotificationManager.Commands
{
    public class SendNotificationCommand:IRequest<Unit>
    {
        public Guid UserId { get; set; }
        public string Message { get; set; }
    }

    public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Unit>
    {
        private readonly INotificationHubService _hubService;

        public SendNotificationCommandHandler(INotificationHubService hubService)
        {
            _hubService = hubService;
        }

        public async Task<Unit> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            await _hubService.SendToUserAsync(request.UserId, request.Message);
            return Unit.Value;
        }
    }


}
