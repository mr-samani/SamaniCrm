using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.NotificationManager.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task<PaginatedResult<NotificationDto>> GetAllNotifications(GetAllNotificationQuery request, Guid userId, CancellationToken cancellationToken);
        Task<NotificationDto> GetNotification(Guid id, Guid userId, CancellationToken cancellationToken);
    }
}
