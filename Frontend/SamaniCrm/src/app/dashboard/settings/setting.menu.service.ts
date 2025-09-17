import { Injectable, Injector } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { AppPermissions } from '@shared/permissions/app-permissions';

class MenuModel {
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
export class SettingMenuService extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
  menuList: MenuModel[] = [
    new MenuModel(this.l('System'), AppPermissions.Administrator, '', '', '', [
      new MenuModel(
        this.l('AppSetting'),
        AppPermissions.Administrator,
        this.l('AppSettingDescription'),
        'fa fa-cog',
        '/dashboard/app-setting',
      ),
      new MenuModel(
        this.l('Maintenance'),
        AppPermissions.Maintenance,
        this.l('MaintenanceDescription'),
        'fa fa-folder-gear',
        '/dashboard/maintenance',
      ),
      new MenuModel(
        this.l('Menu'),
        AppPermissions.MenuManagement,
        this.l('MenuDescription'),
        'fa  fa-list-tree',
        '/dashboard/menu',
      ),
      new MenuModel(
        this.l('Lanaguages'),
        AppPermissions.LanguageManagement,
        this.l('LanguageDescription'),
        'fa  fa-language',
        '/dashboard/languages',
      ),
    ]),
    new MenuModel(this.l('Users'), AppPermissions.UserManagement, '', '', '', [
      new MenuModel(
        this.l('Roles'),
        AppPermissions.RoleManagement,
        this.l('RolesDescription'),
        'fa fa-users-gear',
        '/dashboard/roles',
      ),
      new MenuModel(
        this.l('Users'),
        AppPermissions.UserManagement,
        this.l('UsersDescription'),
        'fa fa-users',
        '/dashboard/users',
      ),
    ]),
  ];
}
