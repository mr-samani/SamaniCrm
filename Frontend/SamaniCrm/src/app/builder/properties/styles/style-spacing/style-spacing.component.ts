import { Component, EventEmitter, forwardRef, Injector, Input, OnInit, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { parseSpacing, spacingRegex } from '../helper/parse-spacing';
import { SizeUnitList } from '../models/SizeUnit';
import { Spacing } from '../models/Spacing';

@Component({
  selector: 'style-spacing',
  templateUrl: './style-spacing.component.html',
  styleUrls: ['./style-spacing.component.scss'],
  standalone: false,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => StyleSpacingComponent),
      multi: true,
    },
  ],
})
export class StyleSpacingComponent extends AppComponentBase implements OnInit, ControlValueAccessor {
  sizeUnitList = SizeUnitList;
  spacingRegex = spacingRegex;

  value: Spacing = {};
  spacingString = '';

  onChangeFn = (val: string) => {};
  onTouchedFn = () => {};

  showPanel = false;

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}

  writeValue(val: string | undefined): void {
    this.spacingString = val ?? '';
    this.value = parseSpacing(this.spacingString);
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

  emitChange() {
    this.spacingString = this.getCssFromSpacingStyle(this.value);
    this.onChangeFn(this.spacingString);
  }

  /**
   * Optional helper to generate CSS string
   */
  private getCssFromSpacingStyle(p?: Spacing): string {
    if (!p) return '';
    const top = p.top ? `${p.top.size ?? 0}${p.top.unit}` : '0';
    const right = p.right ? `${p.right.size ?? 0}${p.right.unit}` : '0';
    const bottom = p.bottom ? `${p.bottom.size ?? 0}${p.bottom.unit}` : '0';
    const left = p.left ? `${p.left.size ?? 0}${p.left.unit}` : '0';
    return `${top} ${right} ${bottom} ${left}`;
  }

  stringChange(): void {
    this.value = parseSpacing(this.spacingString);
    this.emitChange();
  }
  onUiChange() {
    this.emitChange();
  }
  remove() {
    this.spacingString = '';
    this.stringChange();
  }

  togglePanel() {
    this.showPanel = !this.showPanel;
  }
}
