import { Component,  OnInit } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@app/app-component-base';
import { FieldsType } from '@shared/components/table-view/fields-type.model';
import { RoleServiceProxy } from '@shared/service-proxies/api/role.service';
import { RoleDTO } from '@shared/service-proxies/model/role-dto';
import { MatDialog } from '@angular/material/dialog';
import { CreateOrEditRoleComponent } from '../create-or-edit-role/create-or-edit-role.component';

@Component({
  selector: 'app-user-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss'],
  standalone: false,
})
export class RoleListComponent extends AppComponentBase implements OnInit {
  loading = true;
  list: RoleDTO[] = [];
  totalCount = 0;
  fields: FieldsType[] = [
    // { column: 'id', title: this.l('Id'), width: 200 },
    { column: 'roleName', title: this.l('Name') },
    { column: 'displayName', title: this.l('DisplayName') },
  ];
  constructor(
    private roleService: RoleServiceProxy,
    private matDialog: MatDialog,
  ) {
    super();
  }

  ngOnInit(): void {
    this.getList();
  }

  getList() {
    this.loading = true;
    this.roleService
      .getAllRoles()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data ?? [];
        this.list.map((m) => {
          m.displayName = this.l(m.displayName!);
        });
      });
  }

  openCreateOrEditDialog(item?: RoleDTO) {
    this.matDialog
      .open(CreateOrEditRoleComponent, {
        data: {
          role: item,
        },
        width: '768px',
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.getList();
        }
      });
  }

  remove(item: RoleDTO) {
    this.confirmMessage(`${this.l('Delete')}:${item?.roleName}`, this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        this.roleService
          .deleteRole(item.id)
          .pipe(finalize(() => this.hideMainLoading()))
          .subscribe((response) => {
            if (response.success) {
              this.notify.success(this.l('DeletedSuccessfully'));
              this.getList();
            }
          });
      }
    });
  }
}
