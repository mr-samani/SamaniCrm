using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SamaniCrm.Domain.Entities.Subscription;

public class Plan : BaseEntity, IMayHaveTenant
{
    public Guid? TenantId { get; set; }

    [MaxLength(200)]
    public required string Name { get; set; }
    [MaxLength(100)]
    public required string Code { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
    public BillingType BillingType { get; set; } = BillingType.Recurring;

    public bool IsActive { get; set; } = true;
    public bool IsPublic { get; set; } = true;
}
