import { CommonModule } from '@angular/common';
import { Component, forwardRef, Injector, Input, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  NG_VALUE_ACCESSOR,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { TranslateModule } from '@ngx-translate/core';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { PasswordComplexityDto } from '@shared/service-proxies';

@Component({
  selector: 'app-password-input',
  templateUrl: './password-input.component.html',
  styleUrls: ['./password-input.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialCommonModule, TranslateModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => PasswordInputComponent),
      multi: true,
    },
  ],
})
export class PasswordInputComponent extends AppComponentBase implements OnInit {
  @Input() policy: PasswordComplexityDto = new PasswordComplexityDto({
    requiredLength: 6,
    requireDigit: true,
    requireLowercase: false,
    requireUppercase: false,
    requireNonAlphanumeric: false,
  });
  @Input() className = 'col-md-6';
  @Input() parentClass = 'row';

  form: FormGroup;
  hidePassword = true;
  hideConfirm = true;

  private onChange = (_: any) => {};
  private onTouched = () => {};

  constructor(injector: Injector) {
    super(injector);
    this.form = this.fb.group(
      {
        password: [''],
        confirmPassword: [''],
      },
      { validators: this.matchPasswords() },
    );

    this.form.valueChanges.subscribe((val) => {
      const match = val.password === val.confirmPassword;
      if (this.form.valid && match) {
        this.onChange(val.password);
      } else {
        this.onChange(null);
      }
    });
  }

  matchPasswords(): ValidatorFn {
    return (group: AbstractControl): ValidationErrors | null => {
      const password = group.get('password')?.value;
      const confirm = group.get('confirmPassword')?.value;
      if (password !== confirm) {
        group.get('confirmPassword')?.setErrors({ confirmError: true });
        return { passwordsMismatch: true };
      } else {
        group.get('confirmPassword')?.setErrors(null);
        return null;
      }
    };
  }

  ngOnInit() {
    const validators: ValidatorFn[] = [Validators.required, Validators.minLength(this.policy.requiredLength ?? 6)];

    if (this.policy.requireDigit) {
      validators.push(Validators.pattern(/.*\d.*/));
    }
    if (this.policy.requireLowercase) {
      validators.push(Validators.pattern(/.*[a-z].*/));
    }
    if (this.policy.requireUppercase) {
      validators.push(Validators.pattern(/.*[A-Z].*/));
    }
    if (this.policy.requireNonAlphanumeric) {
      validators.push(Validators.pattern(/.*[^a-zA-Z0-9].*/));
    }

    this.form.get('password')?.setValidators(validators);
    this.form.get('password')?.updateValueAndValidity();
  }

  writeValue(value: any): void {
    this.form.patchValue({ password: value, confirmPassword: '' }, { emitEvent: false });
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  get passwordErrors() {
    const control = this.form.get('password');
    if (!control || !control.errors) return [];

    const errors: string[] = [];
    if (control.hasError('required')) errors.push('رمز عبور الزامی است.');
    if (control.hasError('minlength')) errors.push(`حداقل طول باید ${this.policy.requiredLength} کاراکتر باشد.`);
    if (control.hasError('pattern')) {
      if (this.policy.requireDigit && !/\d/.test(control.value)) errors.push('باید شامل عدد باشد.');
      if (this.policy.requireLowercase && !/[a-z]/.test(control.value)) errors.push('باید شامل حروف کوچک باشد.');
      if (this.policy.requireUppercase && !/[A-Z]/.test(control.value)) errors.push('باید شامل حروف بزرگ باشد.');
      if (this.policy.requireNonAlphanumeric && /^[a-zA-Z0-9]*$/.test(control.value))
        errors.push('باید شامل کاراکتر خاص باشد.');
    }

    return errors;
  }
}
