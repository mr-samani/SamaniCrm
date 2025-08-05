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
  static dashboardUrl = '';
  static isDarkMode: boolean;
}
