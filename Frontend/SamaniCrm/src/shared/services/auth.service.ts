import { Injectable, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { Buffer } from 'buffer';
import { LanguageService } from './language.service';
import { AppConst } from '@shared/app-const';
import { removePreviousFolderId } from '@app/file-manager/consts/PreviousFolderId';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { UserDTO } from '@shared/service-proxies/model/user-dto';
import { AccountServiceProxy } from '@shared/service-proxies/api/account.service';
import { UserServiceProxy } from '@shared/service-proxies/api/user.service';
import { LoginCommand } from '@shared/service-proxies/model/login-command';
import { TwoFactorLoginCommand } from '@shared/service-proxies/model/two-factor-login-command';
import { ExternalLoginCallbackCommand } from '@shared/service-proxies/model/external-login-callback-command';
import { isNullOrEmpty } from '@shared/helper/null-or-empty';
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
    private languageService: LanguageService,
    private oidcSecurityService: OidcSecurityService,
    injector: Injector,
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
        return response;
      }),
    );
  }

  loginTwoFactor(input: TwoFactorLoginCommand) {
    return this.accountService.loginTwoFactor(input).pipe(
      map((response) => {
        // if (response.success && response.data && response.data.accessToken) {
        //   this.userRoles = response.data.roles ?? [];
        //   this.userPermissions = response.data.permissions ?? [];
        //   this.tokenService.set(response.data);
        //   this.currentUserSubject.next(response.data.user);
        // }
        return response;
      }),
    );
  }

  externalLoginCallback(input: ExternalLoginCallbackCommand) {
    return this.accountService.externalLoginCallback(input).pipe(
      map((response) => {
        // if (response.success && response.data && response.data.accessToken) {
        //   this.userRoles = response.data.roles ?? [];
        //   this.userPermissions = response.data.permissions ?? [];
        //   this.tokenService.set(response.data);
        //   this.currentUserSubject.next(response.data.user);
        // }
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

  refreshToken(input: any) {
    this.oidcSecurityService.getRefreshToken();
    // return this.accountService.refresh(input).pipe(
    //   map((response) => {
    //     if (response.success && response.data && response.data.accessToken) {
    //       this.tokenService.set(response.data);
    //       return response.data.accessToken;
    //     } else {
    //       this.logout();
    //       this.alert
    //         .show({
    //           title: this.translateService.instant('Message.AccessDenied'),
    //           text: this.translateService.instant('Message.AccessDeniedMessage'),
    //           showConfirmButton: true,
    //           // showCancelButton: true,
    //           confirmButtonText: this.translateService.instant('Ok'),
    //           // cancelButtonText: 'انصراف'
    //         })
    //         .then((result) => {
    //           // if (result.isConfirmed) {
    //           this.router.navigate(['/account/login']);
    //           // }
    //         });
    //     }
    //     return '';
    //   }),
    //   catchError((err: HttpErrorResponse) => {
    //     return throwError(() => err);
    //   }),
    // );
  }

  logout() {
    // no need to this - logout handled by back end
    // this.oidcSecurityService.logoff();
    this.accountService.logout().subscribe();
    removePreviousFolderId();
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
            AppConst.isHost = isNullOrEmpty(response.data.tenantId);
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
