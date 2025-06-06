using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.NotificationManager.Queries
{
    public record GetNotificationInfoQuery(Guid Id) : IRequest<NotificationDto>;


    public class GetNotificationInfoQueryHandler : IRequestHandler<GetNotificationInfoQuery, NotificationDto>
    {
        private readonly INotificationService _notificationService;
        private readonly ICurrentUserService _currentUserService;

        public GetNotificationInfoQueryHandler(INotificationService notificationService, ICurrentUserService currentUserService)
        {
            _notificationService = notificationService;
            _currentUserService = currentUserService;
        }

        public async Task<NotificationDto> Handle(GetNotificationInfoQuery request, CancellationToken cancellationToken)
        {
            Guid.TryParse(_currentUserService.UserId, out var currentUserId);

            var result = await _notificationService.GetNotification(request.Id, currentUserId, cancellationToken);
            return result;
        }
    }

}
