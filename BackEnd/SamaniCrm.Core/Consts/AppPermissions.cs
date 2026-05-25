using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// After add permission 
/// - Must be run Migration seed
/// - Must be add to app permissions in front end
/// </summary>
namespace SamaniCrm.Core.Shared.Consts;



/// <summary>
/// After add permission
/// - Must be run Migration seed
/// - Must be add to app permissions in front end
/// </summary>
public static class AppPermissions
{
    public const string Administrator = "Administrator";

    // ═══════════════════════════════════════════════════════════════════════
    // 🔒 HOST ONLY - فقط مدیران هاست دسترسی دارند
    // ═══════════════════════════════════════════════════════════════════════

    public static class LoggingSystem
    {
        public const string List = "Administrator.LoggingSystem.List";
        public const string Details = "Administrator.LoggingSystem.Details";
        public const string GetStats = "Administrator.LoggingSystem.GetStats";
        public const string ManualCleanUpLog = "Administrator.LoggingSystem.ManualCleanUpLog";

        public static class TenantLogSetting
        {
            public const string List = "Administrator.LoggingSystem.Settings.List";
            public const string Update = "Administrator.LoggingSystem.Settings.Update";

        }
    }


    /// <summary>مدیریت مستأجرین (چندمستأجری)</summary>
    public static class TenantManagement
    {
        public const string List = "Administrator.TenantManagement.List";
        public const string AutoComplete = "Administrator.TenantManagement.AutoComplete";
        public const string Create = "Administrator.TenantManagement.Create";
        public const string Edit = "Administrator.TenantManagement.Edit";
        public const string Delete = "Administrator.TenantManagement.Delete";
        public const string ActiveDeActive = "Administrator.TenantManagement.ActiveDeActive";
        public const string Users = "Administrator.TenantManagement.Users";
        public const string Impersonate = "Administrator.TenantManagement.Impersonate";
        public const string GetTenantSetting = "Administrator.TenantManagement.GetTenantSetting";
        public const string UpdateTenantSetting = "Administrator.TenantManagement.UpdateTenantSetting";
        public const string GetProvisioningData = "Administrator.TenantManagement.GetProvisioningData";
        public const string RetryProvisioningData = "Administrator.TenantManagement.RetryProvisioningData";
        public const string DataBaseInfo = "Administrator.TenantManagement.DataBaseInfo";
        public const string TestDataBaseConnection = "Administrator.TenantManagement.TestDataBaseConnection";
        public const string GetTenantUsage = "Administrator.TenantManagement.GetTenantUsage";
    }


    /// <summary>تنظیمات امنیتی</summary>
    public static class SecuritySetting
    {
        public const string GetSetting = "Administrator.SecuritySetting.GetSetting";
        public const string UpdateSetting = "Administrator.SecuritySetting.UpdateSetting";
        public const string GetUserSetting = "Administrator.SecuritySetting.GetUserSetting";
        public const string UpdateUserSetting = "Administrator.SecuritySetting.UpdateUserSetting";
        public const string TwoFactorApp = "Administrator.SecuritySetting.TwoFactorApp";

        /// <summary>ارائه‌دهندگان خارجی</summary>
        public static class ExternalProviders
        {
            public const string List = "Administrator.ExternalProviders.List";
            public const string Create = "Administrator.ExternalProviders.Create";
            public const string Update = "Administrator.ExternalProviders.Update";
            public const string Delete = "Administrator.ExternalProviders.Delete";
        }
    }


    /// <summary>نگهداری سیستم</summary>
    public static class Maintenance
    {
        public static class Cache
        {
            public const string List = "Administrator.Maintenance.Cache.List";
            public const string Delete = "Administrator.Maintenance.Cache.Delete";
            public const string ClearAll = "Administrator.Maintenance.Cache.ClearAll";
        }
    }




    /// <summary>مدیریت کاربران</summary>
    public static class UserManagement
    {
        public const string List = "UserManagement.User.List";
        public const string Create = "UserManagement.User.Create";
        public const string ChangeAvatar = "UserManagement.User.ChangeAvatar";
        public const string Edit = "UserManagement.User.Edit";
        public const string Delete = "UserManagement.User.Delete";
        public const string AssignRole = "UserManagement.User.AssignRole";
    }

    /// <summary>مدیریت نقش‌ها</summary>
    public static class RoleManagement
    {
        public const string List = "UserManagement.Role.List";
        public const string Create = "UserManagement.Role.Create";
        public const string Edit = "UserManagement.Role.Edit";
        public const string Delete = "UserManagement.Role.Delete";
        public const string EditRolePermissions = "UserManagement.Role.EditRolePermissions";
    }


    /// <summary>مدیریت زبان</summary>
    public static class LanguageManagement
    {
        public const string List = "LanguageManagement.List";
        public const string Create = "LanguageManagement.Create";
        public const string Edit = "LanguageManagement.Edit";
        public const string Delete = "LanguageManagement.Delete";
    }

