export class AppConst {
  static appName = '';
  static languageList: {
    code: string;
    title: string;
    image: string;
  }[] = [];
  static currentLanguage = '';
  static defaultLang = '';
  static isRtl: boolean;
  static apiUrl = '';
  static baseUrl = '';
  static tinyMceApiKey = '';

  static requireCaptcha: boolean = true;

  static isDarkMode: boolean;
}
