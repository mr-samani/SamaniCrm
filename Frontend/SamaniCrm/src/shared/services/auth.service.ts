import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, map, Observable, of, throwError } from 'rxjs';
import { Buffer } from 'buffer';
import { TokenService } from './token.service';
import { NgxAlertModalService } from 'ngx-alert-modal';
import { LanguageService } from './language.service';
import {
  AccountServiceProxy,
  LoginCommand,
  RefreshTokenCommand,
  RevokeRefreshTokenCommand,
  UserResponseDTO,
} from '@shared/service-proxies';
import { TranslateService } from '@ngx-translate/core';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<UserResponseDTO | undefined> = new BehaviorSubject<
    UserResponseDTO | undefined
  >(undefined);
  public currentUser: Observable<UserResponseDTO | undefined>;
  accountService: AccountServiceProxy;
  constructor(
    private http: HttpClient,
    private router: Router,
    private tokenService: TokenService,

    private alert: NgxAlertModalService,
    injector: Injector,
    private translateService: TranslateService,
  ) {
    this.accountService = injector.get(AccountServiceProxy);
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): UserResponseDTO | undefined {
    return this.currentUserSubject.value;
  }

  login(input: LoginCommand) {
    return this.accountService.login(input).pipe(
      map((response) => {
        if (response.success && response.data && response.data.accessToken) {
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
      catchError((err: any, caught: Observable<any>) => {
        return err;
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
    this.router.navigate(['/account/login']);
  }

  // getCurrentUserValue() {
  //   return new Promise((resolve, reject) => {
  //     return this.dataService.get<any, UserResponseDTO>(Apis.getCurrentUser, {}).subscribe({
  //       next: (response) => {
  //         if (response.success && response.data) {
  //           this.currentUserSubject.next(response.data);
  //           AppConst.currentLanguage = response.data.lang!;
  //           this.language.changeLanguage(AppConst.currentLanguage, true);
  //           resolve(true);
  //         } else {
  //           reject(false);
  //         }
  //       },
  //       error: (err) => {
  //         reject(false);
  //       },
  //     });
  //   });
  // }
}

export const encodeBase64 = (data: string) => {
  return Buffer.from(encodeURIComponent(data)).toString('base64');
};
export const decodeBase64 = (data: string) => {
  return decodeURIComponent(Buffer.from(data, 'base64').toString('ascii'));
};
