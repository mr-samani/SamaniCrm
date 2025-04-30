using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;
public class Permission
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string LocalizeKey { get; set; }
}