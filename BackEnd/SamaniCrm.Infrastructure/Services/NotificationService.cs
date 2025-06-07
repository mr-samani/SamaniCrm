using Azure.Core;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.NotificationManager.Queries;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;


        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<NotificationDto>> GetAllNotifications(GetAllNotificationQuery request, Guid currentUserId, CancellationToken cancellationToken)
        {

            var query = _context.Notifications
                        .Where(x => x.RecieverUserId == currentUserId)
                        .AsNoTracking()
                        .AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter))
            {
                query = query.Where(x =>
                    x.Title.Contains(request.Filter) ||
                    (x.Content != null && x.Content.Contains(request.Filter)) ||
                    (x.Data != null && x.Data.Contains(request.Filter))
                );
            }

            if (request.Type?.Any() == true)
            {
                query = query.Where(x => request.Type.Contains(x.Type));
            }

            if (request.Periority?.Any() == true)
            {
                query = query.Where(x => request.Periority.Contains(x.Periority));
            }

            // Sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var sortString = $"{request.SortBy} {request.SortDirection}";
                query = query.OrderBy(sortString);
            }

            int total = await query.CountAsync(cancellationToken);

            var notifications = await query
                .Skip(request.PageSize * (request.PageNumber - 1))
                .Take(request.PageSize)
                .Select(n => new
                {
                    n.Id,
                    n.Type,
                    n.Read,
                    n.Periority,
                    n.Title,
                    n.RecieverUserId,
                    n.SenderUserId,
                    n.CreationTime,
                })
                .ToListAsync(cancellationToken);

            var userIds = notifications
                .SelectMany(n => new[] { n.RecieverUserId, n.SenderUserId })
                .Where(id => id != null)
                .Distinct()
                .ToList();

            var userMap = await _context.Users
                .AsNoTracking()
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

            var result = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = n.Type,
                Read = n.Read,
                Periority = n.Periority,
                Title = n.Title,
                RecieverName = userMap.ContainsKey((Guid)n.RecieverUserId)
                    ? userMap[n.RecieverUserId]
                    : "",
                SenderName = n.SenderUserId != null && userMap.ContainsKey((Guid)n.SenderUserId)
                    ? userMap[n.SenderUserId.Value]
                    : "System",
                CreationTime = n.CreationTime.ToUniversalTime()
            }).ToList();

            return new PaginatedResult<NotificationDto>
            {
                Items = result,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<NotificationDto> GetNotification(Guid id, Guid currentUserId, CancellationToken cancellationToken)
        {

            var notification = await _context.Notifications
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == id && n.RecieverUserId == currentUserId, cancellationToken);

            if (notification == null)
                throw new NotFoundException("Notification not found");



            var userMap = await _context.Users
                .Where(u => u.Id == notification.RecieverUserId || u.Id == notification.SenderUserId)
                .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

            // بروزرسانی Read
            if (!notification.Read)
            {
                notification.Read = true;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return new NotificationDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Content = notification.Content,
                Data = notification.Data,
                Type = notification.Type,
                Periority = notification.Periority,
                Read = notification.Read,
                CreationTime = notification.CreationTime.ToUniversalTime(),
                RecieverName = userMap.ContainsKey(notification.RecieverUserId)
                    ? userMap[notification.RecieverUserId]
                    : "",
                SenderName = notification.SenderUserId.HasValue && userMap.ContainsKey(notification.SenderUserId.Value)
                    ? userMap[notification.SenderUserId.Value]
                    : "System"
            };
        }

    }
}
