import { HttpErrorResponse } from '@angular/common/http';
import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { ExternalLoginCallbackCommand } from '@shared/service-proxies/model/external-login-callback-command';

@Component({
  selector: 'app-external-login-calback',
  templateUrl: './external-login-calback.component.html',
  styleUrls: ['./external-login-calback.component.scss'],
  standalone: false,
})
export class ExternalLoginCalbackComponent extends AppComponentBase implements OnInit {
  code = '';
  loading = true;
  returnUrl = '';
  provider = '';
  constructor(injector: Injector) {
    super(injector);
    this.provider = this.route.snapshot.params['provider'];
    this.code = this.route.snapshot.queryParams['code'];
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'];
  }

  ngOnInit() {
    this.authService
      .externalLoginCallback(new ExternalLoginCallbackCommand({ code: this.code, provider: this.provider }))
      .subscribe({
        next: (result) => {
          if (result.data?.accessToken) {
            this.notify.success(this.l('Message.LoginSuccess'));
            if (this.returnUrl) {
              window.location.href = this.returnUrl;
            } else {
              this.router.navigate(['/dashboard']);
            }
          }else{
            throw new Error('Login failed');
          }
        },
        error: (error: HttpErrorResponse) => {
          if (error.status == 401) {
            this.notify.warning(this.l('Message.UsernameOrPasswordIsInvalid'));
          }
          this.router.navigate(['/account/login']);
        },
      });
  }
}
