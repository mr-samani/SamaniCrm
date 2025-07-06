import { Component, EventEmitter, forwardRef, Injector, Input, OnInit, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { Border, BorderStyle, BorderTypeList, SizePrefixList } from '../models/BorderStyle';
import { cloneDeep } from 'lodash-es';

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
  sizePrefixList = SizePrefixList;

  value?: BorderStyle;

  defaultBorder: Border = {
    color: '#ececec',
    inset: false,
    prefix: 'px',
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
      this.value.borderTop ??= cloneDeep(this.value.border);
      this.value.borderRight ??= cloneDeep(this.value.border);
      this.value.borderBottom ??= cloneDeep(this.value.border);
      this.value.borderLeft ??= cloneDeep(this.value.border);
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
    if (b.forAll && b.border?.width) {
      return `border: ${b.border.width}${b.border.prefix} ${b.border.type} ${b.border.color};`;
    } else {
      const top = b.borderTop?.width
        ? `${b.borderTop.width}${b.borderTop.prefix} ${b.borderTop.type} ${b.borderTop.color}`
        : 'none';
      const right = b.borderRight?.width
        ? `${b.borderRight.width}${b.borderRight.prefix} ${b.borderRight.type} ${b.borderRight.color}`
        : 'none';
      const bottom = b.borderBottom?.width
        ? `${b.borderBottom.width}${b.borderBottom.prefix} ${b.borderBottom.type} ${b.borderBottom.color}`
        : 'none';
      const left = b.borderLeft?.width
        ? `${b.borderLeft.width}${b.borderLeft.prefix} ${b.borderLeft.type} ${b.borderLeft.color}`
        : 'none';
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
