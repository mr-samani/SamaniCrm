import { result } from 'lodash-es';
import { DateTime } from 'luxon';
import { ConvertNumbersToLatin } from './conver-number-to-latin.helper';

export abstract class UtcDateTimeHelper {
  /**
   * تبدیل تاریخ و ساعت دریافتی از کاربر به فرمت یو تی سی
   * @param localDate تاریخ
   * @param localTime ساعت
   * @returns تاریخ و ساعت به صورت UTC
   */
  public static convertLocalDateTimeToUTC(localDate: DateTime, localTime: string): string {
    if (localDate instanceof DateTime) {
      localTime = ConvertNumbersToLatin.fixNumbers(localTime);
      let l1 = ConvertNumbersToLatin.fixNumbers(localDate.toFormat('yyyy/MM/dd'));
      let l2 = new Date(l1 + ' ' + localTime);
      let utcDate = l2.toISOString();
      // let hour = 0;
      // let min = 0;
      // const timeSplit = localTime.split(':');
      // if (localTime && timeSplit.length > 0) {
      //     hour = +timeSplit[0];
      //     min = +timeSplit[1];
      // }
      // let l1= localDate.startOf('day');
      // let l2=l1.plus({ hours: hour, minutes: min });
      // let l3=l2.toUTC()
      // const utcDate =l3.toISO();
      // console.log('localDate', localDate, 'localTime', localTime, 'utcDate', utcDate);
      return utcDate;
    } else {
      return '';
    }
  }

  /**
   * تبدیل تاریخ و ساعت یو تی سی به لوکال کاربر
   * @param datetime  UTC date time
   * @returns تاریخ و ساعت به لوکال کاربر
   */
  public static convertUtcToLocalDateTime(datetime: DateTime): LocalDateTime {
    let userTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    if (datetime instanceof DateTime === false) {
      datetime = DateTime.fromJSDate(new Date(datetime as any));
    }
    const localDateTime = datetime.setZone(userTimeZone);
    const result = {
      localDate: ConvertNumbersToLatin.fixNumbers(localDateTime.toFormat('yyyy/MM/dd')),
      localTime: ConvertNumbersToLatin.fixNumbers(localDateTime.toFormat('HH:mm')),
      localDateTime: localDateTime,
    };
    // console.log('convertUtcToLocalDateTime', datetime, result);
    return result;
  }
}

export interface LocalDateTime {
  localDate: string;
  localTime: string;

  localDateTime: DateTime;
}
