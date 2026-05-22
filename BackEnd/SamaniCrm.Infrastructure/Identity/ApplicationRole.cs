using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Infrastructure.Identity
{
    public class ApplicationRole : IdentityRole<Guid>, IAuditedEntity, IMayHaveTenant
    {

        public Guid? TenantId { get; set; }

        public int Level { get; set; } = 2;
        public bool IsSystem { get; set; } = false;

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        public ApplicationRole() : base()
        {
        }
        public ApplicationRole(string roleName) : base(roleName)
        {
        }

        // ─── IAuditedEntity ───
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public byte[]? RowVersion { get; set; }
    }

}
