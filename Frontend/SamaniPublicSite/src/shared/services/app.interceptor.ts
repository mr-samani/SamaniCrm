import {
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { TokenService } from './token.service';
import { AppConst } from '../app-const';
import { NgxAlertModalService } from 'ngx-alert-modal';
import { Router } from '@angular/router';
import { TranslocoService } from '@jsverse/transloco';
import { StoreService } from './localstore.service';

const exceptionUrls = ['background.css', '/i18n/'];


export const AppInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
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
  

      if (error.status === 400 || error.status === 422) {
        return handleBadRequestError(error.error);
      }

      if (error.status === 403 || error.status === 401) {
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
        showConfirmButton: true,
        confirmButtonText: translateService.translate('Ok'),
      })
      .then(() => {
          router.navigate(['/']);
      });

    return throwError(() => err);
  }
};
