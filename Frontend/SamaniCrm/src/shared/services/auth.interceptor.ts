import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpHeaders,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { catchError, take, filter, switchMap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { TranslateService } from '@ngx-translate/core';
import { TokenService } from './token.service';
import { AppConst } from '../app-const';
import { NgxAlertModalService } from 'ngx-alert-modal';
import { RefreshTokenCommand } from '@shared/service-proxies/model/refresh-token-command';
import { ApiError } from '@shared/service-proxies/model/api-error';
import { Router } from '@angular/router';
export const exceptionUrls = ['background.css', '/i18n/'];
@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(
    private tokenService: TokenService,
    private authService: AuthService,
    //private matDialog: MatDialog,
    private translateService: TranslateService,
    private alertService: NgxAlertModalService,
    private router: Router,
  ) {}
  setHeader(req: HttpRequest<any>): HttpRequest<any> {
    const lang = this.getLang(req);
    const token = this.tokenService.get();
    let modifiedHeaders = req.headers;
    if (token && token.accessToken) {
      modifiedHeaders = modifiedHeaders.set('Authorization', 'Bearer ' + token.accessToken);
    }
    if (lang) {
      modifiedHeaders = modifiedHeaders.set('lang', lang);
    }
    return req.clone({
      headers: modifiedHeaders,
    });
  }

  getLang(req: HttpRequest<any>): string {
    if (AppConst.currentLanguage !== '') {
      return AppConst.currentLanguage;
    } else {
      return localStorage.getItem('lang') || AppConst.defaultLang;
    }
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (exceptionUrls.findIndex((x) => req.url.includes(x)) > -1) {
      return next.handle(req);
    }
    return next.handle(this.setHeader(req)).pipe(
      catchError((err: HttpErrorResponse) => {
        return this.handleError(err, req, next);
      }),
    );
  }

  private handleError(error: HttpErrorResponse, request: HttpRequest<any>, next: HttpHandler) {
    switch (error.status) {
      case 401:
        if (
          error instanceof HttpErrorResponse &&
          !request.url.toLowerCase().includes('api/account/login') &&
          !request.url.toLowerCase().includes('api/account/refresh')
        ) {
          return this.handle401Error(request, next);
        }
        return throwError(() => error);
      case 400:
      case 422:
        return this.handleBadRequestError(error.error);
      case 403:
        return this.handleAccessDeniedError(error.error);
      default:
        return this.handleServerError(error);
    }
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);
      const token = this.tokenService.get();
      if (token && token.refreshToken) {
        return this.authService
          .refreshToken(
            new RefreshTokenCommand({
              refreshToken: token.refreshToken,
            }),
          )
          .pipe(
            switchMap((token) => {
              this.isRefreshing = false;
              this.refreshTokenSubject.next(token);
              return next.handle(this.setHeader(request));
            }),
            catchError((err: HttpErrorResponse) => {
              this.isRefreshing = false;
              if (err.status == 401) {
                this.authService.logOut();
              }
              // this.matDialog.closeAll();
              return throwError(() => err);
            }),
          );
      } else {
        return this.handleAccessDeniedError();
      }
    }
    return this.refreshTokenSubject.pipe(
      filter((token) => token !== null),
      take(1),
      switchMap((token) => next.handle(this.setHeader(request))),
    );
  }

  handleBadRequestError(err: any) {
    const msg = err?.message ?? this.translateService.instant('Message.InvalidYourRequest');
    let errorList = [];
    if (err?.errors && Array.isArray(err.errors)) {
      errorList = err.errors;
    } else if (err.request) {
      errorList = err.request;
    } else if (typeof err.errors == 'object') {
      errorList = Object.entries(err.errors).map((m) => {
        return { field: m[0], message: m[1] };
      });
    }
    let html = '<ol class="text-start">';
    for (let item of errorList) {
      html += `<li>${item.field}: ${item.message}</li>`;
    }
    html += '</ol>';
    this.alertService.show({
      title: msg,
      html: html,
      showCancelButton: false,
      showConfirmButton: true,
      confirmButtonText: this.translateService.instant('Ok'),
    });
    return throwError(() => err);
  }

  handleServerError(err: HttpErrorResponse) {
    const msg = err.error?.message ?? this.translateService.instant('Message.ErrorOccurred');
    this.alertService.show({
      title: msg,
      showCancelButton: false,
      showConfirmButton: true,
      confirmButtonText: this.translateService.instant('Ok'),
    });
    return throwError(() => err);
  }

  handleAccessDeniedError(err?: any) {
    const msg = err?.message ?? this.translateService.instant('Message.AccessDenied');
    this.alertService
      .show({
        title: msg,
        showCancelButton: true,
        showConfirmButton: true,
        confirmButtonText: this.translateService.instant('Ok'),
        cancelButtonText: this.translateService.instant('Cancel'),
      })
      .then((r) => {
        if (r.isConfirmed) {
          this.authService.logOut();
        } else {
          this.router.navigate(['/']);
        }
      });
    return throwError(() => err);
  }
}
