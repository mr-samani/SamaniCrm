using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsGranted { get; set; } = true;
        public Permission Permission { get; set; } = default!;
        public IRole Role { get; set; } = default!;

 
 
 
    }
}
