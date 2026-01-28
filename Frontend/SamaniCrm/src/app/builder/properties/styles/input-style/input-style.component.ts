import { ChangeDetectionStrategy, Component, EventEmitter, forwardRef,  Input, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { SizeUnit, SizeUnitList } from '../models/SizeUnit';

@Component({
  selector: 'input-style',
  templateUrl: './input-style.component.html',
  standalone: false,
  styles: `
    select {
      padding: 0 !important;
      min-width: 40px !important;
      border-radius: 0 4px 4px 0 !important;
    }
    input {
      border-radius: 4px 0 0 4px !important;
      text-align: center;
    }
  `,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputStyleComponent),
      multi: true,
    },
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InputStyleComponent extends AppComponentBase implements ControlValueAccessor {
  @Input() label? = '';
  @Input() tooltip? = '';
  @Output() change = new EventEmitter<string>();
  sizeUnitList = SizeUnitList.filter((x) => x != 'auto');

  value?: number;
  suffix: SizeUnit = 'px';

  onChange = (_: string) => {};
  onTouched = () => {};

  constructor() {
    super();
  }

  writeValue(val: string | undefined): void {
    if (val) {
      let p = val
        .toString()
        .trim()
        .match(/^(\d+(\.\d+)?)(px|em|rem|vw|vh|%)?$/);

      this.value = p && p[1] ? +p[1] : undefined;
      this.suffix = p && p[3] ? (p[3] as any) : 'px';
    } else {
      this.value = undefined;
      this.suffix = 'px';
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  update() {
    this.onChange(this.value + this.suffix);
    this.change.emit(this.value + this.suffix);
  }
}
