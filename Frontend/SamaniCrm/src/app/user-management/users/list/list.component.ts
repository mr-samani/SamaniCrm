import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { FileManagerService } from '@app/file-manager/file-manager.service';
import { FileUsageEnum } from '@app/file-manager/image-cropper-dialog/image-cropper-dialog.component';

import { AppConst } from '@shared/app-const';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { FieldsType } from '@shared/components/table-view/fields-type.model';
import { GetUserQuery, UserServiceProxy } from '@shared/service-proxies';
import { UserResponseDTO } from '@shared/service-proxies/model/user-response-dto';
import { DownloadService, DownloadFileType } from '@shared/services/download.service';
import { Subscription } from 'rxjs';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-user-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss'],
  standalone: false,
})
export class UserListComponent extends AppComponentBase implements OnInit, OnDestroy {
  loading = true;

  list: UserResponseDTO[] = [];
  totalCount = 0;

  fields: FieldsType[] = [
    { column: 'profilePicture', title: this.l('Image'), width: 100, type: 'profilePicture' },
    // { column: 'id', title: this.l('id'), width: 100 },
    { column: 'name', title: this.l('Name') },
    { column: 'username', title: this.l('UserName') },
    { column: 'email', title: this.l('Email') },
    { column: 'lang', title: this.l('Language') },
    { column: 'registerTime', title: this.l('RegisterTime'), type: 'dateTime' },
  ];

  form: FormGroup;
  page = 1;
  perPage = 10;
  listSubscription$?: Subscription;
  showFilter = false;
  constructor(
    injector: Injector,
    private downloadService: DownloadService,
    private fileManager: FileManagerService,
    private userService: UserServiceProxy,
  ) {
    super(injector);
    this.breadcrumb.list = [
      { name: this.l('Settings'), url: '/dashboard/setting' },
      { name: this.l('Users'), url: '/dashboard/users' },
    ];
    this.form = this.fb.group({
      filter: [''],
    });
    this.page = this.route.snapshot.queryParams['page'] ?? 1;
    this.perPage = this.route.snapshot.queryParams['perPage'] ?? 10;
  }

  ngOnInit(): void {
    this.getList();
  }

  ngOnDestroy(): void {
    if (this.listSubscription$) {
      this.listSubscription$.unsubscribe();
    }
  }

  getList() {
    if (this.listSubscription$) {
      this.listSubscription$.unsubscribe();
    }
    this.loading = true;
    const input = new GetUserQuery();
    input.filter = this.form.get('filter')?.value;
    input.pageNumber = this.page;
    input.pageSize = this.perPage;
    this.listSubscription$ = this.userService
      .getAllUser(input)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data?.items ?? [];
        this.totalCount = response.data?.totalCount ?? 0;
      });
  }

  reload(setFirstPage = true) {
    if (setFirstPage) {
      this.page = 1;
    }
    this.onPageChange();
  }
  resetFilter() {
    this.showFilter = false;
    this.form.patchValue({ filter: '' });
    this.reload();
  }

  onPageChange(ev?: PageEvent) {
    this.getList();
    this.router.navigate(['/dashboard/users'], {
      queryParams: {
        page: this.page,
      },
    });
  }

  exportExcel() {
    this.downloadService.downloadUrlWithToken(
      AppConst.apiUrl + '/api/user/exportExcel',
      DownloadFileType.Excel,
      'userlist.xlsx',
    );
  }

  changeAvatar(item: UserResponseDTO) {
    this.fileManager
      .selectFile({
        usage: FileUsageEnum.USER_AVATAR,
      })
      .then((r) => {});
  }
}
