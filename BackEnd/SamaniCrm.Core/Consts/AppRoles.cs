using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Core.Shared.Consts;

public abstract class AppRoles
{
    // host system administrator 
    public const string SysAdmin = nameof(SysAdmin);
    // Tenant admin
    public const string TenantAdministrator = nameof(TenantAdministrator);
    // tenant users
    public const string TenantUser = nameof(TenantUser);

}
