import { DOCUMENT } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AppConst } from '../app-const';
import { Apis } from '@shared/apis';

import { UserResponseDTO } from '@app/account/models/login-dto';
import { MainSpinnerService } from './main-spinner.service';
import { finalize, pipe } from 'rxjs';

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
    localStorage.setItem('lang', lang);
    AppConst.currentLanguage = lang;
    AppConst.isRtl = ['fa', 'ar'].indexOf(lang) > -1;
    this.direction = AppConst.isRtl ? 'rtl' : 'ltr';
    this.translate.use(lang);
    this._document.body.dir = this.direction;
    document.documentElement.lang = this.translate.currentLang;
    document.documentElement.dir = this.direction;
    if (dontSave == false) {
      this.mainSpinner.showLoading = true;
      this.dataService
        .post<any, UserResponseDTO>(Apis.changeUserLanguage + '/' + AppConst.currentLanguage, {
          lang: AppConst.currentLanguage,
        })
        .pipe(finalize(() => (this.mainSpinner.showLoading = false)))
        .subscribe((r) => {
          location.reload();
        });
    }
  }
}
