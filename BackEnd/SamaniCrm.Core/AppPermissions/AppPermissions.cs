using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Core.AppPermissions
{
    public static class AppPermissions
    {
        public const string Permissions = "Permissions";
        // user management 
        public const string UserManagement = Permissions + "Administrator.UserManagement";
        public const string UserManagement_List = UserManagement + "List";
        public const string UserManagement_Create = UserManagement + "Create";
        public const string UserManagement_Edit = UserManagement + "Edit";
        public const string UserManagement_Delete = UserManagement + "Delete";

        // role management 
        public const string RoleManagement = Permissions + "Administrator.RoleManagement";
        public const string RoleManagement_List = RoleManagement + "List";
        public const string RoleManagement_Create = RoleManagement + "Create";
        public const string RoleManagement_Edit = RoleManagement + "Edit";
        public const string RoleManagement_Delete = RoleManagement + "Delete";
        public const string RoleManagement_AssignToRole = RoleManagement + "AssignToRole";

    }



}
