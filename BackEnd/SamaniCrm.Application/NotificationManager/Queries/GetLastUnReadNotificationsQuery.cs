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
        private readonly ICurrentUserService _currentUserService;


        public GetLastUnReadNotificationsQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<List<NotificationDto>> Handle(GetLastUnReadNotificationsQuery request, CancellationToken cancellationToken)
        {
            Guid.TryParse(_currentUserService.UserId, out var currentUserId);

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
                                        CreationTime = s.CreationTime.ToUniversalTime(),
                                    })
                                    .Skip(0)
                                    .Take(10)
                                    .ToListAsync(cancellationToken);
            return result;
        }
    }
}
