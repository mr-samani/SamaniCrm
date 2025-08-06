import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
  isDevMode,
  provideAppInitializer,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import {
  provideHttpClient,
  withInterceptors,
} from '@angular/common/http';
import { TranslocoHttpLoader } from './transloco-loader';
import { provideTransloco } from '@jsverse/transloco';
import { appInit } from '@shared/app-initializer';
import { Configuration } from '@shared/service-proxies/configuration';
import { AppConst } from '@shared/app-const';
import { AppInterceptor } from '@shared/services/app.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideAppInitializer(appInit),
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideClientHydration(withEventReplay()),
    provideHttpClient(withInterceptors([AppInterceptor])),
    provideTransloco({
      config: {
        availableLangs: ['en-US', 'fa-IR', 'ar'],
        defaultLang: 'en-US',
        // Remove this option if your application doesn't support changing language in runtime.
        reRenderOnLangChange: true,
        prodMode: !isDevMode(),
      },
      loader: TranslocoHttpLoader,
    }),

    { provide: Configuration, useFactory: configurationFactory },
  ],
};
function configurationFactory() {
  var config = new Configuration();
  config.basePath = AppConst.apiUrl; //'https://localhost:44343';
  return config;
}
