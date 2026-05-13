using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Constants
{
    public abstract class Roles
    {
        // SuserAdmin
        public const string Administrator = nameof(Administrator);
        // Tenant admin
        public const string TenantAdministrator = nameof(TenantAdministrator);
    }
}
