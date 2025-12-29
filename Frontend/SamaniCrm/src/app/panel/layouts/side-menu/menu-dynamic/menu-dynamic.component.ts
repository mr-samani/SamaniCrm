import { Component, OnInit, Input, ElementRef, OnDestroy } from '@angular/core';
import { SideNavMenuItem } from '../models/menu-item.model';

@Component({
  selector: 'menu-dynamic',
  templateUrl: './menu-dynamic.component.html',
  styleUrls: ['./menu-dynamic.component.scss'],
  standalone: false,
})
export class MenuDynamicComponent implements OnInit, OnDestroy {
  @Input('menu') menuItems: SideNavMenuItem[] = [];

  constructor() {}

  ngOnInit(): void {}

  toggle(item: SideNavMenuItem, ev: Event) {
    item.isOpen = !item.isOpen;
    ev.preventDefault();
    ev.stopPropagation();
  }

  stopPagination(ev: Event) {
    ev.preventDefault();
    ev.stopPropagation();
  }

  ngOnDestroy(): void {}

  checkPermission(item: SideNavMenuItem): boolean {
    return true; // this.authService.checkPermission(item.permission);
  }
}
