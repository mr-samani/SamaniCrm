import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppConst } from '@shared/app-const';
import { CaptchaComponent } from '@shared/captcha/captcha.component';
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
      email: ['', [Validators.required]],
      password: ['', [Validators.required]],
      captchaKey: [''],
      captcha: [''],
      captchaResponse: [''],
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
    let formValue = Object.assign({}, this.loginForm.value);
    formValue.captcha = formValue.captchaResponse?.captcha;
    formValue.captchaKey = formValue.captchaResponse?.key;
    delete formValue.captchaResponse;
    this.authService
      .login(formValue)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (result: any) => {
          this.notify.success(this.l('Message.LoginSuccess'));
          this.router.navigate(['/dashboard']);
        },
        error: (error: any) => {
          this.notify.warning(this.l('Message.UsernameOrPasswordIsInvalid'));
          if (this.captcha && AppConst.requireCaptcha) this.captcha.reloadCaptcha();
        },
      });
  }
}
