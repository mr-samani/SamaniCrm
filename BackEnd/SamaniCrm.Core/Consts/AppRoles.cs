using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Core.Shared.Consts;

public abstract class AppRoles
{
    // SuserAdmin
    public const string Administrator = nameof(Administrator);
    // Tenant admin
    public const string TenantAdministrator = nameof(TenantAdministrator);
}
