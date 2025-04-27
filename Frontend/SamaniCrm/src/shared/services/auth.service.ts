import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, map, Observable, of, throwError } from 'rxjs';
import { Buffer } from 'buffer';
import { TokenService } from './token.service';
import { NgxAlertModalService } from 'ngx-alert-modal';
import { AppConst } from '../app-const';
import { LanguageService } from './language.service';
import { Apis } from '@shared/apis';
import { AccountServiceProxy, LoginCommand, RefreshTokenCommand, UserResponseDTO } from '@shared/service-proxies';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<UserResponseDTO | undefined> = new BehaviorSubject<
    UserResponseDTO | undefined
  >(undefined);
  public currentUser: Observable<UserResponseDTO | undefined>;
  constructor(
    private http: HttpClient,
    private router: Router,
    private tokenService: TokenService,

    private alert: NgxAlertModalService,
    private language: LanguageService,
    private accountService: AccountServiceProxy,
  ) {
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): UserResponseDTO | undefined {
    return this.currentUserSubject.value;
  }

  login(input: LoginCommand) {
    return this.accountService.login(input).pipe(
      map((response) => {
        if (response.success && response.data && response.data.accessToken) {
          this.tokenService.saveToken(response.data.accessToken);
          this.currentUserSubject.next({
            id: response.data.userId!,
            fullName: response.data.fullName,
            email: response.data.email,
            userName: response.data.userName,
            profilePicture: response.data.profilePicture,
            // roles:response.data.roles,
          });
        }
        return response;
      }),
    );
  }

  // register(credential: RegisterRequest): Observable<any> {
  //   return this.dataService.post<RegisterRequest, LoginDto>(Apis.register, credential).pipe(
  //     map((response) => {
  //       if (response.success && response.data && response.data.token && response.data.user) {
  //         this.tokenService.saveToken(response.data.token);
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
          this.tokenService.updateToken(response.data.accessToken);
          return response.data.accessToken;
        } else {
          this.logOut();
          this.alert
            .show({
              title: 'خطای دسترسی',
              text: 'اعتبار دسترسی شما به اتمام رسیده است. لطفاً مجددا وارد حساب کاربری خود شوید.',
              showConfirmButton: true,
              // showCancelButton: true,
              confirmButtonText: 'تائید',
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
    this.tokenService.removeTokens();
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
