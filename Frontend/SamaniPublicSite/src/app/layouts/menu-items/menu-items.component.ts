import { CommonModule } from '@angular/common';
import { Component, Input, input, OnInit, output } from '@angular/core';
import { MenuDTO } from '@shared/service-proxies/model/menu-dto';

@Component({
  selector: 'menu-items',
  templateUrl: './menu-items.component.html',
  styleUrls: ['./menu-items.component.scss'],
  imports: [CommonModule],
})
export class MenuItemsComponent implements OnInit {
  menuList = input.required<MenuDTO[]>();
  closeNavBar = output<void>();

  constructor() {}

  ngOnInit() {}

  closeMenu() {
    this.closeNavBar.emit();
  }
}
