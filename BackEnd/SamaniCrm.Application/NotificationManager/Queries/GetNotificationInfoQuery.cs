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
        private readonly ICurrentUserService _currentUser;

        public GetNotificationInfoQueryHandler(INotificationService notificationService, ICurrentUserService currentUser)
        {
            _notificationService = notificationService;
            _currentUser = currentUser;
        }

        public async Task<NotificationDto> Handle(GetNotificationInfoQuery request, CancellationToken cancellationToken)
        {
            var currentUserId =(Guid) _currentUser.UserId!;

            var result = await _notificationService.GetNotification(request.Id, currentUserId, cancellationToken);
            return result;
        }
    }

}
