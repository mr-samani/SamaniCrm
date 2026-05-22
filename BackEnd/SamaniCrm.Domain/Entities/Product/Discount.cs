using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities;

public class Discount : BaseEntity,IMayHaveTenant
{
    public Guid? TenantId { get; set; }
    public string? Name { get; set; }

    public DiscountTypeEnum DiscountType { get; set; }
    public decimal Value { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public bool Expired { get; set; }


}
