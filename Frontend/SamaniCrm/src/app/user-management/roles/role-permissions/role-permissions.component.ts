import { Component, Injector, OnInit } from '@angular/core';
import { FileUsageEnum } from '@app/file-manager/image-cropper-dialog/image-cropper-dialog.component';
import { Apis } from '@shared/apis';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from 'src/app/app-component-base';
import { FileManagerService } from 'src/app/file-manager/file-manager.service';
import { AppConst } from 'src/shared/app-const';
import { DownloadFileType, DownloadService } from 'src/shared/services/download.service';
import { RoleDto } from '../models/RoleDto';
import { FieldsType } from '@shared/components/table-view/fields-type.model';
import { RolePermissionsDto } from '../models/role-permissions';

@Component({
  selector: 'app-role-permissions',
  templateUrl: './role-permissions.component.html',
  styleUrls: ['./role-permissions.component.scss'],
})
export class RolePermissionsListComponent extends AppComponentBase implements OnInit {
  roleName = '';
  loading = true;
  isSaving = false;
  list: RolePermissionsDto[] = [];
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
    this.dataService
      .get<any, RolePermissionsDto[]>(Apis.getRolePermissions + this.roleName, {})
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data ?? [];
      });
  }

  save() {
    this.isSaving = true;
    this.dataService
      .post<{ permissions: RolePermissionsDto[] }, null>(Apis.saveRolePermissions + this.roleName, {
        permissions: this.list,
      })
      .pipe(finalize(() => (this.isSaving = false)))
      .subscribe((response) => {
        this.notify.success(this.l('SaveSuccessFully'));
        this.getList();
      });
  }
}
