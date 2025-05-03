import { Component, Injector, OnInit } from '@angular/core';
import { FileUsageEnum } from '@app/file-manager/image-cropper-dialog/image-cropper-dialog.component';
import { Apis } from '@shared/apis';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@app/app-component-base';
import { FieldsType } from '@shared/components/table-view/fields-type.model';
import { FileManagerService } from '@app/file-manager/file-manager.service';
import { AppConst } from '@shared/app-const';
import { DownloadService, DownloadFileType } from '@shared/services/download.service';
import { RoleServiceProxy } from '@shared/service-proxies/api/role.service';
import { RoleResponseDTO } from '@shared/service-proxies/model/role-response-dto';

@Component({
  selector: 'app-user-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss'],
  standalone: false,
})
export class RoleListComponent extends AppComponentBase implements OnInit {
  loading = true;
  list: RoleResponseDTO[] = [];
  totalCount = 0;
  fields: FieldsType[] = [
    { column: 'id', title: this.l('Id'), width: 100 },
    { column: 'name', title: this.l('Name') },
  ];
  constructor(
    injector: Injector,
    private downloadService: DownloadService,
    private fileManager: FileManagerService,
    private roleService: RoleServiceProxy,
  ) {
    super(injector);
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
      });
  }

  exportExcel() {
    this.downloadService.downloadUrlWithToken(
      AppConst.apiUrl + '/api/user/exportExcel',
      DownloadFileType.Excel,
      'userlist.xlsx',
    );
  }

  changeAvatar(item: RoleResponseDTO) {
    this.fileManager
      .selectFile({
        usage: FileUsageEnum.USER_AVATAR,
      })
      .then((r) => {});
  }
}
