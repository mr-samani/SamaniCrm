import { Component, Injector, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { SecuritySettingDto, SecuritySettingsServiceProxy, TwoFactorTypeEnum } from '@shared/service-proxies';
import { finalize } from 'rxjs/operators';
import { TwoFaAppConfigComponent } from '../../dialogs/two-fa-app-config/two-fa-app-config.component';

@Component({
  selector: 'app-security-setting',
  templateUrl: './security-setting.component.html',
  styleUrls: ['./security-setting.component.scss'],
  standalone: false,
})
export class SecuritySettingComponent extends AppComponentBase implements OnInit {
  settings: SecuritySettingDto = new SecuritySettingDto();
  isSaving = false;
  loading = true;
  logginAttemptMinute = 0;
  constructor(
    injector: Injector,
    private securitySettingService: SecuritySettingsServiceProxy,
    private matDialog: MatDialog,
  ) {
    super(injector);
  }

  ngOnInit() {
    this.getSettings();
  }

  public get TwoFactorTypeEnum(): typeof TwoFactorTypeEnum {
    return TwoFactorTypeEnum;
  }

  getSettings() {
    this.loading = true;
    this.securitySettingService
      .getSecuritySettings()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.settings = response.data ?? new SecuritySettingDto();
        this.logginAttemptMinute = (this.settings.logginAttemptTimeSecondsLimit ?? 0) / 60;
      });
  }

  save(frm: NgForm) {
    if (frm.invalid || !this.settings) {
      this.notify.warning(this.l('CompleteFormFields'));
      return;
    }

    this.settings.logginAttemptTimeSecondsLimit = this.logginAttemptMinute * 60;
    this.isSaving = true;
    this.securitySettingService
      .updateSecuritySettings(this.settings)
      .pipe(finalize(() => (this.isSaving = false)))
      .subscribe((response) => {
        this.notify.success(this.l('SaveSuccessFully'));
        this.getSettings();
      });
  }

  set2FaAppConfig() {
    this.matDialog
      .open(TwoFaAppConfigComponent, {
        data: {},
        width: '300px',
        disableClose: true,
      })
      .afterClosed()
      .subscribe((result) => {
        if (result == true) {
          this.settings.userSetting.isVerified = true;
        }
      });
  }
}
