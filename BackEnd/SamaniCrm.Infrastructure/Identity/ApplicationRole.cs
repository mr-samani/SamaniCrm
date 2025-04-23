using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.Identity;
public class ApplicationRole : IdentityRole<Guid>, IRole
{
    public ApplicationRole() : base()
    {
    }
    public ApplicationRole(string roleName) : base(roleName)
    {
    }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}


