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
        public const string UserManagement_ChangeAvatar = "Administrator.UserManagement.ChangeAvatar";
        public const string UserManagement_Edit = "Administrator.UserManagement.Edit";
        public const string UserManagement_Delete = "Administrator.UserManagement.Delete";
        public const string UserManagement_AssignRole = "Administrator.UserManagement.AssignRole";

        // role management 
        public const string RoleManagement = "Administrator.RoleManagement";
        public const string RoleManagement_List = "Administrator.RoleManagement.List";
        public const string RoleManagement_Create = "Administrator.RoleManagement.Create";
        public const string RoleManagement_Edit = "Administrator.RoleManagement.Edit";
        public const string RoleManagement_Delete = "Administrator.RoleManagement.Delete";
        public const string RoleManagement_EditRolePermissions = "Administrator.RoleManagement.EditRolePermissions";



        //Security setting
        public const string SecuritySetting = "Administrator.SecuritySetting";
        public const string SecuritySetting_ExternalProviders = "Administrator.ExternalProviders";
        public const string SecuritySetting_ExternalProvidersList = "Administrator.ExternalProviders.List";
        public const string SecuritySetting_ExternalProvidersCreate = "Administrator.ExternalProviders.Create";
        public const string SecuritySetting_ExternalProvidersUpdate = "Administrator.ExternalProviders.Update";
        public const string SecuritySetting_ExternalProvidersDelete = "Administrator.ExternalProviders.Delete";

        public const string SecuritySetting_GetSetting = "Administrator.SecuritySetting.GetSetting";
        public const string SecuritySetting_UpdateSetting = "Administrator.SecuritySetting.UpdateSetting";

        public const string SecuritySetting_GetUserSetting = "Administrator.SecuritySetting.GetUserSetting";
        public const string SecuritySetting_UpdateUserSetting = "Administrator.SecuritySetting.UpdateUserSetting";
        public const string SecuritySetting_TwoFactorApp = "Administrator.SecuritySetting.TwoFactorApp";




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
        public const string MenuManagement_ReOrder = "Administrator.MenuManagement.ReOrder";


        // Maintenance 
        public const string Maintenance = "Administrator.Maintenance";
        public const string CacheEntries_List = "Administrator.Maintenance.Cache.List";
        public const string CacheEntries_Delete = "Administrator.Maintenance.Cache.Delete";
        public const string CacheEntries_ClearAll = "Administrator.Maintenance.Cache.ClearAll";


        // Pages
        public const string Pages = "Administrator.Pages";
        public const string Pages_List = "Administrator.Pages.List";
        public const string Pages_Create = "Administrator.Pages.Create";
        public const string Pages_Update = "Administrator.Pages.Update";
        public const string Pages_Delete = "Administrator.Pages.Delete";
        public const string Pages_Home_Edit = "Administrator.Pages.HomeEdit";
        public const string Pages_AboutUs_Edit = "Administrator.Pages.AboutUsEdit";
        public const string Pages_ContactUs_Edit = "Administrator.Pages.ContactUsEdit";
        public const string Pages_OtherPage_Edit = "Administrator.Pages.OtherPageEdit";
        public const string Pages_Articles_Edit = "Administrator.Pages.ArticlesEdit";
        public const string Pages_News_Edit = "Administrator.Pages.NewsEdit";
        public const string Pages_Blogs_Edit = "Administrator.Pages.BlogsEdit";

        public const string Pages_Builder = "Administrator.Pages.PageBuilder";
        public const string Pages_CustomBlockList = "Administrator.Pages.CustomBlockList";
        public const string Pages_CreateCustomBlock = "Administrator.Pages.CreateCustomBlock";
        public const string Pages_DeleteCustomBlock = "Administrator.Pages.DeleteCustomBlock";


        // products
        public const string Products = "Administrator.Products";
        public const string Products_Category_List = "Administrator.Products.Category.List";
        public const string Products_Category_Export = "Administrator.Products.Category.Export";
        public const string Products_Category_Import = "Administrator.Products.Category.Import";
        public const string Products_Category_Create = "Administrator.Products.Category.Create";
        public const string Products_Category_Edit = "Administrator.Products.Category.Edit";
        public const string Products_Category_Delete = "Administrator.Products.Category.Delete";


        public const string Products_List = "Administrator.Products.List";
        public const string Products_Create = "Administrator.Products.Create";
        public const string Products_Edit = "Administrator.Products.Edit";
        public const string Products_Delete = "Administrator.Products.Delete";


        public const string Products_Attribute_List = "Administrator.Products.Attribute.List";
        public const string Products_Attribute_Create = "Administrator.Products.Attribute.Create";
        public const string Products_Attribute_Edit = "Administrator.Products.Attribute.Edit";
        public const string Products_Attribute_Delete = "Administrator.Products.Attribute.Delete";

        public const string Products_Type_List = "Administrator.Products.Type.List";
        public const string Products_Type_Create = "Administrator.Products.Type.Create";
        public const string Products_Type_Edit = "Administrator.Products.Type.Edit";
        public const string Products_Type_Delete = "Administrator.Products.Type.Delete";



        public const string Products_Currency_List = "Administrator.Products.Currency.List";
        public const string Products_Currency_Create = "Administrator.Products.Currency.Create";
        public const string Products_Currency_Edit = "Administrator.Products.Currency.Edit";
        public const string Products_Currency_Delete = "Administrator.Products.Currency.Delete";


        // Notification
        public const string Notification_List = "Administrator.Notification.List";
        public const string Notification_Delete = "Administrator.Notification.Delete";
        public const string Notification_MarkAllAsRead = "Administrator.Notification.MarkAllAsRead";
        public const string Notification_SendMessageToUser = "Administrator.Notification.SendMessageToUser";
        public const string Notification_BroadCastMessageToAll = "Administrator.Notification.BroadCastMessageToAll";


        // file manager
        public const string FileManager_List = "Administrator.FileManager.List";

        public const string FileManager_CreateFolder = "Administrator.FileManager.CreateFolder";
        public const string FileManager_CreateFile = "Administrator.FileManager.CreateFile";
        public const string FileManager_Delete = "Administrator.FileManager.Delete";





        // Dashboard 
        public const string Dashboard = "Panel.Dashboard";
        public const string Dashboard_List = "Panel.Dashboard.Cache.List";
        public const string Dashboard_Create = "Panel.Dashboard.Create";
        public const string Dashboard_Edit = "Panel.Dashboard.Edit";
        public const string Dashboard_Delete = "Panel.Dashboard.Delete";

        public const string Dashboard_Item_Create = "Panel.Dashboard.Item.Create";
        public const string Dashboard_Item_Edit = "Panel.Dashboard.Item.Edit";
        public const string Dashboard_Item_Delete = "Panel.Dashboard.Item.Delete";


    }



}
