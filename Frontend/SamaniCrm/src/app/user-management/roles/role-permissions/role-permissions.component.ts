import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'app-role-permissions',
  templateUrl: './role-permissions.component.html',
  styleUrls: ['./role-permissions.component.scss'],
  standalone: false,
})
export class RolePermissionsListComponent extends AppComponentBase implements OnInit {
  roleName = '';
  loading = true;
  isSaving = false;
  list: any[] = [];
  constructor(injector: Injector) {
    super(injector);
    this.roleName = this.route.snapshot.params['roleName'];
    this.breadcrumb.list = [
      { name: this.l('Settings'), url: '/dashboard/setting' },
      { name: this.l('Roles'), url: '/dashboard/roles' },
    ];
  }

  ngOnInit(): void {
    this.getList();
  }

  getList() {
    this.loading = true;
    // this.dataService
    //   .get<any, RolePermissionsDto[]>(Apis.getRolePermissions + this.roleName, {})
    //   .pipe(finalize(() => (this.loading = false)))
    //   .subscribe((response) => {
    //     this.list = response.data ?? [];
    //   });
  }

  save() {
    this.isSaving = true;
    // this.dataService
    //   .post<{ permissions: RolePermissionsDto[] }, null>(Apis.saveRolePermissions + this.roleName, {
    //     permissions: this.list,
    //   })
    //   .pipe(finalize(() => (this.isSaving = false)))
    //   .subscribe((response) => {
    //     this.notify.success(this.l('SaveSuccessFully'));
    //     this.getList();
    //   });
  }
}
