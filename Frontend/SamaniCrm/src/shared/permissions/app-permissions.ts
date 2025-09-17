export abstract class AppPermissions {
  public static Administrator = 'Administrator';

  // user management
  public static UserManagement = 'Administrator.UserManagement';
  public static UserManagement_List = 'Administrator.UserManagement.List';
  public static UserManagement_Create = 'Administrator.UserManagement.Create';
  public static UserManagement_ChangeAvatar = 'Administrator.UserManagement.ChangeAvatar';
  public static UserManagement_Edit = 'Administrator.UserManagement.Edit';
  public static UserManagement_Delete = 'Administrator.UserManagement.Delete';
  public static UserManagement_AssignRole = 'Administrator.UserManagement.AssignRole';

  // role management
  public static RoleManagement = 'Administrator.RoleManagement';
  public static RoleManagement_List = 'Administrator.RoleManagement.List';
  public static RoleManagement_Create = 'Administrator.RoleManagement.Create';
  public static RoleManagement_Edit = 'Administrator.RoleManagement.Edit';
  public static RoleManagement_Delete = 'Administrator.RoleManagement.Delete';
  public static RoleManagement_EditRolePermissions = 'Administrator.RoleManagement.EditRolePermissions';

  //Security setting
  public static SecuritySetting = 'Administrator.SecuritySetting';
  public static SecuritySetting_GetSetting = 'Administrator.SecuritySetting.GetSetting';
  public static SecuritySetting_UpdateSetting = 'Administrator.SecuritySetting.UpdateSetting';

  public static SecuritySetting_GetUserSetting = 'Administrator.SecuritySetting.GetUserSetting';
  public static SecuritySetting_UpdateUserSetting = 'Administrator.SecuritySetting.UpdateUserSetting';
  public static SecuritySetting_TwoFactorApp = 'Administrator.SecuritySetting.TwoFactorApp';

  // language management
  public static LanguageManagement = 'Administrator.LanguageManagement';
  public static LanguageManagement_List = 'Administrator.LanguageManagement.List';
  public static LanguageManagement_Create = 'Administrator.LanguageManagement.Create';
  public static LanguageManagement_Edit = 'Administrator.LanguageManagement.Edit';
  public static LanguageManagement_Delete = 'Administrator.LanguageManagement.Delete';

  // menu management
  public static MenuManagement = 'Administrator.MenuManagement';
  public static MenuManagement_List = 'Administrator.MenuManagement.List';
  public static MenuManagement_Create = 'Administrator.MenuManagement.Create';
  public static MenuManagement_Edit = 'Administrator.MenuManagement.Edit';
  public static MenuManagement_Delete = 'Administrator.MenuManagement.Delete';
  public static MenuManagement_ReOrder = 'Administrator.MenuManagement.ReOrder';

  // Maintenance
  public static Maintenance = 'Administrator.Maintenance';
  public static CacheEntries_List = 'Administrator.Maintenance.Cache.List';
  public static CacheEntries_Delete = 'Administrator.Maintenance.Cache.Delete';
  public static CacheEntries_ClearAll = 'Administrator.Maintenance.Cache.ClearAll';

  // Pages
  public static Pages = 'Administrator.Pages';
  public static Pages_List = 'Administrator.Pages.List';
  public static Pages_Create = 'Administrator.Pages.Create';
  public static Pages_Update = 'Administrator.Pages.Update';
  public static Pages_Delete = 'Administrator.Pages.Delete';
  public static Pages_Home_Edit = 'Administrator.Pages.HomeEdit';
  public static Pages_AboutUs_Edit = 'Administrator.Pages.AboutUsEdit';
  public static Pages_ContactUs_Edit = 'Administrator.Pages.ContactUsEdit';
  public static Pages_OtherPage_Edit = 'Administrator.Pages.OtherPageEdit';
  public static Pages_Articles_Edit = 'Administrator.Pages.ArticlesEdit';
  public static Pages_News_Edit = 'Administrator.Pages.NewsEdit';
  public static Pages_Blogs_Edit = 'Administrator.Pages.BlogsEdit';

  public static Pages_Builder = 'Administrator.Pages.PageBuilder';
  public static Pages_CustomBlockList = 'Administrator.Pages.CustomBlockList';
  public static Pages_CreateCustomBlock = 'Administrator.Pages.CreateCustomBlock';
  public static Pages_DeleteCustomBlock = 'Administrator.Pages.DeleteCustomBlock';

  // products
  public static Products = 'Administrator.Products';
  public static Products_Category_List = 'Administrator.Products.Category.List';

  public static Products_Category_Export = 'Administrator.Products.Category.Export';
  public static Products_Category_Import = 'Administrator.Products.Category.Import';
  public static Products_Category_Create = 'Administrator.Products.Category.Create';
  public static Products_Category_Edit = 'Administrator.Products.Category.Edit';
  public static Products_Category_Delete = 'Administrator.Products.Category.Delete';

  public static Products_List = 'Administrator.Products.List';
  public static Products_Create = 'Administrator.Products.Create';
  public static Products_Edit = 'Administrator.Products.Edit';
  public static Products_Delete = 'Administrator.Products.Delete';

  public static Products_Attribute_List = 'Administrator.Products.Attribute.List';
  public static Products_Attribute_Create = 'Administrator.Products.Attribute.Create';
  public static Products_Attribute_Edit = 'Administrator.Products.Attribute.Edit';
  public static Products_Attribute_Delete = 'Administrator.Products.Attribute.Delete';

  public static Products_Type_List = 'Administrator.Products.Type.List';
  public static Products_Type_Create = 'Administrator.Products.Type.Create';
  public static Products_Type_Edit = 'Administrator.Products.Type.Edit';
  public static Products_Type_Delete = 'Administrator.Products.Type.Delete';

  public static Products_Currency_List = 'Administrator.Products.Currency.List';
  public static Products_Currency_Create = 'Administrator.Products.Currency.Create';
  public static Products_Currency_Edit = 'Administrator.Products.Currency.Edit';
  public static Products_Currency_Delete = 'Administrator.Products.Currency.Delete';

  // Notification
  public static Notification_List = 'Administrator.Notification.List';
  public static Notification_MarkAllAsRead = 'Administrator.Notification.MarkAllAsRead';
  public static Notification_Delete = 'Administrator.Notification.Delete';

  // file manager
  public static FileManager_List = 'Administrator.FileManager.List';

  public static FileManager_CreateFolder = 'Administrator.FileManager.CreateFolder';
  public static FileManager_CreateFile = 'Administrator.FileManager.CreateFile';
  public static FileManager_Delete = 'Administrator.FileManager.Delete';
}
