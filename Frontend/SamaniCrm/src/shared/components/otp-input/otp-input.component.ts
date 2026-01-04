
import { Component, ElementRef, forwardRef, Injector, Input, OnInit, QueryList, ViewChildren } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'otp-input',
  templateUrl: './otp-input.component.html',
  styleUrls: ['./otp-input.component.scss'],
  standalone: true,
  imports: [],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OtpInputComponent),
      multi: true,
    },
  ],
})
export class OtpInputComponent extends AppComponentBase implements OnInit {
  @Input() length = 6; // تعداد رقم
  @ViewChildren('otpInput') inputs!: QueryList<ElementRef<HTMLInputElement>>;

  boxes: number[] = [];
  private value: string = '';

  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {
    this.boxes = Array(this.length).fill(0);
  }

  // تبدیل اعداد فارسی/عربی به انگلیسی
  private normalizeNumber(char: string): string {
    const persianDigits = '۰۱۲۳۴۵۶۷۸۹';
    const arabicDigits = '٠١٢٣٤٥٦٧٨٩';

    if (persianDigits.includes(char)) {
      return persianDigits.indexOf(char).toString();
    }
    if (arabicDigits.includes(char)) {
      return arabicDigits.indexOf(char).toString();
    }
    return char;
  }

  onInput(event: Event, index: number) {
    const input = event.target as HTMLInputElement;
    let val = this.normalizeNumber(input.value);

    if (!/^\d$/.test(val)) {
      input.value = '';
      return;
    }

    input.value = val;
    this.updateValue();

    // فقط اگر چیزی وارد شد برو بعدی
    if (val && index < this.length - 1) {
      this.focusInput(index + 1);
    }
  }

  onKeyDown(event: KeyboardEvent, index: number) {
    const input = this.inputs.toArray()[index].nativeElement;

    if (event.key === 'Backspace') {
      if (input.value) {
        // پاک کردن مقدار فعلی
        input.value = '';
        this.updateValue();
        event.preventDefault();
      } else if (index > 0) {
        // اگه خالی بود برو قبلی
        this.focusInput(index - 1);
        const prevInput = this.inputs.toArray()[index - 1].nativeElement;
        prevInput.value = '';
        this.updateValue();
        event.preventDefault();
      }
    } else if (event.key === 'ArrowLeft' && index > 0) {
      this.focusInput(index - 1);
      event.preventDefault();
    } else if (event.key === 'ArrowRight' && index < this.length - 1) {
      this.focusInput(index + 1);
      event.preventDefault();
    }
  }

  onPaste(event: ClipboardEvent, index: number) {
    event.preventDefault();
    const pasted = event.clipboardData?.getData('text') ?? '';
    const digits = pasted
      .split('')
      .map((ch) => this.normalizeNumber(ch))
      .filter((ch) => /^\d$/.test(ch));

    if (digits.length === 0) return;

    const inputsArr = this.inputs.toArray();
    for (let i = 0; i < digits.length && index + i < this.length; i++) {
      inputsArr[index + i].nativeElement.value = digits[i];
    }

    this.updateValue();
    const nextIndex = Math.min(index + digits.length, this.length - 1);
    this.focusInput(nextIndex);
  }

  private focusInput(index: number) {
    const input = this.inputs.toArray()[index];
    input.nativeElement.focus();
    input.nativeElement.select();
  }

  private updateValue() {
    const inputsArr = this.inputs.toArray();
    this.value = inputsArr.map((i) => i.nativeElement.value).join('');
    this.onChange(this.value);
  }

  // ControlValueAccessor
  writeValue(value: string): void {
    this.value = value || '';
    const inputsArr = this.inputs?.toArray();
    if (inputsArr) {
      inputsArr.forEach((input, i) => {
        input.nativeElement.value = this.value[i] || '';
      });
    }
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.inputs?.forEach((inp) => (inp.nativeElement.disabled = isDisabled));
  }
}
