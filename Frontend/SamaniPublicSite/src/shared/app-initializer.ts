import { isPlatformBrowser, PlatformLocation } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { DOCUMENT, inject, isDevMode, PLATFORM_ID } from '@angular/core';
import { AppConst } from './app-const';
import { ColorSchemaService } from './services/color-schema.service';
import { InitialAppDTOApiResponse } from './service-proxies/model/initial-app-dto-api-response';
import { LanguageService } from './services/language.service';

export function appInit(): Promise<boolean> {
  return new Promise((resolve, reject) => {
    const httpClient = inject(HttpClient);
    const colorSchemeService = inject(ColorSchemaService);
    const languageService = inject(LanguageService);
    colorSchemeService.load();
    let baseUrl = getDocumentOrigin() + getBaseHref();
    const rnd = Math.round(Math.random() * 1000);
    if (isDevMode()) {
      baseUrl = baseUrl + 'appconfig.json?v=' + rnd;
    } else {
      baseUrl = baseUrl + 'appconfig.production.json?v=' + rnd;
    }
    console.log('start load config');
    readConfigFile()
      .then((config) => {
        AppConst.apiUrl = config.apiUrl;
        AppConst.baseUrl = config.baseUrl;
        AppConst.appName = config.appName;
        AppConst.fileServerUrl = config.fileServerUrl;
        AppConst.dashboardUrl = config.dashboardUrl;
        // console.log(config);

        // api initialize
        httpClient.get<InitialAppDTOApiResponse>(AppConst.apiUrl + '/api/Common/InitialApp').subscribe({
          next: (resp) => {
            if (resp.success && resp.data) {
              AppConst.languageList = resp.data.languages ?? [];
              AppConst.defaultLang = resp.data.defaultLang ?? 'fa-IR';
              languageService.init();
              resolve(true);
            } else {
              reject();
            }
          },
          error: (er) => {
            console.error('error on get init api from backend:', er);
            reject();
          },
        });
      })
      .catch((er) => {
        console.error('error on load config:', er);
        reject(er);
      });
  });
}

async function readConfigFile(): Promise<any> {
  return new Promise(async (resolve, reject) => {
    const platformId = inject(PLATFORM_ID);
    const httpClient = inject(HttpClient);
    try {
      if (isPlatformBrowser(platformId)) {
        let baseUrl = getDocumentOrigin() + getBaseHref();
        const rnd = Math.round(Math.random() * 1000);
        if (isDevMode()) {
          baseUrl = baseUrl + 'appconfig.json?v=' + rnd;
        } else {
          baseUrl = baseUrl + 'appconfig.production.json?v=' + rnd;
        }
        // app config
        httpClient.get(baseUrl).subscribe({
          next: (response) => {
            resolve(response);
          },
          error: (err) => {
            reject(err);
          },
        });
      } else {
        console.log('init node js for read file');
        const fs = await import('node:fs');
        const path = await import('node:path');
        const filePath = path.join(process.cwd(), './public/appconfig.json');
        const file = fs.readFileSync(filePath, 'utf-8');
        const config = JSON.parse(file);
        resolve(config);
      }
    } catch (error) {
      reject(error);
    }
  });
}

function getBaseHref(): string {
  const platformLocation = inject(PlatformLocation);
  const baseUrl = platformLocation.getBaseHrefFromDOM();
  if (baseUrl) {
    return baseUrl;
  }

  return '/';
}

function getDocumentOrigin(): string {
  const document = inject(DOCUMENT);
  if (!document.location.origin) {
    const port = document.location.port ? ':' + document.location.port : '';
    return document.location.protocol + '//' + document.location.hostname + port;
  }

  return document.location.origin;
}
