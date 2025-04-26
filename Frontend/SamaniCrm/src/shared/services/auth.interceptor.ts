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
export const exceptionUrls = ['background.css', 'assets/i18n/'];
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
  ) {}
  setHeader(req: HttpRequest<any>): HttpRequest<any> {
    const lang = this.getLang(req);
    const token = this.tokenService.getToken();
    let modifiedHeaders = req.headers;
    if (token) {
      modifiedHeaders = modifiedHeaders.set('Authorization', 'Bearer ' + token);
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
      return localStorage.getItem('lang') || '';
    }
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (exceptionUrls.findIndex((x) => req.url.includes(x)) > -1) {
      return next.handle(req);
    }
    return next.handle(this.setHeader(req)).pipe(
      catchError((err: any) => {
        return this.handleError(err, req, next);
      }),
    );
  }

  private handleError(error: any, request: HttpRequest<any>, next: HttpHandler) {
    switch (error.status) {
      case 401:
        if (
          error instanceof HttpErrorResponse &&
          !request.url.includes('api/account/login') &&
          !request.url.includes('api/account/refresh')
        ) {
          return this.handle401Error(request, next);
        } else {
          return this.handleAccessDeniedError(error.error);
        }
      case 400:
      case 422:
        return this.handleBadRequestError(error.error);
      default:
        return this.handleServerError(error);
    }
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);
      const token = this.tokenService.getToken();
      if (token) {
        return this.authService.refreshToken(token).pipe(
          switchMap((token) => {
            this.isRefreshing = false;
            this.refreshTokenSubject.next(token);
            return next.handle(this.setHeader(request));
          }),
          catchError((err) => {
            this.isRefreshing = false;
            this.authService.logOut();
            // this.matDialog.closeAll();
            return throwError(() => new Error(err));
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
    const msg = err?.message ?? this.translateService.instant('Message.ErrorOccurred');
    const errorList: any = err.validation ?? {};
    let html = '';
    for (let item of Object.entries(errorList)) {
      // نام فیلد
      // html += item[0];
      const str: any = item && item[1] ? item[1] : [];
      for (let s of str) {
        html += '<p>' + s + '</p>';
      }
    }
    this.alertService.show({
      title: msg,
      html: html,
      showCancelButton: false,
      showConfirmButton: true,
      confirmButtonText: this.translateService.instant('ok'),
    });
    return throwError(() => new Error(err as any));
  }

  handleServerError(err: HttpErrorResponse) {
    const msg = err.error?.message ?? this.translateService.instant('Message.ErrorOccurred');
    this.alertService.show({
      title: msg,
      showCancelButton: false,
      showConfirmButton: true,
      confirmButtonText: this.translateService.instant('ok'),
    });
    return throwError(() => new Error(err as any));
  }

  handleAccessDeniedError(err?: any) {
    const msg = err?.message ?? this.translateService.instant('pleaseLogin');
    this.alertService
      .show({
        title: msg,
        showCancelButton: false,
        showConfirmButton: true,
        confirmButtonText: this.translateService.instant('ok'),
      })
      .then((r) => {
        this.authService.logOut();
      });
    return throwError(() => new Error(err as any));
  }
}
