import { Pipe, PipeTransform } from '@angular/core';
import { DateTime } from 'luxon';
import { AppConst } from '../app-const';
import { ConvertNumbersToLatin } from '../helper/conver-number-to-latin.helper';
import { Calendars } from './Calendars';
@Pipe({ name: 'luxonFormat', standalone: false })
export class LuxonFormatPipe implements PipeTransform {
  transform(value: DateTime | string | number | undefined, format: string) {
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
        .reconfigure({
          outputCalendar: this.getCalendar,
        })
        .toFormat(format);
      return ConvertNumbersToLatin.fixNumbers(str);
    } else {
      return '';
    }
  }
  private get getCalendar(): string {
    switch (AppConst.currentLanguage) {
      case 'fa-IR':
      case 'fa':
        return Calendars.Persian;
      case 'ar':
        return Calendars.Islamic;
      case 'hi':
        return Calendars.Indian;
      default:
        return Calendars.iso8601;
    }
  }
}
export function isIsoDate(str: string) {
  if (!/\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{3,6}Z/.test(str)) return false;
  const d = new Date(str);
  return d instanceof Date; // valid date
}
