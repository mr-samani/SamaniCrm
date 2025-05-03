import { HttpErrorResponse } from '@angular/common/http';
import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppConst } from '@shared/app-const';
import { CaptchaComponent } from '@shared/captcha/captcha.component';
import { InputCaptchaDTO } from '@shared/service-proxies/model/input-captcha-dto';
import { LoginCommand } from '@shared/service-proxies/model/login-command';
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
  constructor(injector: Injector) {
    super(injector);
    this.loginForm = this.fb.group({
      userName: ['', [Validators.required]],
      password: ['', [Validators.required]],
      captcha: [new InputCaptchaDTO({captchaKey:'',captchaText:''})],
      rememberMe: [true],
    });
  }

  ngOnInit(): void {}

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
        next: (result: any) => {
          this.notify.success(this.l('Message.LoginSuccess'));
          this.router.navigate(['/dashboard']);
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
