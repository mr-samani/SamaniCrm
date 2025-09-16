import { HttpErrorResponse } from '@angular/common/http';
import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppConst } from '@shared/app-const';
import { CaptchaComponent } from '@shared/captcha/captcha.component';
import { InputCaptchaDTO } from '@shared/service-proxies/model/input-captcha-dto';
import { LoginCommand } from '@shared/service-proxies/model/login-command';
import { TwoFactorLoginCommand } from '@shared/service-proxies/model/two-factor-login-command';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  animations: [accountModuleAnimation()],
  standalone: false,
})
export class LoginComponent extends AppComponentBase implements OnInit {
  loginForm: FormGroup;
  loading = false;
  @ViewChild('captcha') captcha!: CaptchaComponent;
  requiredCaptcha = AppConst.requireCaptcha;
  showPassword = false;

  returnUrl?: string;

  getTwoFactorCode = false;
  code = '';
  constructor(
    injector: Injector,
    private matDialog: MatDialog,
  ) {
    super(injector);
    this.loginForm = this.fb.group({
      userName: ['', [Validators.required]],
      password: ['', [Validators.required]],
      captcha: [new InputCaptchaDTO({ captchaKey: '', captchaText: '' })],
      rememberMe: [true],
    });
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'];
  }

  ngOnInit(): void {
    this.matDialog.closeAll();
  }

  login() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      this.notify.warning(this.l('Message.EnterUsernameAndPassword'));
      return;
    }
    this.loading = true;
    let formValue: LoginCommand = this.loginForm.value;
    formValue.captcha = new InputCaptchaDTO(this.loginForm.get('captcha')?.value);
    this.authService
      .login(formValue)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (result) => {
          if (result.success && result.data?.accessToken) {
            this.notify.success(this.l('Message.LoginSuccess'));
            if (this.returnUrl) {
              window.location.href = this.returnUrl;
            } else {
              this.router.navigate(['/dashboard']);
            }
          } else if (result.success && result.data?.enableTwoFactor) {
            this.getTwoFactorCode = true;
          }
        },
        error: (error: HttpErrorResponse) => {
          if (error.status == 401) {
            this.notify.warning(this.l('Message.UsernameOrPasswordIsInvalid'));
          }
          if (this.captcha && AppConst.requireCaptcha) this.captcha.reloadCaptcha();
        },
      });
  }

  loginWithTwoFactor() {
    if (!this.code) {
      this.notify.warning(this.l('CompleteFormFields'));
      return;
    }
    this.loading = true;
    let input = new TwoFactorLoginCommand();
    input.code = this.code.toString();
    input.userName = this.loginForm.get('userName')?.value;
    input.password = this.loginForm.get('password')?.value;
    this.authService
      .loginTwoFactor(input)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (result) => {
          if (result.success && result.data?.accessToken) {
            this.notify.success(this.l('Message.LoginSuccess'));
            if (this.returnUrl) {
              window.location.href = this.returnUrl;
            } else {
              this.router.navigate(['/dashboard']);
            }
          }
        },
        error: (error: HttpErrorResponse) => {
          if (error.status == 401) {
            this.notify.warning(this.l('Message.UsernameOrPasswordIsInvalid'));
          }
          if (this.captcha && AppConst.requireCaptcha) this.captcha.reloadCaptcha();
        },
      });
  }
}
