import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { MenuModel, SettingMenuService } from './setting.menu.service';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss'],
  standalone: false,
})
export class SettingsComponent extends AppComponentBase implements OnInit {
  menuList: MenuModel[] = [];
  constructor(private menuService: SettingMenuService) {
    super();
    this.breadcrumb.list = [{ name: this.l('Settings'), url: '/panel/setting' }];
  }

  ngOnInit(): void {
    this.menuList = this.menuService.getMenuList(this.language);
  }
}
