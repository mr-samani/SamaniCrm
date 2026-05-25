import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { IWidgetBase } from '../IWidgetBase';
import { AdminLogServiceProxy, GetLogStatesQuery, LogStatsDto } from '@shared/service-proxies';
import { finalize } from 'rxjs';
import { DateTime } from 'luxon';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '@shared/shared.module';
import { MatProgressSpinner } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-log-stats',
  templateUrl: './log-stats.component.html',
  styleUrls: ['./log-stats.component.scss'],
  providers: [AdminLogServiceProxy],
  imports: [CommonModule, TranslateModule, SharedModule, MatProgressSpinner],
})
export class LogStatsComponent implements OnInit, IWidgetBase {
  editMode: boolean = false;
  loading = true;

  stats?: LogStatsDto;

  toDate = DateTime.now().toUTC();
  fromDate = DateTime.now().minus({ months: 3 }).toUTC();
  constructor(
    private logService: AdminLogServiceProxy,
    private chdr: ChangeDetectorRef,
  ) {}
  edit(): void {
    this.editMode = true;
  }

  ngOnInit() {
    this.getData();
  }

  getData() {
    this.loading = true;
    const input = new GetLogStatesQuery();
    input.toDate = this.toDate.toISO();
    input.fromDate = this.fromDate.toISO();
    this.logService
      .getStats(input)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.chdr.detectChanges();
        }),
      )
      .subscribe((result) => {
        this.stats = result.data;
      });
  }
}
