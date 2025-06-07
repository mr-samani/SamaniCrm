using SamaniCrm.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface INotificationHubService
    {
        Task SendToUserAsync(Guid userId, NotificationDto message);
    }

}
