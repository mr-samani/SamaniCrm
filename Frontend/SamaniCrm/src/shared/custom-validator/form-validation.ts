import { AbstractControl, ValidatorFn, FormGroup } from '@angular/forms';
import { ConvertNumbersToLatin } from '@shared/helper/conver-number-to-latin.helper';

export abstract class CustomValidators {
  /**
   * بررسی صحت کد ملی
   * use invalid
   * @param control  string
   */
  static checkNationalCode(control: AbstractControl) {
    if (!control.value) return null;
    let nationalCode = ConvertNumbersToLatin.fixNumbers(control.value.toString());
    let isNumber = /^\d{10}$/.test(nationalCode);

    let isValid: boolean = false;

    if (isNumber && nationalCode.length == 10) {
      let allDigitEqual = [
        '0000000000',
        '1111111111',
        '2222222222',
        '3333333333',
        '4444444444',
        '5555555555',
        '6666666666',
        '7777777777',
        '8888888888',
        '9999999999',
      ];
      if (allDigitEqual.indexOf(nationalCode) > -1) isValid = false;
      else {
        let num0 = parseInt(nationalCode.charAt(0)) * 10;
        let num2 = parseInt(nationalCode.charAt(1)) * 9;
        let num3 = parseInt(nationalCode.charAt(2)) * 8;
        let num4 = parseInt(nationalCode.charAt(3)) * 7;
        let num5 = parseInt(nationalCode.charAt(4)) * 6;
        let num6 = parseInt(nationalCode.charAt(5)) * 5;
        let num7 = parseInt(nationalCode.charAt(6)) * 4;
        let num8 = parseInt(nationalCode.charAt(7)) * 3;
        let num9 = parseInt(nationalCode.charAt(8)) * 2;
        let a = parseInt(nationalCode.charAt(9));

        let b = num0 + num2 + num3 + num4 + num5 + num6 + num7 + num8 + num9;
        let c = b % 11;

        if ((c < 2 && a == c) || (c >= 2 && 11 - c == a)) isValid = true;
        else isValid = false;
      }
    } else isValid = false;

    if (isValid == true) return null;
    else return { invalid: true };
  }

  /**
   *  بررسی صحت شماره موبایل
   * use invalid
   * @param control
   */
  static checkMobileNumber(control: AbstractControl) {
    if (!control.value) return null;
    let mobileNumber = ConvertNumbersToLatin.fixNumbers(control.value.toString());
    let isNumber = /^\d{11}$/.test(mobileNumber);
    if (!isNumber) return { invalid: true };

    // validation logic pre number
    let validNumber = [
      // irancel
      '930',
      '933',
      '935',
      '936',
      '937',
      '938',
      '939',
      '900',
      '901',
      '902',
      '903',
      '904',
      '905',
      '941',
      // MCI
      '910',
      '911',
      '912',
      '913',
      '914',
      '915',
      '916',
      '917',
      '918',
      '919',
      '990',
      '991',
      '992',
      '993',
      '994',
      '995',
      '996', // MCI
      // Rightel
      '920',
      '921',
      '922',
      '923',
      // Espadan (MTCE)
      '931',
      // Taliya
      '921',
      // Kish-TCI
      '934',
      // Shatel
      '998',
      // samantel
      '999',
      //talia
      '932',
    ];
    let valid = false;
    // check validation mobile number
    for (let i = 0; i < validNumber.length; i++) {
      if (mobileNumber.toString().substring(1, 4) == validNumber[i]) valid = true;
    }

    if (valid) return null;
    else return { invalid: true };
  }

  /**
   * بررسی حروف فارسی
   * use pattern
   * @param control
   */
  static checkPersianCharacters(control: AbstractControl) {
    if (!control.value) return null;
    let word = control.value;
    let isValid = /^([\u0600-\u06FF]+\s*)+$/.test(word);
    if (isValid) return null;
    else return { pattern: true };
  }

  /**
   * بررسی حروف انگلیسی
   * use pattern
   * @param control
   */
  static checkEnglishCharacters(control: AbstractControl) {
    if (!control.value) return null;
    let word = control.value;
    let isValid = /^([a-zA-Z]+\s*)+$/.test(word);
    if (isValid) return null;
    else return { pattern: true };
  }
  /**
   * بررسی حروف انگلیسی و اعداد
   * با عدد شروع نشود
   *  use pattern
   * @param control
   */
  static globalVariable(control: AbstractControl) {
    if (!control.value) return null;
    let word = control.value;
    let isValid = /^[a-zA-Z_][a-zA-Z0-9_]*$/.test(word);
    if (isValid) return null;
    else return { pattern: true };
  }

  /**
   * بررسی حروف انگلیسی و اعداد
   * use pattern
   * @param control
   */
  static checkEnglishAndNumberCharacters(control: AbstractControl) {
    if (!control.value) return null;
    let word = control.value;
    let isValid = /^[a-zA-Z0-9-_]*$/.test(word);
    if (isValid) return null;
    else return { pattern: true };
  }

  /**
   * با عدد شروع نشود
   *
   * use startWithNumber
   */
  static notStartWithNumber(control: AbstractControl) {
    if (!control.value) return null;
    let word = control.value;
    let isValid = /^[^0-9]\S*/.test(word);
    if (isValid) return null;
    else return { startWithNumber: true };
  }

  /**
   * بررسی تلفن ثابت به همراه کد
   * 0xxxxxxxxxx
   * use pattern
   * @param control
   */
  static checkPhoneNumber(control: AbstractControl) {
    if (!control.value) return null;
    let word = ConvertNumbersToLatin.fixNumbers(control.value.toString());
    let isValid = /^((?:0)[0-9]{10})$/.test(word);
    if (isValid) return null;
    else return { pattern: true };
  }

  /**
   * بررسی صحیت ایمیل
   * a@b.c
   * use pattern
   * @param control
   */
  static checkEmail(control: AbstractControl) {
    if (!control.value) return null;
    let word = control.value;
    let isValid = /^[\w-.]+@([\w-]+\.)+[\w-]+$/.test(word); // /^.+@.+\..+$/.test(word);
    if (isValid) return null;
    else return { pattern: true };
  }

  /**
   * بررسی کاراکترهای عددی
   * use pattern
   * @param control
   */
  static checkNumberCharacters(control: AbstractControl) {
    if (!control.value) return null;
    let word = ConvertNumbersToLatin.fixNumbers(control.value);

    let isValid = /^-?(0|[1-9]\d*)?$/.test(word);
    if (isValid) return null;
    else return { pattern: true };
  }

  /**
   * حداقل 6 کاراکتر
   * حروف لاتین شامل اعداد و حروف بزرگ
   * @param control
   */
  static strongPassword(control: AbstractControl) {
    if (!control.value) return null;
    let word = control.value;
    let isValid = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%?&^#*()_\-=+])[A-Za-z\d$@$!%?&^#*()_\-=+]{6,}$/.test(
      word,
    );
    if (isValid) return null;
    else return { weakPassword: true };
  }
}
