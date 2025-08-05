import { Component, EventEmitter, forwardRef, Injector, Input, OnInit, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { Border, BorderStyle, BorderTypeList } from '../models/BorderStyle';
import { cloneDeep } from 'lodash-es';
import { SizeUnitList } from '../models/SizeUnit';

@Component({
  selector: 'style-border',
  templateUrl: './style-border.component.html',
  styleUrls: ['./style-border.component.scss'],
  standalone: false,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => StyleBorderComponent),
      multi: true,
    },
  ],
})
export class StyleBorderComponent extends AppComponentBase implements OnInit, ControlValueAccessor {
  borderTypeList = BorderTypeList;
  sizeUnitList = SizeUnitList;

  value?: BorderStyle;

  defaultBorder: Border = {
    color: '#ececec',
    inset: false,
    unit: 'px',
    type: 'solid',
  };

  onChangeFn: any = () => {};
  onTouchedFn: any = () => {};

  @Output() css = new EventEmitter<string>();

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {
    this.onChangeAll();
  }

  writeValue(val: BorderStyle): void {
    this.value = val;
    this.onChangeAll();
  }

  registerOnChange(fn: any): void {
    this.onChangeFn = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouchedFn = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    // you can implement this if needed
  }

  onSelectForAll(forAll: boolean) {
    this.value = !this.value ? { forAll } : this.value;
    this.value.forAll = forAll;
    this.onChangeAll();
  }
  onChangeAll() {
    if (!this.value) return;
    if (this.value.forAll && !this.value.border) {
      this.value.border = cloneDeep(this.defaultBorder);
    } else {
      this.value.top ??= cloneDeep(this.value.border);
      this.value.right ??= cloneDeep(this.value.border);
      this.value.bottom ??= cloneDeep(this.value.border);
      this.value.left ??= cloneDeep(this.value.border);
    }
    this.emitChange();
  }

  emitChange() {
    this.onChangeFn(this.value);

    this.css.emit(this.getCssFromBorderStyle(this.value));
  }

  /**
   * Optional helper to generate CSS string
   */
  public getCssFromBorderStyle(b?: BorderStyle): string {
    if (!b) return '';
    if (b.forAll && b.border?.size) {
      return `border: ${b.border.size}${b.border.unit} ${b.border.type} ${b.border.color};`;
    } else {
      const top = b.top?.size ? `${b.top.size}${b.top.unit} ${b.top.type} ${b.top.color}` : 'none';
      const right = b.right?.size ? `${b.right.size}${b.right.unit} ${b.right.type} ${b.right.color}` : 'none';
      const bottom = b.bottom?.size ? `${b.bottom.size}${b.bottom.unit} ${b.bottom.type} ${b.bottom.color}` : 'none';
      const left = b.left?.size ? `${b.left.size}${b.left.unit} ${b.left.type} ${b.left.color}` : 'none';
      return `border-top: ${top}; border-right: ${right}; border-bottom: ${bottom}; border-left: ${left};`;
    }
  }

  // Call this method on UI input changes
  public onUiChange(): void {
    this.emitChange();
  }

  remove(name: string) {
    if (this.value) (this.value as any)[name] = {};
    this.emitChange();
  }
}
