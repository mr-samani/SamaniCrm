import { PlatformLocation } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Injectable, isDevMode } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AppConst } from './app-const';
import { ColorSchemaService } from './services/color-schema.service';
import { LanguageService } from './services/language.service';
import { InitialAppDTOApiResponse } from './service-proxies/model/initial-app-dto-api-response';

@Injectable({
  providedIn: 'root',
})
export class AppInitializer {
  constructor(
    private platformLocation: PlatformLocation,
    private httpClient: HttpClient,
    private colorSchemeService: ColorSchemaService,
    private languageService: LanguageService,
  ) {
    // Load Color Scheme
    this.colorSchemeService.load();
  }

  public init(translate: TranslateService): () => Promise<boolean> {
    return () => {
      // AppConst.currentLanguage = localStorage.getItem('lang') || '';
      return new Promise((resolve, reject) => {
        let baseUrl = this.getDocumentOrigin() + this.getBaseHref();
        const rnd = Math.round(Math.random() * 1000);
        if (isDevMode()) {
          baseUrl = baseUrl + 'appconfig.json?v=' + rnd;
        } else {
          baseUrl = baseUrl + 'appconfig.production.json?v=' + rnd;
        }
        // app config
        this.httpClient.get(baseUrl).subscribe({
          next: (config: any) => {
            AppConst.apiUrl = config.apiUrl;
            AppConst.baseUrl = config.baseUrl;
            AppConst.tinyMceApiKey = config.tinyMceApiKey;
            AppConst.appName = config.appName;
            AppConst.publicSiteUrl = config.publicSiteUrl;
            AppConst.fileServerUrl = config.fileServerUrl;
            // api initialize
            this.httpClient.get<InitialAppDTOApiResponse>(AppConst.apiUrl + '/api/Common/InitialApp').subscribe({
              next: (resp) => {
                if (resp.success && resp.data) {
                  AppConst.languageList = resp.data.languages ?? [];
                  AppConst.defaultLang = resp.data.defaultLang ?? 'fa-IR';
                  AppConst.requireCaptcha = resp.data.requireCaptcha === true;
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
    };
  }

  private getBaseHref(): string {
    const baseUrl = this.platformLocation.getBaseHrefFromDOM();
    if (baseUrl) {
      return baseUrl;
    }

    return '/';
  }

  private getDocumentOrigin(): string {
    if (!document.location.origin) {
      const port = document.location.port ? ':' + document.location.port : '';
      return document.location.protocol + '//' + document.location.hostname + port;
    }

    return document.location.origin;
  }
}
