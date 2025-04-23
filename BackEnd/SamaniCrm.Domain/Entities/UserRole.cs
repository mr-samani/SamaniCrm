using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Constants;

namespace SamaniCrm.Domain.Entities
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        public ICollection<Roles> Roles { get; set; } = new List<Roles>();
        public ICollection<IUser> Users { get; set; } = new List<IUser>();

    }
}
