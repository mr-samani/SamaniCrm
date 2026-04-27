import { Injectable } from '@angular/core';
import { AppPermissions } from '@shared/permissions/app-permissions';
import { LanguageService } from '@shared/services/language.service';
export class MenuModel {
  title!: string;
  permission?: string;
  description?: string;
  icon?: string;
  route?: string;
  children: MenuModel[];
  constructor(
    title: string,
    permission?: string,
    description?: string,
    icon?: string,
    route?: string,
    children?: MenuModel[],
  ) {
    this.title = title;
    this.permission = permission;
    this.description = description;
    this.icon = icon;
    this.route = route;
    this.children = children ?? [];
  }
}

@Injectable()
export class SettingMenuService {
  constructor() {}

  getMenuList(language: LanguageService) {
    const l = (key: string) => {
      return language.translate.instant(key);
    };
    const menuList: MenuModel[] = [
      new MenuModel(l('System'), AppPermissions.Administrator, '', '', '', [
        new MenuModel(
          l('AppSetting'),
          AppPermissions.Administrator,
          l('AppSettingDescription'),
          'fa fa-cog',
          '/panel/app-setting',
        ),
        new MenuModel(
          l('Maintenance'),
          AppPermissions.Maintenance,
          l('MaintenanceDescription'),
          'fa fa-folder-gear',
          '/panel/maintenance',
        ),
        new MenuModel(
          l('Menu'),
          AppPermissions.MenuManagement,
          l('MenuDescription'),
          'fa  fa-list-tree',
          '/panel/menu',
        ),
        new MenuModel(
          l('Lanaguages'),
          AppPermissions.LanguageManagement,
          l('LanguageDescription'),
          'fa  fa-language',
          '/panel/languages',
        ),
      ]),
      new MenuModel(l('Users'), AppPermissions.UserManagement, '', '', '', [
        new MenuModel(
          l('Roles'),
          AppPermissions.RoleManagement,
          l('RolesDescription'),
          'fa fa-users-gear',
          '/panel/roles',
        ),
        new MenuModel(l('Users'), AppPermissions.UserManagement, l('UsersDescription'), 'fa fa-users', '/panel/users'),
      ]),
    ];
    return menuList;
  }
}
