import { DIALOG_DATA } from '@angular/cdk/dialog';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { FieldsType, SortEvent } from '@shared/components/table-view/fields-type.model';
import { GetTenantUsersQuery, TenantUserDTO, UserServiceProxy } from '@shared/service-proxies';
import { TenantListDto } from '@shared/service-proxies/model/tenant-list-dto';
import { Subscription } from 'rxjs/internal/Subscription';
import { finalize } from 'rxjs/operators';

@Component({
  standalone: false,
  selector: 'app-tenant-users',
  templateUrl: './tenant-users.component.html',
  styleUrls: ['./tenant-users.component.scss'],
})
export class TenantUsersComponent extends AppComponentBase implements OnInit, OnDestroy {
  tenant = inject<TenantListDto>(DIALOG_DATA);

  loading = true;

  list: TenantUserDTO[] = [];
  totalCount = 0;

  fields: FieldsType[] = [
    { column: 'fullName', title: this.l('Name') },
    { column: 'userName', title: this.l('UserName') },
    { column: 'email', title: this.l('Email') },
    { column: 'roles', title: this.l('Roles') },
  ];
  form: FormGroup;
  page = 1;
  perPage = AppConst.defaultTablePerPage;
  showFilter = false;
  listSubscription$?: Subscription;
  userService = inject(UserServiceProxy);
  constructor() {
    super();
    this.form = this.fb.group({
      filter: [''],
    });
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
    const input = new GetTenantUsersQuery();
    input.tenantId = this.tenant.id;
    input.filter = this.form.get('filter')?.value;
    input.pageNumber = this.page;
    input.pageSize = this.perPage;
    input.sortBy = ev ? ev.field : '';
    input.sortDirection = ev ? ev.direction : '';
    this.listSubscription$ = this.userService
      .getTenantUsers(input)
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
  }

  impersonate(item: TenantUserDTO) {}
}
