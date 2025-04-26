export const Apis = {
  login: '/api/account/login',
  register: '/api/account/register',
  refresh: '/api/account/refresh',
  getCurrentUser: '/api/account/getCurrentUser',

  reloadCaptcha: '/api/index/reloadCaptcha',

  getAllLanguages: '/api/language/getAll',
  changeActiveLanguage: '/api/language/changeActive',
  changeUserLanguage: '/api/language/changeUserLanguage',

  userList: '/api/user/userList',
  roleList: '/api/role/list',
  getRolePermissions: '/api/role/getRolePermissions/',
  saveRolePermissions: '/api/role/saveRolePermissions/',

  createDirectory: '/api/filemanager/createDirectory',
  getFolders: '/api/filemanager/getFolders',
  uploadFile: '/api/filemanager/uploadFile',
  getFileDetails: '/api/filemanager/getDetails',
  getFilemanagerIcons: '/api/filemanager/getIcons',
  setFolderIcon: '/api/filemanager/setFolderIcon',

  getAllSettings: '/api/setting/getAll',
  saveSettings: '/api/setting/saveSettings',

  getAllCacheKeys: '/api/maintenance/getAllCacheKeys',
  clearCache: '/api/maintenance/clearCache',

  // product category
  productCategories: '/api/productCategory/categoryList',
  reorderProductCategories: '/api/productCategory/reorder',
  getProductCategoryForEdit: '/api/productCategory/getForEdit',
  createOrEditProductCategory: '/api/productCategory/createOrEdit',
  deleteProductCategory: '/api/productCategory/deleteCategory',
  setImageProductCategory: '/api/productCategory/setImage',
  changeActiveProductCategory: '/api/productCategory/changeActive',

  // menu
  menuList: '/api/menu/list',
  reorderMenu: '/api/menu/reorder',
  getMenuForEdit: '/api/menu/getForEdit',
  createOrEditMenu: '/api/menu/createOrEdit',
  deleteMenu: '/api/menu/delete',
  changeActiveMenu: '/api/menu/changeActive',

  // pages
  pageList: '/api/page/list',
  reorderPages: '/api/page/reorder',
  getPageForEdit: '/api/page/getForEdit',
  createOrEditPage: '/api/page/createOrEdit',
  deletePage: '/api/page/deleteCategory',
  setPageCover: '/api/page/setCover',
  changeActivePage: '/api/page/changeActive',

  //pageGlobalConfig
  getPageGlobalConfigs: '/api/pageGlobalConfig/getGlobalConfigs',
};
