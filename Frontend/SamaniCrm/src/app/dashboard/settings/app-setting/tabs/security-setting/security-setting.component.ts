import { Component, Input, OnInit } from '@angular/core';
import { SecuritySettingDto } from '../../models/app-settings';
import { ControlContainer, NgForm } from '@angular/forms';

@Component({
  selector: 'app-security-setting',
  templateUrl: './security-setting.component.html',
  styleUrls: ['./security-setting.component.css'],
  viewProviders: [
    {
      provide: ControlContainer,
      useExisting: NgForm,
    },
  ],
  standalone: false,
})
export class SecuritySettingComponent implements OnInit {
  @Input() settings?: SecuritySettingDto;

  constructor() {}

  ngOnInit() {
    if (this.settings && this.settings.twoFactorAuthentication) {
      if (this.settings.twoFactorAuthentication.viaEmail || this.settings.twoFactorAuthentication.viaSMS) {
        this.settings.twoFactorAuthentication.enable = true;
      }
    }
  }

  onChangeTwoFactor() {
    if (this.settings?.twoFactorAuthentication.enable == false) {
      this.settings!.twoFactorAuthentication.viaEmail = false;
      this.settings!.twoFactorAuthentication.viaSMS = false;
    } else if (
      this.settings?.twoFactorAuthentication.enable &&
      !this.settings!.twoFactorAuthentication.viaEmail &&
      !this.settings!.twoFactorAuthentication.viaSMS
    ) {
      this.settings!.twoFactorAuthentication.viaEmail = true;
    }
  }
}
