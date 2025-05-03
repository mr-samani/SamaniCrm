import { DOCUMENT } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AppConst } from '../app-const';
import { MainSpinnerService } from './main-spinner.service';
import { isRtl } from '@shared/helper/is-rtl';

@Injectable({
  providedIn: 'root',
})
export class LanguageService {
  direction: string = 'ltr';
  constructor(
    public translate: TranslateService,
    @Inject(DOCUMENT) private _document: Document,

    private mainSpinner: MainSpinnerService,
  ) {
    // this.changeLanguage(AppConst.currentLanguage);
    // this.translate.onLangChange.subscribe(param => {
    //     AppConst.currentLanguage = param.lang;
    //     localStorage.setItem('lang', param.lang);
    //     // setTimeout(() => {
    //     //     location.reload();
    //     // }, 100);
    // });
  }

  changeLanguage(lang: string, dontSave = false) {
    lang = this.validateLanguage(lang);
    localStorage.setItem('lang', lang);
    AppConst.currentLanguage = lang;
    AppConst.isRtl = isRtl(lang);
    this.direction = AppConst.isRtl ? 'rtl' : 'ltr';
    this.translate.use(lang);
    this._document.body.dir = this.direction;
    document.documentElement.lang = this.translate.currentLang;
    document.documentElement.dir = this.direction;
    if (dontSave == false) {
      this.mainSpinner.showLoading = true;
      // this.dataService
      //   .post<any, UserResponseDTO>(Apis.changeUserLanguage + '/' + AppConst.currentLanguage, {
      //     lang: AppConst.currentLanguage,
      //   })
      //   .pipe(finalize(() => (this.mainSpinner.showLoading = false)))
      //   .subscribe((r) => {
      //     location.reload();
      //   });
    }
  }

  validateLanguage(lang: string) {
    if (AppConst.languageList.findIndex((x) => x.culture === lang) > -1) {
      return lang;
    } else {
      return AppConst.defaultLang;
    }
  }
}
