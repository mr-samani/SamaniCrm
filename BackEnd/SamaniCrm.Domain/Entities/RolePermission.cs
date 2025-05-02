﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;
public class RolePermission
{
    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }
    public virtual Permission Permission { get; set; } = default!;
}
