import { ExternalLoginCalbackComponent } from './external-login-calback/external-login-calback.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountRoutingModule } from './account-routing.module';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { AccountComponent } from './account.component';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { CaptchaModule } from '@shared/captcha/captcha.module';
import { SharedModule } from '@shared/shared.module';
import { AccountServiceProxy } from '@shared/service-proxies';

@NgModule({
  declarations: [AccountComponent, LoginComponent, RegisterComponent, ExternalLoginCalbackComponent],
  imports: [
    CommonModule,
    AccountRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    TranslateModule,
    SharedModule,
    CaptchaModule,
    MaterialCommonModule,
  ],
  providers: [AccountServiceProxy],
})
export class AccountModule {}
