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
    public class GetLastUnReadNotificationsQuery : IRequest<List<NotificationDto>>
    {
    }



    public class GetLastUnReadNotificationsQueryHandler : IRequestHandler<GetLastUnReadNotificationsQuery, List<NotificationDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUser;


        public GetLastUnReadNotificationsQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<List<NotificationDto>> Handle(GetLastUnReadNotificationsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;

            var result = await _dbContext.Notifications
                                    .Where(x => x.Read == false)
                                    .Where(x => x.RecieverUserId == currentUserId)
                                    .Select(s => new NotificationDto()
                                    {
                                        Id = s.Id,
                                        Title = s.Title,
                                        Data = s.Data,
                                        Type = s.Type,
                                        Periority = s.Periority,
                                        Read = s.Read,
                                        CreationTime = s.CreatedAt.ToUniversalTime(),
                                    })
                                    .Skip(0)
                                    .Take(10)
                                    .ToListAsync(cancellationToken);
            return result;
        }
    }
}
