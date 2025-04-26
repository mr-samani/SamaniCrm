import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { SettingMenuService } from './setting.menu.service';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss'],
})
export class SettingsComponent extends AppComponentBase {
  constructor(
    injector: Injector,
    public menuService: SettingMenuService,
  ) {
    super(injector);
    this.breadcrumb.list = [{ name: this.l('Settings'), url: '/dashboard/setting' }];
  }
}
