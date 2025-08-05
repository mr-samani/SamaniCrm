import { DOCUMENT, isPlatformBrowser } from '@angular/common';
import { inject, Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { AppConst } from '../app-const';
import { isRtl } from '@shared/helper/is-rtl';
import { finalize } from 'rxjs/operators';
import { MainSpinnerService } from './main-spinner.service';
import { UserServiceProxy } from '@shared/service-proxies/api/user.service';
import { StoreService } from './localstore.service';

@Injectable({
  providedIn: 'root',
})
export class LanguageService {
  direction: string = 'ltr';
  //userService: UserServiceProxy;
  constructor(
    @Inject(DOCUMENT) private _document: Document,
    private mainSpinner: MainSpinnerService,
    private storeService: StoreService
  ) {
    // this.userService = inject(UserServiceProxy);
    // this.changeLanguage(AppConst.currentLanguage);
    // this.translate.onLangChange.subscribe(param => {
    //     AppConst.currentLanguage = param.lang;
    //     this.storeService.setItem('lang', param.lang);
    //     // setTimeout(() => {
    //     //     location.reload();
    //     // }, 100);
    // });
  }

  init() {
    let lang = this.storeService.getItem<string>('lang') || AppConst.defaultLang;
    this.changeLanguage(lang, true, true);
  }

  changeLanguage(lang: string, dontSave = false, isInit = false) {
    lang = this.validateLanguage(lang);

    this.storeService.setItem('lang', lang);

    AppConst.currentLanguage = lang;
    AppConst.isRtl = isRtl(lang);
    this.direction = AppConst.isRtl ? 'rtl' : 'ltr';
    //this.translate.use(lang);
    this._document.body.dir = this.direction;
    this._document.documentElement.lang = lang.substring(0, 2);
    this._document.documentElement.dir = this.direction;
    if (dontSave == false) {
      this.mainSpinner.showLoading = true;
      // this.userService
      //   .updateCurrentuserLanguage(AppConst.currentLanguage)
      //   .pipe(finalize(() => (this.mainSpinner.showLoading = false)))
      //   .subscribe((r) => {
      //     if (r.data == true) location.reload();
      //   });
      if (!isInit) this._document.location.reload();
    } else {
      if (!isInit) this._document.location.reload();
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
