import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { TreeViewModel } from '@shared/components/tree-view/tree-view.model';
import { RoleServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-role-permissions',
  templateUrl: './role-permissions.component.html',
  styleUrls: ['./role-permissions.component.scss'],
  standalone: false,
})
export class RolePermissionsListComponent extends AppComponentBase implements OnInit {
  roleId = '';
  loading = true;
  isSaving = false;
  list: TreeViewModel[] = [];
  constructor(
    injector: Injector,

    private roleService: RoleServiceProxy,
  ) {
    super(injector);
    this.roleId = this.route.snapshot.params['roleId'];
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
    this.roleService
      .getRolePermissions(this.roleId)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data ?? ([] as any);
      });
  }

  save() {
    this.isSaving = true;
    // this.dataService
    //   .post<{ permissions: RolePermissionsDto[] }, null>(Apis.saveRolePermissions + this.roleId, {
    //     permissions: this.list,
    //   })
    //   .pipe(finalize(() => (this.isSaving = false)))
    //   .subscribe((response) => {
    //     this.notify.success(this.l('SaveSuccessFully'));
    //     this.getList();
    //   });
  }
}
