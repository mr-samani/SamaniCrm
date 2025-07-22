import { LanguageDTO } from './service-proxies';

export class AppConst {
  static appName = '';
  static languageList: LanguageDTO[] = [];
  static currentLanguage = '';
  static defaultLang = '';
  static isRtl: boolean;
  static apiUrl = '';
  static fileServerUrl = '';
  static baseUrl = '';
  static publicSiteUrl = '';
  static tinyMceApiKey = '';

  static requireCaptcha: boolean = true;

  static isDarkMode: boolean;

  static mainHeaderFixedTop: boolean = true;
}
