import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, map, Observable, of, throwError } from 'rxjs';
import { Buffer } from 'buffer';
import { TokenService } from './token.service';
import { NgxAlertModalService } from 'ngx-alert-modal';
import { AppConst } from '../app-const';
import { LanguageService } from './language.service';
import { DataService } from './data-service.service';
import { LoginRequest } from '@app/account/models/login-request';
import { Apis } from '@shared/apis';
import { LoginDto, UserDto } from '@app/account/models/login-dto';
import { RegisterRequest } from '@app/account/models/register-request';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<UserDto | undefined>(undefined);
  public currentUser: Observable<UserDto | undefined>;
  constructor(
    private http: HttpClient,
    private router: Router,
    private tokenService: TokenService,
    private dataService: DataService,
    private alert: NgxAlertModalService,
    private language: LanguageService,
  ) {
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): UserDto | undefined {
    return this.currentUserSubject.value;
  }

  login(credential: LoginRequest) {
    return this.dataService.post<LoginRequest, LoginDto>(Apis.login, credential).pipe(
      map((response) => {
        if (response.success && response.result && response.result.token && response.result.user) {
          this.tokenService.saveToken(response.result.token, credential.rememberMe);
          this.currentUserSubject.next(response.result.user);
        }
        return response;
      }),
    );
  }

  register(credential: RegisterRequest): Observable<any> {
    return this.dataService.post<RegisterRequest, LoginDto>(Apis.register, credential).pipe(
      map((response) => {
        if (response.success && response.result && response.result.token && response.result.user) {
          this.tokenService.saveToken(response.result.token);
          this.currentUserSubject.next(response.result.user);
        }
        return response;
      }),
    );
  }

  refreshToken(token: string) {
    return this.dataService.post<any, LoginDto>(Apis.refresh, {}).pipe(
      map((response) => {
        if (response.success && response.result && response.result.token) {
          this.tokenService.updateToken(response.result.token);
          return response.result.token;
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

  getCurrentUserValue() {
    return new Promise((resolve, reject) => {
      return this.dataService.get<any, UserDto>(Apis.getCurrentUser, {}).subscribe({
        next: (response) => {
          if (response.success && response.result) {
            this.currentUserSubject.next(response.result);
            AppConst.currentLanguage = response.result.lang!;
            this.language.changeLanguage(AppConst.currentLanguage, true);
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
