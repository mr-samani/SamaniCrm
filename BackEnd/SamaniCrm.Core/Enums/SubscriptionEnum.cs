using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Core.Shared.Enums;


public enum BillingType
{
    Recurring,
    OneTime,
    UsageBased
}

public enum BillingPeriod
{
    Daily,
    Weekly,
    Monthly,
    Yearly
}


public enum SubscriptionStatus
{
    /// <summary>
    /// آزمایشی
    /// </summary>
    Trialing,
    /// <summary>
    /// فعال
    /// </summary>
    Active,
    /// <summary>
    /// متوقف شده
    /// </summary>
    Paused,
    /// <summary>
    /// پرداخت معوق
    /// </summary>
    PastDue,
    /// <summary>
    /// کنسل شده
    /// </summary>
    Canceled,
    /// <summary>
    /// منقضی شده
    /// </summary>
    Expired
}


public enum PlanFeatureType
{
    Boolean,
    Integer,
    Decimal,
    String,

}