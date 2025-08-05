/*
۰۱۲۳۴۵۶۷۸۹
٠١٢٣٤٥٦٧٨٩
0123456789
*/
export abstract class ConvertNumbersToLatin {
  private static persianNumbers = [/۰/g, /۱/g, /۲/g, /۳/g, /۴/g, /۵/g, /۶/g, /۷/g, /۸/g, /۹/g];
  private static arabicNumbers = [/٠/g, /١/g, /٢/g, /٣/g, /٤/g, /٥/g, /٦/g, /٧/g, /٨/g, /٩/g];

  public static fixNumbers(str: string): string {
    if (typeof str === 'string') {
      for (let i = 0; i < 10; i++) {
        str = str
          .replace(ConvertNumbersToLatin.persianNumbers[i], i.toString())
          .replace(ConvertNumbersToLatin.arabicNumbers[i], i.toString());
      }
    }
    return str;
  }
}
