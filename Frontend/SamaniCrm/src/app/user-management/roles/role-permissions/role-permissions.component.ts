import { Component,  OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { TreeViewModel } from '@shared/components/tree-view/tree-view.model';
import { EditRolePermissionsCommand, RoleServiceProxy } from '@shared/service-proxies';
import { forkJoin } from 'rxjs';
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
  selectedBasePermission: TreeViewModel[] = [];
  roleName!: string;
  constructor(
    private roleService: RoleServiceProxy,
  ) {
    super();
    this.roleId = this.route.snapshot.params['roleId'];
    this.breadcrumb.list = [
      { name: this.l('Settings'), url: '/panel/setting' },
      { name: this.l('Roles'), url: '/panel/roles' },
    ];
  }

  ngOnInit(): void {
    this.getList();
  }

  getList() {
    this.loading = true;
    forkJoin([this.roleService.getRoleById(this.roleId), this.roleService.getRolePermissions(this.roleId)])
      .pipe(finalize(() => (this.loading = false)))
      .subscribe(([role, permissions]) => {
        this.list = permissions.data ?? ([] as any);
        this.roleName = role.data!.roleName;
      });
  }

  save() {
    this.isSaving = true;
    const input = new EditRolePermissionsCommand();
    input.roleName = this.roleName;
    input.grantedPermissions = this.getSelectedPermissions(this.list);
    this.roleService
      .editRolePermissions(input)
      .pipe(finalize(() => (this.isSaving = false)))
      .subscribe((response) => {
        this.notify.success(this.l('SaveSuccessFully'));
        this.getList();
      });
  }

  getSelectedPermissions(list: TreeViewModel[], selecteds: string[] = []): string[] {
    for (let item of list) {
      if (item.selected) {
        selecteds.push(item.name);
      }

      if (item.children && item.children.length > 0) {
        this.getSelectedPermissions(item.children, selecteds);
      }
    }

    return selecteds;
  }
}
