import { Component, inject, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { IWidgetBase } from '../IWidgetBase';
import { Widget } from '../widgets';
import { LastLoginDto, SecurityLogServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { MatProgressSpinner } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-last-login-infos',
  templateUrl: './last-login-infos.component.html',
  styleUrls: ['./last-login-infos.component.scss'],
  imports: [CommonModule, SharedModule, MatProgressSpinner],
  providers: [SecurityLogServiceProxy],
})
export class LastLoginInfosComponent extends AppComponentBase implements OnInit, IWidgetBase {
  item!: Widget;
  @Input('item') set setItem(val: Widget) {
    this.item = val;
  }

  list: LastLoginDto[] = [];
  loading = true;
  logService = inject(SecurityLogServiceProxy);
  constructor() {
    super();
  }
  ngOnInit() {
    this.getData();
  }
  edit(): void {
    throw new Error('Method not implemented.');
  }

  getData() {
    this.loading = true;
    this.logService
      .getLastLoginList()
      .pipe(
        finalize(() => {
          this.loading = false;
          this.chdr.detectChanges();
        }),
      )
      .subscribe((result) => {
        this.list = result.data ?? [];
      });
  }
}
