using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Application.DTOs
{
    public class NotificationDto
    {
        public Guid? Id { get; set; }


        [MaxLength(1000)]
        public required string Title { get; set; }
        public string? Content { get; set; }
        public NotificationPeriorityEnum Periority { get; set; }

        public NotificationTypeEnum Type { get; set; }

        public Guid? RecieverUserId { get; set; }
        public string? RecieverName { get; set; }
        public Guid? SenderUserId { get; set; }
        public string? SenderName { get; set; }

        public bool Read { get; set; } = false;

        public string? Data { get; set; }

        public DateTime? CreationTime { get; set; }

    }
}
