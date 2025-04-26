import { Component, Injector } from '@angular/core';
import { AppComponentBase } from 'src/app/app-component-base';
import { menus } from './menus/menus';

@Component({
  selector: 'app-side-menu',
  templateUrl: './side-menu.component.html',
  styleUrls: ['./side-menu.component.scss'],
})
export class SideMenuComponent extends AppComponentBase {
  menuItems = menus;
  constructor(injector: Injector) {
    super(injector);
  }
}
