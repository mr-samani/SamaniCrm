import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, map, Observable, of, throwError } from 'rxjs';
import { Buffer } from 'buffer';
import { NgxAlertModalService } from 'ngx-alert-modal';
import { LanguageService } from './language.service';
import {
  AccountServiceProxy,
  LoginCommand,
  RefreshTokenCommand,
  RevokeRefreshTokenCommand,
  UserDTO,
  UserServiceProxy,
} from '@shared/service-proxies';
import { AppConst } from '@shared/app-const';
import { TokenService } from './token.service';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<UserDTO | undefined> = new BehaviorSubject<UserDTO | undefined>(
    undefined
  );
  public currentUser: Observable<UserDTO | undefined>;
  accountService: AccountServiceProxy;
  userService: UserServiceProxy;
  constructor(
    private router: Router,
    private tokenService: TokenService,

    private alert: NgxAlertModalService,
    injector: Injector,
    private languageService: LanguageService
  ) {
    this.accountService = injector.get(AccountServiceProxy);
    this.userService = injector.get(UserServiceProxy);
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): UserDTO | undefined {
    return this.currentUserSubject.value;
  }

  logOut(gotoLogin = false) {
    this.accountService
      .revoke(
        new RevokeRefreshTokenCommand({
          token: this.tokenService.get().accessToken,
        })
      )
      .subscribe();
    this.tokenService.remove();
    const returnUrl = window.location.href;
    if (gotoLogin) {
      this.router.navigate(['/account/login'], {
        queryParams: { returnUrl },
      });
    }
  }

  getCurrentUserValue() {
    return new Promise((resolve, reject) => {
      return this.userService.getCurrentUser().subscribe({
        next: (response) => {
          if (response.success && response.data) {
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
}

export const encodeBase64 = (data: string) => {
  return Buffer.from(encodeURIComponent(data)).toString('base64');
};
export const decodeBase64 = (data: string) => {
  return decodeURIComponent(Buffer.from(data, 'base64').toString('ascii'));
};
