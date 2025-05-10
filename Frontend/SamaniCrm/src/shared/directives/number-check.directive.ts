import { Directive, ElementRef, HostListener, Input, OnInit, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { DOWN_ARROW, UP_ARROW } from '@angular/cdk/keycodes';
import { isNullOrEmpty } from '@shared/helper/null-or-empty';
import { ConvertNumbersToLatin } from '@shared/helper/conver-number-to-latin.helper';

@Directive({
  standalone: false,
  selector: '[numberCheck]',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => NumberCheckDirective),
      multi: true,
    },
  ],
})
export class NumberCheckDirective implements OnInit, ControlValueAccessor {
  /**
   * thousands separator
   */
  @Input() thousandsSeparator: boolean = false;
  /**
   * decimal number
   */
  @Input() isDecimal: boolean = false;
  /**
   * Ability to enter negative numbers
   */
  @Input() allowNegative: boolean = true;
  /**
   * By focusing on the input, its value will be selected
   */
  @Input() selectAllOnFocus: boolean = true;
  /**
   * Remove extra decimal places
   * * Note: The original number will also change
   */
  @Input() maxDecimalDigit?: number;

  @Input() step: number = 1;
  @Input() min?: number | string;
  @Input() max?: number | string;

  value: string | number = '';
  previewsValue?: string | number;
  isDisabled = false;
  private _regEx!: RegExp;
  private _regExNumber: RegExp = new RegExp(/[^0-9]*/g);
  private _regExNumberAndDecimal: RegExp = new RegExp(/[^0-9/.]*/g);
  private _regExNumberForNegative: RegExp = new RegExp(/[^0-9/-]*/g);
  private _regExNumberAndDecimalForNegative: RegExp = new RegExp(/[^0-9/.-]*/g);

  _onChange = (value: any) => {};
  _onTouched = () => {};
  _onValidatorChange = () => {};

  constructor(private el: ElementRef<HTMLInputElement>) {}

  registerOnChange(fn: any): void {
    this._onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this._onTouched = fn;
  }
  setDisabledState(disabled: boolean): void {
    this.isDisabled = disabled;
  }

  writeValue(obj: any): void {
    // console.log('write value', obj);
    this.value = obj;
    this.previewsValue = obj;
    this.checkNumbers();
    this.checkMinMax();
    this.changed();
  }

  ngOnInit(): void {
    // if type is number on type dot has removed in numbercheck
    if (this.isDecimal && this.el.nativeElement.getAttribute('type') == 'number') {
      this.el.nativeElement.setAttribute('type', 'text');
    }
  }

  @HostListener('focus', ['$event']) onInputFocus(ev: any): void {
    if (this.selectAllOnFocus) ev.currentTarget?.select();
  }

  @HostListener('input', ['$event']) onInputChange(event: Event | any): void {
    // console.log('input');
    this.value = event.currentTarget?.value;
    this.checkNumbers();
  }
  @HostListener('blur', ['$event']) inputBlur(event: Event): void {
    // console.log('blur', (event.currentTarget as any)?.value);
    this.value = (event.currentTarget as any)?.value;
    this.checkNumbers();
    this.checkMinMax();
  }

  @HostListener('paste', ['$event']) blockPaste(event: KeyboardEvent): void {
    // console.log('paste', (event.currentTarget as any)?.value);
    this.checkNumbers();
    this.checkMinMax();
  }
  @HostListener('change', ['$event']) inputChange(event: KeyboardEvent): void {
    // console.log('inputChange', (event.currentTarget as any)?.value);
    this.checkNumbers();
  }
  @HostListener('keydown', ['$event']) public onKeyDown(event: KeyboardEvent): void {
    if (this.isDisabled) {
      return;
    }
    if ([DOWN_ARROW, UP_ARROW].includes(event.keyCode)) event.preventDefault();
    // console.log('keydown');

    // tslint:disable:deprecation
    switch (event.keyCode) {
      case DOWN_ARROW:
        this.addStep(-1 * this.step);
        break;
      case UP_ARROW:
        this.addStep(1 * this.step);
        break;
    }
  }

  @HostListener('keyup', ['$event']) public onKeyUp(event: KeyboardEvent): void {
    //  this.checkMinMax();
  }

  private changed() {
    // console.log('previewsValue=', this.previewsValue, 'value=', this.value);
    if (this.value !== this.previewsValue) {
      this._onChange(this.value);
    }
  }

  private checkNumbers(): void {
    if (this.value === undefined || this.value === null) {
      return;
    }
    this.value = ConvertNumbersToLatin.fixNumbers(this.value.toString());
    this.value = this.removeChar(this.value);
    this.value = this.checkMaxDigit(this.value);
    let previewValue: string = this.value.toString();
    if (this.thousandsSeparator) {
      previewValue = this.applyCamaFormat(this.value);
    }
    this.el.nativeElement.value = previewValue;
    this.changed();
  }

  private checkMinMax() {
    if (isNullOrEmpty(this.min) == false && isNullOrEmpty(this.value) == false && +this.value < +this.min!) {
      this.value = +this.min!;
    }
    if (isNullOrEmpty(this.max) == false && isNullOrEmpty(this.value) == false && +this.value > +this.max!) {
      this.value = +this.max!;
    }
    this.checkNumbers();
  }

  private removeChar(initialValue: string): string {
    this._regEx = this.isDecimal ? this._regExNumberAndDecimal : this._regExNumber;
    if (!this.isDecimal && this.allowNegative) this._regEx = this._regExNumberForNegative;
    else if (this.isDecimal && this.allowNegative) this._regEx = this._regExNumberAndDecimalForNegative;

    if (!this.isDecimal && /\./g.test(initialValue.toString())) {
      initialValue = initialValue.substring(0, initialValue.indexOf('.'));
    }
    let num = initialValue.toString().replace(this._regEx, '');

    const dotMatch = num.match(/\./g);
    if (dotMatch && dotMatch.length > 1) {
      num = parseFloat(num).toString();
    }
    const dashMatch = num.match(/\-/g);
    //TODO: get best practice
    if (dashMatch && (dashMatch.length > 0 || num.indexOf('-') > 0)) {
      const fc = num.substring(0, 1);
      const fw = num.substring(1, num.length).replace(this._regExNumberAndDecimal, '');
      num = fc + fw;
    }

    return num;
  }

  private applyCamaFormat(value: string | number): string {
    return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
  }
  private checkMaxDigit(initialValue: string | number): string {
    initialValue = initialValue.toString();
    const dotPosition = initialValue.indexOf('.');
    if (this.maxDecimalDigit != undefined && dotPosition > 0) {
      initialValue = initialValue.substring(0, dotPosition + +this.maxDecimalDigit + 1);
    }
    // console.log(this.maxDecimalDigit, '=>', dotPosition, 'result=>', initialValue);
    return initialValue;
  }

  private addStep(step: number) {
    this.value = +this.value + step;
    this.checkNumbers();
  }
}
