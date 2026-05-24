using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.NotificationManager.Queries
{
    public class GetLastUnReadNotificationsQuery : IRequest<UnReadNotificationListDto>
    {
    }



    public class GetLastUnReadNotificationsQueryHandler : IRequestHandler<GetLastUnReadNotificationsQuery, UnReadNotificationListDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUser;


        public GetLastUnReadNotificationsQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<UnReadNotificationListDto> Handle(GetLastUnReadNotificationsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;

            var result = await _dbContext.Notifications
                                    .AsNoTracking()
                                    .Where(x => x.Read == false)
                                    .Where(x => x.RecieverUserId == currentUserId)
                                    .OrderByDescending(x => x.CreatedAt)
                                    .Skip(0)
                                    .Take(10)
                                     .Select(s => new NotificationDto()
                                     {
                                         Id = s.Id,
                                         Title = s.Title,
                                         Data = s.Data,
                                         Type = s.Type,
                                         Periority = s.Periority,
                                         Read = s.Read,
                                         CreationTime = s.CreatedAt,
                                     })
                                    .ToListAsync(cancellationToken);
            int unreadCount = await _dbContext.Notifications
                                    .AsNoTracking()
                                    .Where(x => x.Read == false)
                                    .Where(x => x.RecieverUserId == currentUserId)
                                    .CountAsync(cancellationToken);

            return new UnReadNotificationListDto()
            {
                items = result,
                UnreadCount = unreadCount
            };
        }
    }
}
