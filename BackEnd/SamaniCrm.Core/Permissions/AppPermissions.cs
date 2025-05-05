using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Core.Permissions
{
    public static class AppPermissions
    {

        public const string Administrator = "Administrator";

        // user management 
        public const string UserManagement = "Administrator.UserManagement";
        public const string UserManagement_List = "Administrator.UserManagement.List";
        public const string UserManagement_Create = "Administrator.UserManagement.Create";
        public const string UserManagement_Edit = "Administrator.UserManagement.Edit";
        public const string UserManagement_Delete = "Administrator.UserManagement.Delete";
        public const string UserManagement_AssignRole = "Administrator.UserManagement.AssignRole";
        public const string UserManagement_EditUserProfile = "Administrator.UserManagement.EditUserProfile";

        // role management 
        public const string RoleManagement = "Administrator.RoleManagement";
        public const string RoleManagement_List = "Administrator.RoleManagement.List";
        public const string RoleManagement_Create = "Administrator.RoleManagement.Create";
        public const string RoleManagement_Edit = "Administrator.RoleManagement.Edit";
        public const string RoleManagement_Delete = "Administrator.RoleManagement.Delete";
        public const string RoleManagement_AssignToRole = "Administrator.RoleManagement.AssignToRole";

        // language management 
        public const string LanguageManagement = "Administrator.LanguageManagement";
        public const string LanguageManagement_List = "Administrator.LanguageManagement.List";
        public const string LanguageManagement_Create = "Administrator.LanguageManagement.Create";
        public const string LanguageManagement_Edit = "Administrator.LanguageManagement.Edit";
        public const string LanguageManagement_Delete = "Administrator.LanguageManagement.Delete";

        // menu management 
        public const string MenuManagement = "Administrator.MenuManagement";
        public const string MenuManagement_List = "Administrator.MenuManagement.List";
        public const string MenuManagement_Create = "Administrator.MenuManagement.Create";
        public const string MenuManagement_Edit = "Administrator.MenuManagement.Edit";
        public const string MenuManagement_Delete = "Administrator.MenuManagement.Delete";

    }



}
