import {
  APP_INITIALIZER,
  ApplicationConfig,
  ErrorHandler,
  importProvidersFrom,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { TranslateModule } from '@ngx-translate/core';
import { NgxAlertModalModule } from 'ngx-alert-modal';
import { AppConst } from '@shared/app-const';
import { AppInitializer } from '@shared/app-initializer';
import { AuthInterceptor } from '@shared/services/auth.interceptor';
import { AccountServiceProxy } from '@shared/service-proxies/api/account.service';
import { Configuration } from '@shared/service-proxies/configuration';
import { GlobalErrorHandler } from '@shared/handlers/global-error-handler';
import { UserServiceProxy } from '@shared/service-proxies';

export const appConfig: ApplicationConfig = {
  providers: [
    // provideZonelessChangeDetection(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    importProvidersFrom(
      // ServiceWorkerModule.register('ngsw-worker.js', {
      //   enabled: !isDevMode(),
      //   // Register the ServiceWorker as soon as the application is stable
      //   // or after 30 seconds (whichever comes first).
      //   registrationStrategy: 'registerWhenStable:30000',
      // }),
      NgxAlertModalModule,
      TranslateModule.forRoot(),
      MatSnackBarModule,
    ),
    AccountServiceProxy,
    UserServiceProxy,
    {
      provide: APP_INITIALIZER,
      useFactory: (appInitializer: AppInitializer) => appInitializer.init(),
      deps: [AppInitializer],
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    { provide: Configuration, useFactory: configurationFactory },
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
  ],
};
function configurationFactory() {
  var config = new Configuration();
  config.basePath = 'https://localhost:44343';
  return config;
}

export function getRemoteServiceBaseUrl(): string {
  return AppConst.apiUrl;
}
