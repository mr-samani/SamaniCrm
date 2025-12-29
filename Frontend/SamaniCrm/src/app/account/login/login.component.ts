import { HttpErrorResponse } from '@angular/common/http';
import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppConst } from '@shared/app-const';
import { CaptchaComponent } from '@shared/captcha/captcha.component';
import { AccountServiceProxy, ExternalProviderDto, ExternalProviderTypeEnum } from '@shared/service-proxies';
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

  loadingExternalProviders = false;
  externalProviderList: ExternalProviderDto[] = [];
  constructor(
    injector: Injector,
    private matDialog: MatDialog,
    private accountService: AccountServiceProxy,
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
    this.getExternalProviders();
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
              this.router.navigate(['/panel']);
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
              this.router.navigate(['/panel']);
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

  getExternalProviders() {
    this.loadingExternalProviders = true;
    this.accountService
      .getExternalProviders()
      .pipe(finalize(() => (this.loadingExternalProviders = false)))
      .subscribe((response) => {
        this.externalProviderList = response.data ?? [];
      });
  }

  async loginExternalProvider(provider: ExternalProviderDto) {
    this.loadingExternalProviders = true;
    try {
      const baseUrl = AppConst.apiUrl;
      const callbackUrl = `${baseUrl}/api/externalauth/callback/${provider.name.toLowerCase()}`;

      let url = '';
      switch (provider.providerType) {
        case ExternalProviderTypeEnum.OpenIdConnect:
          // Build OpenID Connect URL
          const params = new URLSearchParams({
            client_id: provider.clientId,
            redirect_uri: callbackUrl,
            response_type: provider.responseType || 'code',
            response_mode: provider.responseMode || 'query',
            scope: provider.scopes,
            state: this.generateState(), // Generate random state for security
            nonce: this.generateNonce(), // Generate nonce for OIDC
          });

          // Add PKCE if enabled
          if (provider.usePkce) {
            const codeVerifier = this.generateCodeVerifier();
            const codeChallenge = await this.generateCodeChallenge(codeVerifier);

            // Store code_verifier in session/local storage for later use
            sessionStorage.setItem('pkce_code_verifier', codeVerifier);

            params.append('code_challenge', codeChallenge);
            params.append('code_challenge_method', 'S256');
          }

          url = `${provider.authorizationEndpoint}?${params.toString()}`;
          break;
        case ExternalProviderTypeEnum.Microsoft:
        case ExternalProviderTypeEnum.Google:
        case ExternalProviderTypeEnum.GitHub:
        case ExternalProviderTypeEnum.LinkedIn:
          url =
            provider.authorizationEndpoint +
            `?client_id=${provider.clientId}&redirect_uri=${AppConst.baseUrl + '/account/external/' + provider.name}&response_type=code&scope=${provider.scopes}`;
          break;
        default:
          this.alert.show({
            title: this.l('Message.ExternalLoginNotAccessable'),
          });
          throw new Error('External login not supported');
      }
      window.location.href = url;
      console.log(url);
      //window.open(url, '_blank', 'noopener,noreferrer',);
    } catch (error) {
      console.error('Error occurred while handling external login:', error);
      this.loadingExternalProviders = false;
    }
  }

  // Helper methods for PKCE and security
  private generateState(): string {
    const array = new Uint8Array(32);
    crypto.getRandomValues(array);
    return Array.from(array, (byte) => byte.toString(16).padStart(2, '0')).join('');
  }

  private generateNonce(): string {
    const array = new Uint8Array(32);
    crypto.getRandomValues(array);
    return Array.from(array, (byte) => byte.toString(16).padStart(2, '0')).join('');
  }

  private generateCodeVerifier(): string {
    const array = new Uint8Array(32);
    crypto.getRandomValues(array);
    return this.base64URLEncode(array);
  }

  private async generateCodeChallenge(verifier: string): Promise<string> {
    const encoder = new TextEncoder();
    const data = encoder.encode(verifier);
    const hash = await crypto.subtle.digest('SHA-256', data);
    return this.base64URLEncode(new Uint8Array(hash));
  }

  private base64URLEncode(buffer: Uint8Array): string {
    const base64 = btoa(String.fromCharCode(...buffer));
    return base64.replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '');
  }
}
