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

    /// <summary>مدیریت کاربران</summary>
    public static class UserManagement
    {
        public const string List = "Administrator.UserManagement.List";
        public const string Create = "Administrator.UserManagement.Create";
        public const string ChangeAvatar = "Administrator.UserManagement.ChangeAvatar";
        public const string Edit = "Administrator.UserManagement.Edit";
        public const string Delete = "Administrator.UserManagement.Delete";
        public const string AssignRole = "Administrator.UserManagement.AssignRole";
    }

    /// <summary>مدیریت نقش‌ها</summary>
    public static class RoleManagement
    {
        public const string List = "Administrator.RoleManagement.List";
        public const string Create = "Administrator.RoleManagement.Create";
        public const string Edit = "Administrator.RoleManagement.Edit";
        public const string Delete = "Administrator.RoleManagement.Delete";
        public const string EditRolePermissions = "Administrator.RoleManagement.EditRolePermissions";
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

    /// <summary>مدیریت زبان</summary>
    public static class LanguageManagement
    {
        public const string List = "Administrator.LanguageManagement.List";
        public const string Create = "Administrator.LanguageManagement.Create";
        public const string Edit = "Administrator.LanguageManagement.Edit";
        public const string Delete = "Administrator.LanguageManagement.Delete";
    }

    /// <summary>مدیریت منو</summary>
    public static class MenuManagement
    {
        public const string List = "Administrator.MenuManagement.List";
        public const string Create = "Administrator.MenuManagement.Create";
        public const string Edit = "Administrator.MenuManagement.Edit";
        public const string Delete = "Administrator.MenuManagement.Delete";
        public const string ReOrder = "Administrator.MenuManagement.ReOrder";
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

    /// <summary>مدیریت صفحات (CMS)</summary>
    public static class Pages
    {
        public const string List = "Administrator.Pages.List";
        public const string Create = "Administrator.Pages.Create";
        public const string Update = "Administrator.Pages.Update";
        public const string Delete = "Administrator.Pages.Delete";
        public const string HomeEdit = "Administrator.Pages.HomeEdit";
        public const string AboutUsEdit = "Administrator.Pages.AboutUsEdit";
        public const string ContactUsEdit = "Administrator.Pages.ContactUsEdit";
        public const string OtherPageEdit = "Administrator.Pages.OtherPageEdit";
        public const string ArticlesEdit = "Administrator.Pages.ArticlesEdit";
        public const string NewsEdit = "Administrator.Pages.NewsEdit";
        public const string BlogsEdit = "Administrator.Pages.BlogsEdit";
        public const string ProductEdit = "Administrator.Pages.ProductEdit";
        public const string Builder = "Administrator.Pages.PageBuilder";
        public const string PluginList = "Administrator.Pages.PluginList";
        public const string CreatePlugin = "Administrator.Pages.CreatePlugin";
        public const string DeletePlugin = "Administrator.Pages.DeletePlugin";
    }

    /// <summary>مدیریت محصولات</summary>
    public static class Products
    {
        public const string List = "Administrator.Products.List";
        public const string Create = "Administrator.Products.Create";
        public const string Edit = "Administrator.Products.Edit";
        public const string Delete = "Administrator.Products.Delete";

        /// <summary>دسته‌بندی محصولات</summary>
        public static class Category
        {
            public const string List = "Administrator.Products.Category.List";
            public const string Export = "Administrator.Products.Category.Export";
            public const string Import = "Administrator.Products.Category.Import";
            public const string Create = "Administrator.Products.Category.Create";
            public const string Edit = "Administrator.Products.Category.Edit";
            public const string Delete = "Administrator.Products.Category.Delete";
        }

        /// <summary>ویژگی‌های محصول</summary>
        public static class Attribute
        {
            public const string List = "Administrator.Products.Attribute.List";
            public const string Create = "Administrator.Products.Attribute.Create";
            public const string Edit = "Administrator.Products.Attribute.Edit";
            public const string Delete = "Administrator.Products.Attribute.Delete";
        }

        /// <summary>نوع محصول</summary>
        public static class Type
        {
            public const string List = "Administrator.Products.Type.List";
            public const string Create = "Administrator.Products.Type.Create";
            public const string Edit = "Administrator.Products.Type.Edit";
            public const string Delete = "Administrator.Products.Type.Delete";
        }

        /// <summary>واحد پول</summary>
        public static class Currency
        {
            public const string List = "Administrator.Products.Currency.List";
            public const string Create = "Administrator.Products.Currency.Create";
            public const string Edit = "Administrator.Products.Currency.Edit";
            public const string Delete = "Administrator.Products.Currency.Delete";
        }
    }

    /// <summary>مدیریت اعلان‌ها</summary>
    public static class Notification
    {
        public const string List = "Administrator.Notification.List";
        public const string Delete = "Administrator.Notification.Delete";
        public const string MarkAllAsRead = "Administrator.Notification.MarkAllAsRead";
        public const string SendMessageToUser = "Administrator.Notification.SendMessageToUser";
        public const string BroadCastMessageToAll = "Administrator.Notification.BroadCastMessageToAll";
    }

    /// <summary>مدیریت فایل</summary>
    public static class FileManager
    {
        public const string List = "Administrator.FileManager.List";
        public const string CreateFolder = "Administrator.FileManager.CreateFolder";
        public const string CreateFile = "Administrator.FileManager.CreateFile";
        public const string Delete = "Administrator.FileManager.Delete";
        public const string Rename = "Administrator.FileManager.Rename";
    }

    // ═══════════════════════════════════════════════════════════════════════
    // 🏢 TENANT ONLY - فقط بهره‌برداران دسترسی دارند
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>داشبورد</summary>
    public static class Dashboard
    {
        public const string List = "Panel.Dashboard.Cache.List";
        public const string Create = "Panel.Dashboard.Create";
        public const string Edit = "Panel.Dashboard.Edit";
        public const string Delete = "Panel.Dashboard.Delete";

        public static class Item
        {
            public const string Create = "Panel.Dashboard.Item.Create";
            public const string Edit = "Panel.Dashboard.Item.Edit";
            public const string Delete = "Panel.Dashboard.Item.Delete";
        }
    }
}