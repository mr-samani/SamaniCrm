using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Domain.Entities;

public class Notification : BaseEntity
{
    [MaxLength(1000)]
    public required string Title { get; set; }
    public string? Content { get; set; }
    public NotificationPeriorityEnum Periority { get; set; } = NotificationPeriorityEnum.Normal;

    public NotificationTypeEnum Type { get; set; } = NotificationTypeEnum.Message;


    public required Guid RecieverUserId { get; set; }
    [Description("Systemic is null")]
    public Guid? SenderUserId { get; set; }

    public bool Read { get; set; } = false;

    public string? Data { get; set; }

}



