import { Component, forwardRef, Input, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'switch',
  templateUrl: './switch.component.html',
  styleUrls: ['./switch.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SwitchComponent),
      multi: true,
    },
  ],
  standalone: false,
})
export class SwitchComponent implements OnInit, ControlValueAccessor {
  @Input() mini = false;
  toggle = false;

  disabled = false;
  private _onChange = (val: any) => {};
  private _onTouched = () => {};
  constructor() {}
  writeValue(obj: any): void {
    this.toggle = obj;
  }
  registerOnChange(fn: any): void {
    this._onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this._onTouched = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  ngOnInit(): void {}

  change() {
    this._onChange(this.toggle);
  }
}
