import { inject } from '@angular/core';
import type { ResolveFn } from '@angular/router';
import { AppConst } from '@shared/app-const';
import { LanguageService } from '@shared/services/language.service';

export const accountResolver: ResolveFn<boolean> = (route, state) => {
  debugger;
  const languageService = inject(LanguageService);
  const currentLanguage = localStorage.getItem('lang');
  const lang = currentLanguage || AppConst.defaultLang;
  languageService.changeLanguage(lang, true);
  return true;
};
