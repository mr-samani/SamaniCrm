import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { FieldsType, SortEvent } from '@shared/components/table-view/fields-type.model';
import { TenantsServiceProxy } from '@shared/service-proxies/api/tenants.service';
import { TenantListDto } from '@shared/service-proxies/model/tenant-list-dto';
import { TenantListQuery } from '@shared/service-proxies/model/tenant-list-query';
import { TenantStatus } from '@shared/service-proxies/model/tenant-status';
import { Subscription, finalize } from 'rxjs';
import { TenantUsersComponent } from './tenant-users/tenant-users.component';

@Component({
  standalone: false,
  selector: 'app-Tenants',
  templateUrl: './Tenants.component.html',
  styleUrls: ['./Tenants.component.scss'],
})
export class TenantsComponent extends AppComponentBase implements OnInit, OnDestroy {
  loading = true;

  list: TenantListDto[] = [];
  totalCount = 0;

  fields: FieldsType[] = [
    // { column: 'id', title: this.l('Id'), width: 100 },
    { column: 'name', title: this.l('Name') },
    { column: 'slug', title: this.l('Slug') },
    { column: 'userCount', title: this.l('userCount'), type: 'number' },
    { column: 'status', title: this.l('Status'), type: 'localize', localizeKey: 'TenantStatus_' },
    { column: 'createdAt', title: this.l('CreationTime'), type: 'dateTime' },
  ];

  form: FormGroup;
  page = 1;
  perPage = AppConst.defaultTablePerPage;
  listSubscription$?: Subscription;
  showFilter = false;
  constructor(
    private tenantService: TenantsServiceProxy,
    private matDialog: MatDialog,
  ) {
    super();
    this.breadcrumb.list = [{ name: this.l('Tenants'), url: '/panel/tenants' }];
    this.form = this.fb.group({
      filter: [''],
    });
    this.page = this.route.snapshot.queryParams['page'] ?? 1;
    this.perPage = this.route.snapshot.queryParams['perPage'] ?? 10;
  }

  ngOnInit(): void {
    this.getList();
  }

  public get TenantStatus(): typeof TenantStatus {
    return TenantStatus;
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
    const input = new TenantListQuery();
    input.search = this.form.get('filter')?.value;
    input.pageNumber = this.page;
    input.pageSize = this.perPage;
    input.sortBy = ev ? ev.field : '';
    input.sortDirection = ev ? ev.direction : '';
    this.listSubscription$ = this.tenantService
      .getAllTenants(input)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.chdr.detectChanges();
        }),
      )
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
    this.router.navigate(['/panel/tenants'], {
      queryParams: {
        page: this.page,
      },
    });
  }

  // openCreateOrEditUserDialog(item?: UserDTO) {
  //   this.matDialog
  //     .open(CreateOrEditUserComponent, {
  //       data: {
  //         user: item,
  //       },
  //       width: '768px',
  //     })
  //     .afterClosed()
  //     .subscribe((result) => {
  //       if (result) {
  //         this.reload();
  //       }
  //     });
  // }

  remove(item: TenantListDto) {
    this.confirmMessage(`${this.l('Delete')}:${item?.name}`, this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        this.tenantService
          .deleteTenant(item.id!)
          .pipe(
            finalize(() => {
              this.hideMainLoading();
              this.chdr.detectChanges();
            }),
          )
          .subscribe((response) => {
            if (response.success) {
              this.notify.success(this.l('DeletedSuccessfully'));
              this.reload();
            }
          });
      }
    });
  }
  openTenantUsersDialog(item: TenantListDto) {
    this.matDialog.open(TenantUsersComponent, {
      data: item,
      width: '80%',
    });
  }
}
