using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Domain.Entities.Subscription;

public class PlanPrice : BaseEntity, IMayHaveTenant
{
    public Guid? TenantId { get; set; }

    public Guid PlanId { get; set; }

    [MaxLength(5)]
    public required string Culture { get; set; }
    [MaxLength(5)]
    public string CurrencyCode { get; set; } = default!;  // مثلا "USD", "IRR"

    public decimal Amount { get; set; }

    public BillingPeriod BillingPeriod { get; set; }
    public int BillingPeriodCount { get; set; } = 1; // مثلا 3 ماهه
    public int TrialDays { get; set; } = 0;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;


    public virtual Language Language { get; set; } = default!;
    public virtual Plan Plan { get; set; } = default!;
}
