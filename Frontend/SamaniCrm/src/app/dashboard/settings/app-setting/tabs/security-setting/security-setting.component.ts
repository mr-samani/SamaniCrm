import { Component, Injector, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { SecuritySettingDTO, SecuritySettingsServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-security-setting',
  templateUrl: './security-setting.component.html',
  styleUrls: ['./security-setting.component.scss'],
  standalone: false,
})
export class SecuritySettingComponent extends AppComponentBase implements OnInit {
  settings?: SecuritySettingDTO ;
  isSaving = false;
  loading = true;

  constructor(
    injector: Injector,
    private securitySettingService: SecuritySettingsServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit() {
    this.getSettings();
  }

  onChangeTwoFactor() {
    // if (this.settings?.twoFactorAuthentication.enable == false) {
    //   this.settings!.twoFactorAuthentication.viaEmail = false;
    //   this.settings!.twoFactorAuthentication.viaSMS = false;
    // } else if (
    //   this.settings?.twoFactorAuthentication.enable &&
    //   !this.settings!.twoFactorAuthentication.viaEmail &&
    //   !this.settings!.twoFactorAuthentication.viaSMS
    // ) {
    //   this.settings!.twoFactorAuthentication.viaEmail = true;
    // }
  }

  getSettings() {
    this.loading = true;
    this.securitySettingService
      .getSecuritySettings()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.settings = response.data ;
        // if (this.settings && this.settings.twoFactorAuthentication) {
        //   if (this.settings.twoFactorAuthentication.viaEmail || this.settings.twoFactorAuthentication.viaSMS) {
        //     this.settings.twoFactorAuthentication.enable = true;
        //   }
        // }
      });
  }

  save(frm: NgForm) {
    if (frm.invalid || !this.settings) {
      this.notify.warning(this.l('CompleteFormFields'));
      return;
    }

    this.isSaving = true;
    this.securitySettingService.updateSecuritySettings(this.settings) 
      .pipe(finalize(() => (this.isSaving = false)))
      .subscribe((response) => {
        this.notify.success(this.l('SaveSuccessFully'));
        this.getSettings();
      });
  }
}
