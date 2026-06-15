import { CommonModule } from '@angular/common';
import { Component, Input, input, OnInit, output } from '@angular/core';
import { MenuDTO } from '@shared/service-proxies/model/menu-dto';
import { PageViewRoutingModule } from '@app/page-view/page-view-routing.module';
import { AppConst } from '@shared/app-const';
import { MenuTargetEnum } from '@shared/service-proxies/model/menu-target-enum';

@Component({
  selector: 'menu-items',
  templateUrl: './menu-items.component.html',
  styleUrls: ['./menu-items.component.scss'],
  imports: [CommonModule, PageViewRoutingModule],
})
export class MenuItemsComponent implements OnInit {
  menuList = input.required<MenuDTO[]>();
  closeNavBar = output<void>();
  currentLanguage = AppConst.currentLanguage;

  constructor() {}

  ngOnInit() {}

  public get MenuTargetEnum(): typeof MenuTargetEnum {
    return MenuTargetEnum;
  }

  closeMenu() {
    this.closeNavBar.emit();
  }
}
