using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Interfaces;

public class IMayHaveTenant
{
    public Guid? TenantId { get; set; }
}
