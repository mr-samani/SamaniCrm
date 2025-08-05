import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, BehaviorSubject, throwError, of } from 'rxjs';
import { catchError, take, filter, switchMap } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { TokenService } from './token.service';
import { AppConst } from '../app-const';
import { NgxAlertModalService } from 'ngx-alert-modal';
import { RefreshTokenCommand } from '@shared/service-proxies/model/refresh-token-command';
import { Router } from '@angular/router';
import { TranslocoService } from '@jsverse/transloco';
import { StoreService } from './localstore.service';

const exceptionUrls = ['background.css', '/i18n/'];

let isRefreshing = false;
const refreshTokenSubject = new BehaviorSubject<any>(null);

export const AppInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const authService = inject(AuthService);
  const alertService = inject(NgxAlertModalService);
  const router = inject(Router);
  const translateService = inject(TranslocoService);
  const storeService = inject(StoreService);

  if (exceptionUrls.some((x) => req.url.includes(x))) {
    return next(req);
  }

  const modifiedReq = setAuthHeaders(req);

  return next(modifiedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (
        error.status === 401 &&
        !req.url.toLowerCase().includes('api/account/login') &&
        !req.url.toLowerCase().includes('api/account/refresh')
      ) {
        return handle401Error(modifiedReq, next);
      }

      if (error.status === 400 || error.status === 422) {
        return handleBadRequestError(error.error);
      }

      if (error.status === 403) {
        return handleAccessDeniedError(error.error);
      }

      return handleServerError(error);
    })
  );

  function setAuthHeaders(request: HttpRequest<any>): HttpRequest<any> {
    const lang = AppConst.currentLanguage || storeService.getItem('lang') || AppConst.defaultLang;
    const token = tokenService.get();

    let headers = request.headers;
    if (token?.accessToken) {
      headers = headers.set('Authorization', `Bearer ${token.accessToken}`);
    }
    if (lang) {
      headers = headers.set('lang', lang);
    }

    return request.clone({ headers });
  }

  function handle401Error(request: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> {
    if (!isRefreshing) {
      isRefreshing = true;
      refreshTokenSubject.next(null);

      const token = tokenService.get();
      if (token?.refreshToken) {
        return authService.refreshToken(new RefreshTokenCommand({ refreshToken: token.refreshToken })).pipe(
          switchMap((newToken) => {
            isRefreshing = false;
            refreshTokenSubject.next(newToken);
            return next(setAuthHeaders(request));
          }),
          catchError((err) => {
            isRefreshing = false;
            authService.logOut();
            return throwError(() => err);
          })
        );
      } else {
        return handleAccessDeniedError();
      }
    }

    return refreshTokenSubject.pipe(
      filter((token) => token !== null),
      take(1),
      switchMap(() => next(setAuthHeaders(request)))
    );
  }

  function handleBadRequestError(err: any) {
    const msg = err?.message ?? translateService.translate('Message.InvalidYourRequest');
    let errorList: { field: string; message: string }[] = [];

    if (Array.isArray(err?.errors)) {
      errorList = err.errors;
    } else if (typeof err.errors === 'object') {
      errorList = Object.entries(err.errors).map(([field, message]) => ({ field, message } as any));
    }

    const html = `<ol class="text-start">${errorList.map((e) => `<li>${e.field}: ${e.message}</li>`).join('')}</ol>`;

    alertService.show({
      title: msg,
      html,
      showCancelButton: false,
      showConfirmButton: true,
      confirmButtonText: translateService.translate('Ok'),
    });

    return throwError(() => err);
  }

  function handleServerError(err: HttpErrorResponse) {
    const msg = err?.error?.message ?? translateService.translate('Message.ErrorOccurred');
    alertService.show({
      title: msg,
      showCancelButton: false,
      showConfirmButton: true,
      confirmButtonText: translateService.translate('Ok'),
    });
    return throwError(() => err);
  }

  function handleAccessDeniedError(err?: any) {
    const msg = err?.message ?? translateService.translate('Message.AccessDenied');
    alertService
      .show({
        title: msg,
        showCancelButton: true,
        showConfirmButton: true,
        confirmButtonText: translateService.translate('Ok'),
        cancelButtonText: translateService.translate('Cancel'),
      })
      .then((result) => {
        if (result.isConfirmed) {
          authService.logOut();
        } else {
          router.navigate(['/']);
        }
      });

    return throwError(() => err);
  }
};
