import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { Apis } from '@shared/apis';
import { finalize } from 'rxjs';
import { AppSettingsDto } from './models/app-settings';
import { NgForm } from '@angular/forms';
import { ApiResult } from '@shared/models/api-result';

@Component({
  selector: 'app-app-setting',
  templateUrl: './app-setting.component.html',
  styleUrls: ['./app-setting.component.css'],
})
export class AppSettingComponent extends AppComponentBase implements OnInit {
  loading = true;

  settings?: AppSettingsDto;
  isSaving = false;

  constructor(injector: Injector) {
    super(injector);
    this.breadcrumb.list = [
      { name: this.l('Settings'), url: '/dashboard/setting' },
      { name: this.l('AppSetting'), url: '/dashboard/app-setting' },
    ];
  }

  ngOnInit(): void {
    this.getSettings();
  }

  getSettings() {
    this.loading = true;
    this.dataService
      .get<any, AppSettingsDto>(Apis.getAllSettings, {})
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.settings = response.result;
      });
  }

  save(frm: NgForm) {
    if (frm.invalid || !this.settings) {
      this.notify.warning(this.l('CompleteFormFields'));
      return;
    }

    this.isSaving = true;
    this.dataService
      .post<{ settings: AppSettingsDto }, null>(Apis.saveSettings, { settings: this.settings })
      .pipe(finalize(() => (this.isSaving = false)))
      .subscribe((response) => {
        this.notify.success(this.l('SaveSuccessFully'));
        this.getSettings();
      });
  }
}
