import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { BASE_PATH, Configuration } from '../shared/service-proxies';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    { provide: Configuration, useFactory: configurationFactory },
  ],
};
function configurationFactory() {
  var config = new Configuration();
  config.basePath = 'https://localhost:44342';
  return config;
}
