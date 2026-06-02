import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { TranslateService } from '@ngx-translate/core';
import { AppConst } from '../app-const';
import { NgxAlertModalService } from 'ngx-alert-modal';
import { Router } from '@angular/router';
export const exceptionUrls = ['background.css', '/i18n/'];
@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private authService: AuthService,
    //private matDialog: MatDialog,
    private translateService: TranslateService,
    private alertService: NgxAlertModalService,
    private router: Router,
  ) {}
  setHeader(req: HttpRequest<any>): HttpRequest<any> {
    const lang = this.getLang(req);
    let modifiedHeaders = req.headers;
    if (lang) {
      modifiedHeaders = modifiedHeaders.set('lang', lang);
    }
    return req.clone({
      headers: modifiedHeaders,
      withCredentials: true,
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
          !request.url.toLowerCase().includes('api/account/external') &&
          !request.url.toLowerCase().includes('api/account/loginTwoFactor') &&
          !request.url.toLowerCase().includes('api/account/refresh')
        ) {
          return this.handleAccessDeniedError(request);
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
      html += `<li>${item.field ?? ''}: ${item.message}</li>`;
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
    this.translateService.get('Message.ErrorOccurred').subscribe((lmsg) => {
      const msg =
        (err.error?.message ?? lmsg != 'Message.ErrorOccurred')
          ? lmsg
          : 'Unfortunately, an error has occurred on the server!';
      let errorList = [];
      if (err.error && err.error.errors && Array.isArray(err.error.errors)) {
        errorList = err.error.errors;
      }
      let html = '';
      for (let item of errorList) {
        html += `<p>${item.message}</p>`;
      }
      this.alertService.show({
        title: errorList.length > 0 ? '' : msg,
        html: html,
        showCancelButton: false,
        showConfirmButton: true,
        confirmButtonText: this.translateService.instant('Ok'),
      });
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
          this.authService.logout();
        } else {
          this.router.navigate(['/']);
        }
      });
    return throwError(() => err);
  }
}
