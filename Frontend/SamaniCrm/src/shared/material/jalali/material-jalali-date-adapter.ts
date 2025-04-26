import { Optional, Inject } from '@angular/core';
import { DateAdapter, MatDateFormats, MAT_DATE_LOCALE } from '@angular/material/core';
import { DateTime } from 'luxon';
import { JalaliDate, GregorianJalaliHelper, LONG_MONTHS, NARROW_MONTHS, SHORT_MONTHS } from './jalali-date';

/**
 * Use like this:
 * { provide: DateAdapter, useClass: MaterialJalaliDateAdapter, deps: [MAT_DATE_LOCALE] },
   { provide: MAT_DATE_FORMATS, useValue: PERSIAN_DATE_FORMATS },
 */

export const PERSIAN_DATE_FORMATS: MatDateFormats = {
  parse: {
    dateInput: 'YYYY/MM/DD',
  },
  display: {
    dateInput: 'YYYY/MM/DD',
    monthYearLabel: 'YYYY MMMM',
    dateA11yLabel: 'YYYY/MM/DD',
    monthYearA11yLabel: 'YYYY MMMM',
  },
};

export class MaterialJalaliDateAdapter extends DateAdapter<JalaliDate> {
  private readonly dayNames: string[] = [...new Array(31)].map((_, i) => (i + 1).toString());

  constructor(@Optional() @Inject(MAT_DATE_LOCALE) dateLocale: string) {
    super();
    super.setLocale(dateLocale);
  }

  getYear(date: JalaliDate): number {
    return this.clone(date).year;
  }

  getMonth(date: JalaliDate): number {
    return this.clone(date).month - 1;
  }

  getDate(date: JalaliDate): number {
    return this.clone(date).day;
  }

  getDayOfWeek(date: JalaliDate): number {
    return this.clone(date).dayOfWeek();
  }

  getMonthNames(style: 'long' | 'short' | 'narrow'): string[] {
    switch (style) {
      case 'long':
        return LONG_MONTHS;
      case 'short':
        return SHORT_MONTHS;
      default:
        // case 'narrow':
        return NARROW_MONTHS;
    }
  }

  getDateNames(): string[] {
    return this.dayNames;
  }

  getDayOfWeekNames(style: 'long' | 'short' | 'narrow'): string[] {
    switch (style) {
      case 'long':
        return ['یکشنبه', 'دوشنبه', 'سه\u200Cشنبه', 'چهارشنبه', 'پنجشنبه', 'جمعه', 'شنبه'];
      case 'short':
        return ['یک', 'دو', 'سه', 'چهار', 'پنج', 'جمعه', 'شنبه'];
      default:
        // case 'narrow':
        return ['ی', 'د', 'س', 'چ', 'پ', 'ج', 'ش'];
    }
  }

  getYearName(date: JalaliDate): string {
    return this.clone(date).year.toString();
  }

  getFirstDayOfWeek(): number {
    return 6;
  }

  getNumDaysInMonth(date: JalaliDate): number {
    if (date instanceof DateTime) {
      date = GregorianJalaliHelper.fromGregorian(date);
    }
    let daysPerMonth = GregorianJalaliHelper.getDaysPerMonth(date.month, date.year);
    return daysPerMonth;
  }

  clone(date: any): JalaliDate {
    if (date instanceof DateTime) {
      return GregorianJalaliHelper.fromGregorian(date);
    } else if (date instanceof JalaliDate) {
      return date.clone();
    } else if (DateTime.fromSQL(date).isValid) {
      return GregorianJalaliHelper.fromGregorian(DateTime.fromSQL(date));
    } else {
      throw new Error('input date is invalid');
    }
    // return date.clone().locale('fa-IR');
  }

  createDate(year: number, month: number, date: number): JalaliDate {
    if (month < 0 || month > 12) {
      throw new Error(`Invalid month index "${month}". Month index has to be between 0 and 11.`);
    }
    if (date < 1) {
      throw new Error(`Invalid date "${date}". Date has to be greater than 0.`);
    }

    let result: any = new JalaliDate(year, month + 1, date);
    let grogorien = GregorianJalaliHelper.toGregorian(result);
    result = DateTime.fromJSDate(grogorien).startOf('day');
    // if (this.getMonth(result) !== month) {
    //   throw new Error(`Invalid date ${date} for month with index ${month}.`);
    // }
    if (!result.isValid) {
      throw new Error(`Invalid date "${date}" for month with index "${month}".`);
    }
    return result;
  }

  today(): JalaliDate {
    return GregorianJalaliHelper.fromGregorian(DateTime.now());
  }

  parse(value: any, parseFormat: string | string[]): JalaliDate | null {
    if (typeof value === 'string') {
      return JalaliDate.parse(value, parseFormat);
    }
    return null;
  }

  format(date: JalaliDate, displayFormat: string): string {
    date = this.clone(date);
    if (!this.isValid(date)) {
      throw new Error(
        'JalaliMomentDateAdapter: Cannot format invalid date. date=' +
          JSON.stringify(date) +
          ',displayFormat=' +
          displayFormat,
      );
    }
    return date.format(displayFormat);
  }

  addCalendarYears(date: JalaliDate, years: number): JalaliDate {
    return this.clone(date).addYears(years);
  }

  addCalendarMonths(date: JalaliDate, months: number): JalaliDate {
    return this.clone(date).addMonths(months);
  }

  addCalendarDays(date: JalaliDate, days: number): JalaliDate {
    return this.clone(date).addDays(days);
  }

  toIso8601(date: JalaliDate): string {
    return this.clone(date).format('YYYY-MM-DD');
  }

  isDateInstance(obj: any): boolean {
    return obj instanceof JalaliDate || obj instanceof DateTime || DateTime.fromSQL(obj).isValid;
  }

  isValid(date: JalaliDate): boolean {
    return this.clone(date).isValid();
  }

  invalid(): JalaliDate {
    return new JalaliDate(-1, -1, -1);
  }

  override deserialize(value: any): JalaliDate | null {
    let date;
    if (value instanceof Date) {
      date = GregorianJalaliHelper.fromGregorian(value);
    }
    if (typeof value === 'string') {
      if (!value) {
        return null;
      }
    }
    if (date && this.isValid(date)) {
      return date;
    }
    return super.deserialize(value);
  }
}
