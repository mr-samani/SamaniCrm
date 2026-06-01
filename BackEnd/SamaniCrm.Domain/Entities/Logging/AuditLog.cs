using SamaniCrm.Domain.Attributes;
using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Domain.Entities;

[AuditIgnore]
public class AuditLog:IMayHaveTenant
{
    public Guid Id { get; set; }
    public required string CorrelationId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }

    public Guid? DelegatorId { get; set; }

    public bool IsDelegated { get; set; }


    public string EntityName { get; set; } = null!;

    public string EntityId { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string? Changes { get; set; }

    public DateTime CreationTime { get; set; }
}
