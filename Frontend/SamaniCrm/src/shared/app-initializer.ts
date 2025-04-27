import { PlatformLocation } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Injectable, isDevMode } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AppConst } from './app-const';
import { InitializeDto } from './models/initialize-dto';
import { ColorSchemaService } from './services/color-schema.service';
import { LanguageService } from './services/language.service';

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
      AppConst.currentLanguage = localStorage.getItem('lang') || '';
      return new Promise((resolve, reject) => {
        let baseUrl = this.getDocumentOrigin() + this.getBaseHref();
        if (isDevMode()) {
          baseUrl = baseUrl + 'appconfig.json';
        } else {
          baseUrl = baseUrl + 'appconfig.production.json';
        }
        // app config
        this.httpClient.get(baseUrl).subscribe({
          next: (config: any) => {
            AppConst.apiUrl = config.apiUrl;
            AppConst.baseUrl = config.baseUrl;
            AppConst.tinyMceApiKey = config.tinyMceApiKey;
            AppConst.appName = config.appName;

            AppConst.languageList = [];
            AppConst.defaultLang = 'fa';
            AppConst.requireCaptcha = true;
            let lang = 'fa';
            translate.setDefaultLang(lang);
            this.languageService.changeLanguage(lang, true);
            resolve(true);
            // api initialize
            // this.httpClient
            //   .get<{
            //     result: InitializeDto;
            //     success: string;
            //     message: string;
            //   }>(AppConst.apiUrl + '/api/index/initialize')
            //   .subscribe({
            //     next: (resp) => {
            //       AppConst.languageList = resp.result.languages ?? [];
            //       AppConst.defaultLang = resp.result.defaultLang;
            //       AppConst.requireCaptcha = resp.result.requireCaptcha;
            //       let lang = resp.result.currentLanguage || resp.result.defaultLang;
            //       translate.setDefaultLang(lang);
            //       this.languageService.changeLanguage(lang, true);
            //       resolve(true);
            //     },
            //     error: (er) => {
            //       reject();
            //     },
            //   });
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
