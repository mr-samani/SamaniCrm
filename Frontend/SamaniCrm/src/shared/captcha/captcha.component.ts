import { Component, forwardRef, Injector, OnInit } from '@angular/core';
import {
  NG_VALUE_ACCESSOR,
  NG_VALIDATORS,
  ControlValueAccessor,
  Validator,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms'; 
import { finalize } from 'rxjs/operators';
import { CaptchaDto } from '@app/account/models/captcha-dto';
import { Apis } from '@shared/apis';
import { AppComponentBase } from '@app/app-component-base';

export interface getCaptcharesponse {
  sensitive: boolean;
  key: string;
  img: string;
}

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
  ],
  standalone: false,
})
export class CaptchaComponent extends AppComponentBase implements OnInit, ControlValueAccessor, Validator {
  captchaCode = '';
  key = '';
  image = '';
  loading = true;
  constructor(injector: Injector) {
    super(injector);
  }
  private _onChange = (t: { key: string; captcha: string }) => {};
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
    this.dataService
      .get<any, CaptchaDto>(Apis.reloadCaptcha, {})
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((result: any) => {
        let data: getCaptcharesponse = result;
        this.image = data.img;
        this.key = data.key;
      });
  }

  onChange() {
    this._onChange({
      key: this.key,
      captcha: this.captchaCode,
    });
  }
}
