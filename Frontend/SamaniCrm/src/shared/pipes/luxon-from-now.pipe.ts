import { Pipe, PipeTransform } from '@angular/core';
import { AppConst } from '@shared/app-const';
import { ConvertNumbersToLatin } from '@shared/helper/conver-number-to-latin.helper';
import { DateTime } from 'luxon';
import { isIsoDate } from './luxon-format.pipe';

@Pipe({ name: 'luxonFromNow', standalone: false })
export class LuxonFromNowPipe implements PipeTransform {
  transform(value: DateTime | string | number | undefined) {
    if (!value) {
      return '';
    }
    const localName = AppConst.currentLanguage;
    let userTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    if (typeof value === 'string') {
      if (isIsoDate(value)) {
        value = DateTime.fromISO(value as any);
      } else {
        value = DateTime.fromJSDate(new Date(value));
      }
    }
    if (value instanceof DateTime) {
      let str = value
        .setZone(userTimeZone)
        .setLocale(localName)
        .toRelative() ?? '';
      return ConvertNumbersToLatin.fixNumbers(str);
    } else {
      return '';
    }
  }
}
