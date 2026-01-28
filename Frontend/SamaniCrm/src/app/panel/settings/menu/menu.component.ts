import { Component,  OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { MenuDTO, MenuServiceProxy, ReorderItem, ReorderMenuCommand } from '@shared/service-proxies';
import { finalize } from 'rxjs';
import { TreeNode } from './TreeNode';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.scss',
  standalone: false,
})
export class MenuComponent extends AppComponentBase implements OnInit {
  list: TreeNode[] = [];
  loading = true;
  isSaving = false;
  constructor(
    private menuService: MenuServiceProxy,
  ) {
    super();
    this.breadcrumb.list = [
      { name: this.l('Settings'), url: '/panel/setting' },
      { name: this.l('Menu'), url: '/panel/menu' },
    ];
  }

  ngOnInit(): void {
    this.getList();
  }

  getList() {
    this.loading = true;
    this.menuService
      .getAllMenus()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data ?? ([] as any);
      });
  }

  save() {
    this.isSaving = true;
    const input = new ReorderMenuCommand();
    input.items = this.getOrderItems(this.list);

    this.menuService
      .reOrderMenu(input)
      .pipe(finalize(() => (this.isSaving = false)))
      .subscribe((response) => {
        this.notify.success(this.l('SaveSuccessFully'));
        this.getList();
      });
  }

  getOrderItems(list: TreeNode[], items: ReorderItem[] = [], orderIndex = 0, parentId?: string): ReorderItem[] {
    for (let i = 0; i < list.length; i++) {
      items.push(
        new ReorderItem({
          menuId: list[i].id,
          orderIndex: orderIndex,
          parentId: parentId,
        }),
      );
      orderIndex++;
      if (list[i].children) {
        this.getOrderItems(list[i].children, items, orderIndex, list[i].id);
      }
    }
    return items;
  }
}
