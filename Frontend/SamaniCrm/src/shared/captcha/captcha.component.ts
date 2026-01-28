import { Component, forwardRef,  Input, OnInit } from '@angular/core';
import {
  NG_VALUE_ACCESSOR,
  NG_VALIDATORS,
  ControlValueAccessor,
  Validator,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@app/app-component-base';
import { CaptchaServiceProxy } from '@shared/service-proxies';

@Component({
  selector: 'captcha',
  templateUrl: './captcha.component.html',
  styleUrls: ['./captcha.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CaptchaComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: CaptchaComponent,
    },
    CaptchaServiceProxy,
  ],
  standalone: false,
})
export class CaptchaComponent extends AppComponentBase implements OnInit, ControlValueAccessor, Validator {
  @Input() isMaterial = true;
  captchaCode = '';
  key = '';
  image = '';
  loading = true;
  constructor(
    private captchaService: CaptchaServiceProxy,
  ) {
    super();
  }
  private _onChange = (t: { captchaKey: string; captchaText: string }) => {};
  private _onTouched = () => {};

  ngOnInit(): void {
    this.reloadCaptcha();
  }
  writeValue(value: any): void {}
  registerOnChange(fn: any): void {
    this._onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this._onTouched = fn;
  }

  validate(control: AbstractControl): ValidationErrors | null {
    const content = control.value;
    return control.errors;
    // if (!content) {
    //   return { required: true };
    // } else {
    //   return null;
    // }
  }

  change(event: any) {
    this._onChange(event);
  }

  public reloadCaptcha() {
    this.loading = true;
    this.captchaService
      .reload()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((result) => {
        this.image = result.img;
        this.key = result.key;
        this.onChange();
      });
  }

  onChange() {
    this._onChange({
      captchaKey: this.key,
      captchaText: this.captchaCode,
    });
  }
}
