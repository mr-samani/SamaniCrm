import { Component,  OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { FileManagerService } from '@app/file-manager/file-manager.service';
import { FileUsageEnum } from '@app/file-manager/image-cropper-dialog/image-cropper-dialog.component';

import { AppConst } from '@shared/app-const';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { FieldsType, SortEvent } from '@shared/components/table-view/fields-type.model';
import { GetUserQuery, UserServiceProxy } from '@shared/service-proxies';
import { UserDTO } from '@shared/service-proxies/model/user-dto';
import { DownloadService, DownloadFileType } from '@shared/services/download.service';
import { Subscription } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { CreateOrEditUserComponent } from '../create-or-edit-user/create-or-edit-user.component';

@Component({
  selector: 'app-user-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss'],
  standalone: false,
})
export class UserListComponent extends AppComponentBase implements OnInit, OnDestroy {
  loading = true;

  list: UserDTO[] = [];
  totalCount = 0;

  fields: FieldsType[] = [
    { column: 'profilePicture', title: this.l('Image'), width: 100, type: 'profilePicture' },
    // { column: 'id', title: this.l('Id'), width: 100 },
    { column: 'fullName', title: this.l('Name') },
    { column: 'userName', title: this.l('UserName') },
    { column: 'email', title: this.l('Email') },
    { column: 'phoneNumber', title: this.l('Phone') },
    { column: 'address', title: this.l('Address') },
    { column: 'roles', title: this.l('Roles') },
    { column: 'lang', title: this.l('Language'), width: 50 },
    { column: 'creationTime', title: this.l('CreationTime'), type: 'dateTime' },
  ];

  form: FormGroup;
  page = 1;
  perPage = AppConst.defaultTablePerPage;
  listSubscription$?: Subscription;
  showFilter = false;
  constructor(
    private downloadService: DownloadService,
    private fileManager: FileManagerService,
    private userService: UserServiceProxy,
    private matDialog: MatDialog,
  ) {
    super();
    this.breadcrumb.list = [
      { name: this.l('Settings'), url: '/panel/setting' },
      { name: this.l('Users'), url: '/panel/users' },
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

  getList(ev?: SortEvent) {
    if (this.listSubscription$) {
      this.listSubscription$.unsubscribe();
    }
    this.loading = true;
    const input = new GetUserQuery();
    input.filter = this.form.get('filter')?.value;
    input.pageNumber = this.page;
    input.pageSize = this.perPage;
    input.sortBy = ev ? ev.field : '';
    input.sortDirection = ev ? ev.direction : '';
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
    if (ev) this.perPage = ev.perPage;
    this.getList();
    this.router.navigate(['/panel/users'], {
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

  changeAvatar(item: UserDTO) {
    this.fileManager
      .selectFile({
        usage: FileUsageEnum.USER_AVATAR,
      })
      .then((r) => {});
  }

  openCreateOrEditUserDialog(item?: UserDTO) {
    this.matDialog
      .open(CreateOrEditUserComponent, {
        data: {
          user: item,
        },
        width: '768px',
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.reload();
        }
      });
  }

  remove(item: UserDTO) {
    this.confirmMessage(`${this.l('Delete')}:${item?.fullName}`, this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        this.userService
          .deleteUser(item.id)
          .pipe(finalize(() => this.hideMainLoading()))
          .subscribe((response) => {
            if (response.success) {
              this.notify.success(this.l('DeletedSuccessfully'));
              this.reload();
            }
          });
      }
    });
  }
}
