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
    public string DisplayName { get; set; } = default!;
    public Guid? ParentId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public int SortOrder { get; set; }

    public Permission? Parent { get; set; }


    public ICollection<Permission> Children { get; set; } = new List<Permission>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();


}