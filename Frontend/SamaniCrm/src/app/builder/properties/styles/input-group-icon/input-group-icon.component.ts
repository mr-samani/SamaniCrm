import { ChangeDetectionStrategy, Component, EventEmitter, forwardRef, Injector, Input, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { InputGroupItem } from './InputGroupItem';
@Component({
  selector: 'input-group-icon',
  templateUrl: './input-group-icon.component.html',
  standalone: false,
  styles: ``,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputGroupIconComponent),
      multi: true,
    },
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InputGroupIconComponent extends AppComponentBase implements ControlValueAccessor {
  @Output() change = new EventEmitter<string | undefined>();
  @Input() items: InputGroupItem[] = [];
  value?: string;

  onChange = (_: string | undefined) => {};
  onTouched = () => {};

  constructor(injector: Injector) {
    super(injector);
  }

  writeValue(val: string | undefined): void {
    this.value = val;
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  update() {
    this.onChange(this.value);
    this.change.emit(this.value);
  }
}
