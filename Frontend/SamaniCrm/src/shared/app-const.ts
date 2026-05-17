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
  static multiTenancy: boolean = true;

  static isDarkMode: boolean;

  static mainHeaderFixedTop: boolean = true;

  // default values
  static defaultTablePerPage = 10;

  /** must be set with tenant resolver */
  static tenantId: string;
  static tenancyName: string;
}