    /// <summary>مدیریت منو</summary>
    public static class MenuManagement
    {
        public const string List = "MenuManagement.List";
        public const string Create = "MenuManagement.Create";
        public const string Edit = "MenuManagement.Edit";
        public const string Delete = "MenuManagement.Delete";
        public const string ReOrder = "MenuManagement.ReOrder";
    }

    /// <summary>مدیریت صفحات (CMS)</summary>
    public static class Pages
    {
        public const string List = "PageManagement.List";
        public const string Create = "PageManagement.Create";
        public const string Update = "PageManagement.Update";
        public const string Delete = "PageManagement.Delete";
        public const string HomeEdit = "PageManagement.HomeEdit";
        public const string AboutUsEdit = "PageManagement.AboutUsEdit";
        public const string ContactUsEdit = "PageManagement.ContactUsEdit";
        public const string OtherPageEdit = "PageManagement.OtherPageEdit";
        public const string ArticlesEdit = "PageManagement.ArticlesEdit";
        public const string NewsEdit = "PageManagement.NewsEdit";
        public const string BlogsEdit = "PageManagement.BlogsEdit";
        public const string ProductEdit = "PageManagement.ProductEdit";
        public const string Builder = "PageManagement.PageBuilder";
        public const string PluginList = "PageManagement.PluginList";
        public const string CreatePlugin = "PageManagement.CreatePlugin";
        public const string DeletePlugin = "PageManagement.DeletePlugin";
    }

    /// <summary>مدیریت محصولات</summary>
    public static class Products
    {
        public const string List = "ProductManagement.List";
        public const string Create = "ProductManagement.Create";
        public const string Edit = "ProductManagement.Edit";
        public const string Delete = "ProductManagement.Delete";

        /// <summary>دسته‌بندی محصولات</summary>
        public static class Category
        {
            public const string List = "ProductManagement.Category.List";
            public const string Export = "ProductManagement.Category.Export";
            public const string Import = "ProductManagement.Category.Import";
            public const string Create = "ProductManagement.Category.Create";
            public const string Edit = "ProductManagement.Category.Edit";
            public const string Delete = "ProductManagement.Category.Delete";
        }

        /// <summary>ویژگی‌های محصول</summary>
        public static class Attribute
        {
            public const string List = "ProductManagement.Attribute.List";
            public const string Create = "ProductManagement.Attribute.Create";
            public const string Edit = "ProductManagement.Attribute.Edit";
            public const string Delete = "ProductManagement.Attribute.Delete";
        }

        /// <summary>نوع محصول</summary>
        public static class Type
        {
            public const string List = "ProductManagement.Type.List";
            public const string Create = "ProductManagement.Type.Create";
            public const string Edit = "ProductManagement.Type.Edit";
            public const string Delete = "ProductManagement.Type.Delete";
        }

        /// <summary>واحد پول</summary>
        public static class Currency
        {
            public const string List = "ProductManagement.Currency.List";
            public const string Create = "ProductManagement.Currency.Create";
            public const string Edit = "ProductManagement.Currency.Edit";
            public const string Delete = "ProductManagement.Currency.Delete";
        }
    }

    /// <summary>مدیریت اعلان‌ها</summary>
    public static class Notification
    {
        public const string List = "NotificationManagement.List";
        public const string Delete = "NotificationManagement.Delete";
        public const string MarkAllAsRead = "NotificationManagement.MarkAllAsRead";
        public const string SendMessageToUser = "NotificationManagement.SendMessageToUser";
        public const string BroadCastMessageToAll = "NotificationManagement.BroadCastMessageToAll";
    }

    /// <summary>مدیریت فایل</summary>
    public static class FileManager
    {
        public const string List = "FileManagement.FileManager.List";
        public const string CreateFolder = "FileManagement.FileManager.CreateFolder";
        public const string CreateFile = "FileManagement.FileManager.CreateFile";
        public const string Delete = "FileManagement.FileManager.Delete";
        public const string Rename = "FileManagement.FileManager.Rename";
    }

    // ═══════════════════════════════════════════════════════════════════════
    // 🏢 TENANT ONLY - فقط بهره‌برداران دسترسی دارند
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>داشبورد</summary>
    public static class Dashboard
    {
        public const string List = "DashboardManagement.List";
        public const string Create = "DashboardManagement.Create";
        public const string Edit = "DashboardManagement.Edit";
        public const string Delete = "DashboardManagement.Delete";

        public static class Item
        {
            public const string Create = "DashboardManagement.Item.Create";
            public const string Edit = "DashboardManagement.Item.Edit";
            public const string Delete = "DashboardManagement.Item.Delete";
        }
    }

    /// <summary>
    /// Subscription
    /// </summary>
    public static class SubscriptionManagement
    {
        public const string Page = "SubscriptionManagement.Page";
        public static class Plans
        {

            public const string List = "SubscriptionManagement.Plans.List";
            public const string Create = "SubscriptionManagement.Plans.Create";
            public const string Edit = "SubscriptionManagement.Plans.Edit";
            public const string Delete = "SubscriptionManagement.Plans.Delete";
        }
    }


}