import { Injectable, Injector } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';

class MenuModel {
  title!: string;
  description?: string;
  icon?: string;
  route?: string;
  children: MenuModel[];
  constructor(title: string, description?: string, icon?: string, route?: string, children?: MenuModel[]) {
    this.title = title;
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
    new MenuModel(this.l('System'), '', '', '', [
      new MenuModel(this.l('AppSetting'), this.l('AppSettingDescription'), 'fa fa-cog', '/dashboard/app-setting'),
      new MenuModel(
        this.l('Maintenance'),
        this.l('MaintenanceDescription'),
        'fa fa-folder-gear',
        '/dashboard/maintenance',
      ),
      new MenuModel(this.l('Menu'), this.l('MenuDescription'), 'fa  fa-list-tree', '/dashboard/menu'),
      new MenuModel(this.l('Lanaguages'), this.l('LanguageDescription'), 'fa  fa-language', '/dashboard/languages'),
    ]),
    new MenuModel(this.l('Users'), '', '', '', [
      new MenuModel(this.l('Roles'), this.l('RolesDescription'), 'fa fa-users-gear', '/dashboard/roles'),
      new MenuModel(this.l('Users'), this.l('UsersDescription'), 'fa fa-users', '/dashboard/users'),
    ]),
  ];
}
