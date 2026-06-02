import { Component, inject, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { FieldsType, SortEvent } from '@shared/components/table-view/fields-type.model';
import { SecurityLogServiceProxy } from '@shared/service-proxies/api/security-log.service';
import { AutoCompleteDtoOfGuid } from '@shared/service-proxies/model/auto-complete-dto-of-guid';
import { GetSecurityLogsQuery } from '@shared/service-proxies/model/get-security-logs-query';
import { LogLevel } from '@shared/service-proxies/model/log-level';
import { SecurityEventType } from '@shared/service-proxies/model/security-event-type';
import { SecurityLogDto } from '@shared/service-proxies/model/security-log-dto';
import { Subscription, finalize } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-security-logs',
  templateUrl: './security-logs.component.html',
  styleUrls: ['./security-logs.component.scss'],
})
export class SecurityLogsComponent extends AppComponentBase implements OnInit {
  tenantId = null;
  items: SecurityLogDto[] = [];
  totalCount = 0;
  fields: FieldsType[] = [
    { column: 'message', title: this.l('Message'), width: 300, wrap: true },
    { column: 'severity', title: this.l('Severity'), type: 'localize', localizeKey: 'LogLevel_', width: 100 },
    { column: 'correlationId', title: this.l('Correlation'), width: 150 },
    {
      column: 'eventType',
      title: this.l('EventType'),
      type: 'localize',
      localizeKey: 'SecurityEventType_',
      width: 100,
    },
    { column: 'username', title: this.l('Username'), width: 100 },
  //  { column: 'createdBy', title: this.l('CreatedBy'), width: 100 },
    { column: 'action', title: this.l('Action'), width: 150 },
   // { column: 'resource', title: this.l('Resource'), width: 80 },
    { column: 'statusCode', title: this.l('StatusCode'), width: 80 },
    { column: 'isSuccessful', title: this.l('Success'), width: 80, type: 'yesNo' },
    { column: 'userName', title: this.l('UserName'), width: 200 },
    { column: 'userAgent', title: this.l('UserAgent'), width: 100, type: 'userAgent' },
    { column: 'ipAddress', title: this.l('Ip'), width: 100 },
    { column: 'createdAt', title: this.l('CreatedAt'), type: 'dateTime', width: 150 },
  ];

  loading = true;
  form: FormGroup;
  page = 1;
  perPage = AppConst.defaultTablePerPage;
  listSubscription$?: Subscription;
  showFilter = false;
  matDialog = inject(MatDialog);

  isHost = AppConst.isHost;
  constructor(private logService: SecurityLogServiceProxy) {
    super();
    this.breadcrumb.list = [
      { name: this.l('Settings'), url: '/panel/setting' },
      { name: this.l('Maintenance'), url: '/panel/maintenance' },
    ];
    this.form = this.fb.group({
      tenant: [null],
      filter: [null],
      severity: [null],
      eventType: [null],
    });
    this.page = this.route.snapshot.queryParams['page'] ?? 1;
    this.perPage = this.route.snapshot.queryParams['perPage'] ?? 10;
  }

  ngOnInit() {
    this.getList();
  }
  ngOnDestroy(): void {
    if (this.listSubscription$) {
      this.listSubscription$.unsubscribe();
    }
  }

  public get LogLevel(): typeof LogLevel {
    return LogLevel;
  }
  public get SecurityEventType(): typeof SecurityEventType {
    return SecurityEventType;
  }

  getList(ev?: SortEvent) {
    if (this.listSubscription$) {
      this.listSubscription$.unsubscribe();
    }
    this.loading = true;
    const input = new GetSecurityLogsQuery();

    input.severity = this.form.get('severity')?.value;
    input.search = this.form.get('filter')?.value;
    input.pageNumber = this.page;
    input.pageSize = this.perPage;
    input.sortBy = ev ? ev.field : '';
    input.sortDirection = ev ? ev.direction : '';

    if (this.isHost && this.form.get('tenant')?.value) {
      input.tenantId = (this.form.get('tenant')?.value as AutoCompleteDtoOfGuid).id;
    }
    this.logService
      .getLogs(input)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.chdr.detectChanges();
        }),
      )
      .subscribe((response) => {
        this.items = response.data?.items ?? [];
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
}
