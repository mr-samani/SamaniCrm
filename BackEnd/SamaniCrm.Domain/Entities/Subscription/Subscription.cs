using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Domain.Entities.Subscription;

public class Subscription : BaseEntity, IMayHaveTenant
{
    public Guid? TenantId { get; set; }
    public Guid UserId { get; set; }

    public Guid PlanId { get; set; }
    public Guid PriceId { get; set; }

    [MaxLength(5)]
    public string CurrencyCode { get; set; } = default!;  // مثلا "USD", "IRR"
    public decimal TotalPrice { get; set; }

    public SubscriptionStatus SubscriptionStatus { get; set; }

    public DateTime? TrialStartDate { get; set; }
    public DateTime? TrialEndDate { get; set; }

    public DateTime CurrentPeriodStartDate { get; set; }
    public DateTime CurrentPeriodEndDate { get; set; }

    public DateTime? PausedDate { get; set; }
    public DateTime? ResumeDate { get; set; }

    public bool AutoRenew { get; set; } = false; // تمدید خودکار

    public DateTime? NextBillingDate { get; set; } // زمان صورت حساب بعدی

    [MaxLength(4000)]
    public string? MetaData { get; set; }


    public virtual Plan Plan { get; set; } = default!;
    public virtual PlanPrice Price { get; set; } = default!;

}
