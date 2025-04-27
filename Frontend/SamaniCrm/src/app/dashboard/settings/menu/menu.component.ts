import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { Apis } from '@shared/apis';
import { finalize } from 'rxjs';
import { MenuModel } from './models/menu';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.scss',
})
export class MenuComponent extends AppComponentBase implements OnInit {
  list: MenuModel[] = [];
  loading = true;
  isSaving = false;
  constructor(injector: Injector) {
    super(injector);
    this.breadcrumb.list = [
      { name: this.l('Settings'), url: '/dashboard/setting' },
      { name: this.l('Menu'), url: '/dashboard/menu' },
    ];
  }

  ngOnInit(): void {
    this.getList();
  }

  getList() {
    this.loading = true;
    // this.dataService
    //   .get<any, MenuModel[]>(Apis.menuList, {})
    //   .pipe(finalize(() => (this.loading = false)))
    //   .subscribe((response) => {
    //     this.list = response.data ?? [];
    //   });
  }

  save() {
    this.isSaving = true;
    // this.dataService
    //   .post<{ menus: MenuModel[] }, null>(Apis.reorderMenu, { menus: this.list })
    //   .pipe(finalize(() => (this.isSaving = false)))
    //   .subscribe((response) => {
    //     this.notify.success(this.l('SaveSuccessFully'));
    //     this.getList();
    //   });
  }
}
