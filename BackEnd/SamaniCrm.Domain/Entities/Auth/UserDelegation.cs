using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SamaniCrm.Domain.Entities;

public class UserDelegation
{
    public Guid Id { get; set; }

    public Guid AdminId { get; set; }

    public Guid TargetUserId { get; set; }

    [MaxLength(2000)]
    public string? Reason { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }


    [MaxLength(100)]
    public string? StartedFromIp { get; set; }
    [MaxLength(100)]
    public string? EndedFromIp { get; set; }

    public bool IsActive { get; set; }
}
