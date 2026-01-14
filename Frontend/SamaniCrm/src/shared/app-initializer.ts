import { PlatformLocation } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { inject, isDevMode } from '@angular/core';
import { AppConst } from './app-const';
import { LanguageService } from './services/language.service';
import { InitialAppDTOApiResponse } from './service-proxies/model/initial-app-dto-api-response';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ColorSchemaService } from './services/color-schema.service';

export function AppInitializer(): Promise<boolean> {
  const platformLocation = inject(PlatformLocation);
  const httpClient = inject(HttpClient);
  const colorSchemeService = inject(ColorSchemaService);
  const languageService = inject(LanguageService);

  // Load Color Scheme
  colorSchemeService.load();
  // AppConst.currentLanguage = localStorage.getItem('lang') || '';
  return new Promise((resolve, reject) => {
    let baseUrl = getDocumentOrigin() + getBaseHref(platformLocation);
    const rnd = Math.round(Math.random() * 1000);
    if (isDevMode()) {
      baseUrl = baseUrl + 'appconfig.json?v=' + rnd;
    } else {
      baseUrl = baseUrl + 'appconfig.production.json?v=' + rnd;
    }
    // app config
    httpClient.get(baseUrl).subscribe({
      next: (config: any) => {
        AppConst.apiUrl = config.apiUrl;
        AppConst.baseUrl = config.baseUrl;
        AppConst.tinyMceApiKey = config.tinyMceApiKey;
        AppConst.appName = config.appName;
        AppConst.publicSiteUrl = config.publicSiteUrl;
        AppConst.fileServerUrl = config.fileServerUrl;
        // api initialize
        httpClient.get<InitialAppDTOApiResponse>(AppConst.apiUrl + '/api/Common/InitialApp').subscribe({
          next: (resp) => {
            if (resp.success && resp.data) {
              AppConst.languageList = resp.data.languages ?? [];
              AppConst.defaultLang = resp.data.defaultLang ?? 'fa-IR';
              AppConst.requireCaptcha = resp.data.requireCaptcha === true;
              const loader = new TranslateHttpLoader(httpClient, AppConst.apiUrl + '/api/language/i18n/', '');
              languageService.translate.currentLoader = loader;
              resolve(true);
            } else {
              reject();
            }
          },
          error: (er) => {
            reject();
          },
        });
      },
      error: (er) => {
        reject(er);
      },
    });
  });
}

function getBaseHref(platformLocation: PlatformLocation): string {
  const baseUrl = platformLocation.getBaseHrefFromDOM();
  if (baseUrl) {
    return baseUrl;
  }

  return '/';
}

function getDocumentOrigin(): string {
  if (!document.location.origin) {
    const port = document.location.port ? ':' + document.location.port : '';
    return document.location.protocol + '//' + document.location.hostname + port;
  }

  return document.location.origin;
}
