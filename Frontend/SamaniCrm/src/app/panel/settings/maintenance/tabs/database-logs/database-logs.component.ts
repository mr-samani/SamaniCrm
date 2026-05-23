import { Component, inject, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { LogLevelFlag } from '@app/panel/settings/app-setting/tabs/log-setting/models/log-level-flag';
import { LOG_LEVELS } from '@app/panel/settings/app-setting/tabs/log-setting/models/LOG_LEVELS';
import { AppConst } from '@shared/app-const';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { FieldsType, SortEvent } from '@shared/components/table-view/fields-type.model';
import { Bitmask } from '@shared/helper/bit-mask.utils';
import { LuxonFormatPipe } from '@shared/pipes/luxon-format.pipe';
import { AdminLogServiceProxy } from '@shared/service-proxies/api/admin-log.service';
import { GetLogsQuery } from '@shared/service-proxies/model/get-logs-query';
import { LogEntryDto } from '@shared/service-proxies/model/log-entry-dto';
import { LogLevel } from '@shared/service-proxies/model/log-level';
import { ManulaCleanupLogCommand } from '@shared/service-proxies/model/manula-cleanup-log-command';
import { Subscription } from 'rxjs/internal/Subscription';
import { finalize } from 'rxjs/operators';

@Component({
  standalone: false,
  selector: 'app-database-logs',
  templateUrl: './database-logs.component.html',
  styleUrls: ['./database-logs.component.scss'],
})
export class DatabaseLogsComponent extends AppComponentBase implements OnInit {
  tenantId = null;
  items: LogEntryDto[] = [];
  totalCount = 0;
  fields: FieldsType[] = [
    { column: 'level', title: this.l('Level'), type: 'localize', localizeKey: 'LogLevel_', width: 100 },
    { column: 'duration', title: this.l('Duration'), width: 100, prefix: 'ms' },
    { column: 'controllerName', title: this.l('Controlle'), width: 150 },
    { column: 'actionName', title: this.l('Action'), width: 150 },
    { column: 'httpMethod', title: this.l('Method'), width: 80 },
    { column: 'requestPath', title: this.l('Request'), width: 300 },
    { column: 'userName', title: this.l('UserName'), width: 200 },
    { column: 'ipAddress', title: this.l('Ip'), width: 100 },
    { column: 'message', title: this.l('Message'), width: 600, wrap: true },
    { column: 'timestamp', title: this.l('StartTime'), type: 'dateTime', width: 150 },
  ];

  loading = true;
  form: FormGroup;
  page = 1;
  perPage = AppConst.defaultTablePerPage;
  listSubscription$?: Subscription;
  showFilter = false; 
  matDialog = inject(MatDialog);
  constructor(private logService: AdminLogServiceProxy) {
    super();
    this.breadcrumb.list = [
      { name: this.l('Settings'), url: '/panel/setting' },
      { name: this.l('Maintenance'), url: '/panel/maintenance' },
    ];
    this.form = this.fb.group({
      filter: [''],
      filterLevels: [[]],
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

  public get LogLevel():typeof LogLevel{
return LogLevel;
  }

  getList(ev?: SortEvent) {
    if (this.listSubscription$) {
      this.listSubscription$.unsubscribe();
    }
    this.loading = true;
    const input = new GetLogsQuery();

    input.levels = this.form.get('filterLevels')?.value;
    input.search = this.form.get('filter')?.value;
    input.pageNumber = this.page;
    input.pageSize = this.perPage;
    input.sortBy = ev ? ev.field : '';
    input.sortDirection = ev ? ev.direction : '';
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
    this.router.navigate(['/panel/maintenance'], {
      queryParams: {
        page: this.page,
      },
    });
  }

  manulaCleanup() {
    this.confirmMessage(this.l('Delete'), this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        const input = new ManulaCleanupLogCommand();
        input.daysOld = 30;
        input.tenantId = this.tenantId ?? undefined;
        this.logService
          .manualCleanup(input)
          .pipe(
            finalize(() => {
              this.hideMainLoading();
              this.chdr.detectChanges();
            }),
          )
          .subscribe((response) => {
            this.notify.success(this.l('DoneSuccessFully'));
            this.alert.show({
              html: `<p>${this.l('CutOfDate')}: ${new LuxonFormatPipe().transform(response.data?.cutoffDate)}</p>
              <p>${this.l('DeletedCount')}: ${response.data?.deletedCount}</p>
              `,
            });
            this.getList();
          });
      }
    });
  }

  async openLogDetails(item: LogEntryDto) {
    const { LogDetailsComponent } = await import('./log-details/log-details.component');
    this.matDialog.open(LogDetailsComponent, {
      data: item,
    });
  }
}
