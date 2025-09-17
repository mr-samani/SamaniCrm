import { Injectable, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, map, Observable, throwError } from 'rxjs';
import { Buffer } from 'buffer';
import { TokenService } from './token.service';
import { NgxAlertModalService } from 'ngx-alert-modal';
import { LanguageService } from './language.service';
import {
  AccountServiceProxy,
  LoginCommand,
  RefreshTokenCommand,
  RevokeRefreshTokenCommand,
  TwoFactorLoginCommand,
  UserDTO,
  UserServiceProxy,
} from '@shared/service-proxies';
import { TranslateService } from '@ngx-translate/core';
import { AppConst } from '@shared/app-const';
import { HttpErrorResponse } from '@angular/common/http';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<UserDTO | undefined> = new BehaviorSubject<UserDTO | undefined>(
    undefined,
  );
  public currentUser: Observable<UserDTO | undefined>;
  accountService: AccountServiceProxy;
  userService: UserServiceProxy;

  userRoles: string[] = [];
  userPermissions: string[] = [];
  constructor(
    private router: Router,
    private tokenService: TokenService,

    private alert: NgxAlertModalService,
    injector: Injector,
    private translateService: TranslateService,
    private languageService: LanguageService,
  ) {
    this.accountService = injector.get(AccountServiceProxy);
    this.userService = injector.get(UserServiceProxy);
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): UserDTO | undefined {
    return this.currentUserSubject.value;
  }

  login(input: LoginCommand) {
    return this.accountService.login(input).pipe(
      map((response) => {
        if (response.success && response.data && response.data.accessToken) {
          this.userRoles = response.data.roles ?? [];
          this.userPermissions = response.data.permissions ?? [];
          this.tokenService.set(response.data);
          this.currentUserSubject.next(response.data.user);
        }
        return response;
      }),
    );
  }
  loginTwoFactor(input: TwoFactorLoginCommand) {
    return this.accountService.loginTwoFactor(input).pipe(
      map((response) => {
        if (response.success && response.data && response.data.accessToken) {
          this.userRoles = response.data.roles ?? [];
          this.userPermissions = response.data.permissions ?? [];
          this.tokenService.set(response.data);
          this.currentUserSubject.next(response.data.user);
        }
        return response;
      }),
    );
  }
  // register(credential: RegisterRequest): Observable<any> {
  //   return this.dataService.post<RegisterRequest, LoginDto>(Apis.register, credential).pipe(
  //     map((response) => {
  //       if (response.success && response.data && response.data.token && response.data.user) {
  //         this.tokenService.set(response.data);
  //         this.currentUserSubject.next(response.data.user);
  //       }
  //       return response;
  //     }),
  //   );
  // }

  refreshToken(input: RefreshTokenCommand) {
    return this.accountService.refresh(input).pipe(
      map((response) => {
        if (response.success && response.data && response.data.accessToken) {
          this.tokenService.set(response.data);
          return response.data.accessToken;
        } else {
          this.logOut();
          this.alert
            .show({
              title: this.translateService.instant('Message.AccessDenied'),
              text: this.translateService.instant('Message.AccessDeniedMessage'),
              showConfirmButton: true,
              // showCancelButton: true,
              confirmButtonText: this.translateService.instant('Ok'),
              // cancelButtonText: 'انصراف'
            })
            .then((result) => {
              // if (result.isConfirmed) {
              this.router.navigate(['/account/login']);
              // }
            });
        }
        return '';
      }),
      catchError((err: HttpErrorResponse) => {
        return throwError(() => err);
      }),
    );
  }

  logOut() {
    this.accountService
      .revoke(
        new RevokeRefreshTokenCommand({
          token: this.tokenService.get().accessToken,
        }),
      )
      .subscribe();
    this.tokenService.remove();
    const returnUrl = window.location.href;
    this.router.navigate(['/account/login'], {
      queryParams: { returnUrl },
    });
  }

  getCurrentUserValue() {
    return new Promise((resolve, reject) => {
      return this.userService.getCurrentUser().subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.userRoles = response.data.roles ?? [];
            this.userPermissions = response.data.permissions ?? [];
            this.currentUserSubject.next(response.data);
            if (
              AppConst.currentLanguage !== response.data.lang &&
              AppConst.languageList.findIndex((x) => x.culture == response.data?.lang) > -1
            ) {
              AppConst.currentLanguage = response.data.lang!;
              this.languageService.changeLanguage(AppConst.currentLanguage, true);
            }
            resolve(true);
          } else {
            reject(false);
          }
        },
        error: (err) => {
          reject(false);
        },
      });
    });
  }

  isGranted(permission: string): boolean {
    if (!this.currentUserValue || !this.userPermissions) {
      return false;
    }
    return this.userPermissions.includes(permission);
  }
}

export const encodeBase64 = (data: string) => {
  return Buffer.from(encodeURIComponent(data)).toString('base64');
};
export const decodeBase64 = (data: string) => {
  return decodeURIComponent(Buffer.from(data, 'base64').toString('ascii'));
};
