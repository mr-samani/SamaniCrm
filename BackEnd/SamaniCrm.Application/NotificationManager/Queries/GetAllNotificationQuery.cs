using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Queries.User;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using SamaniCrm.Core.Shared.Enums;



namespace SamaniCrm.Application.NotificationManager.Queries
{
    public class GetAllNotificationQuery : PaginationRequest, IRequest<PaginatedResult<NotificationDto>>
    {
        public string? Filter { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<NotificationTypeEnum> Type { get; set; } = [];
        public List<NotificationPeriorityEnum> Periority { get; set; } = []; 
    }

    public class GetAllNotificationQueryHandler : IRequestHandler<GetAllNotificationQuery, PaginatedResult<NotificationDto>>
    {
        private readonly INotificationService _notificationService;
        private readonly ICurrentUserService _currentUser;

        public GetAllNotificationQueryHandler(INotificationService notificationService, ICurrentUserService currentUser)
        {
            _notificationService = notificationService;
            _currentUser = currentUser;
        }



        public async Task<PaginatedResult<NotificationDto>> Handle(GetAllNotificationQuery request, CancellationToken cancellationToken)
        {
            if(_currentUser.UserId == null)
            {
                throw new UnauthorizedAccessException();
            }

            Guid currentUserId = (Guid)_currentUser.UserId!;
            var result = await _notificationService.GetAllNotifications(request, currentUserId, cancellationToken);
            return result;
        }
    }
}
