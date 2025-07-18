import { Component, forwardRef, Injector, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { getCssFromSpacingStyle, parseSpacing, spacingRegex } from '../helper/parse-spacing';
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

  value: Spacing | null = null;
  spacingString = '';

  onChangeFn = (val: string) => {};
  onTouchedFn = () => {};

  showPanel = false;

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}

  writeValue(val: string | undefined): void {
    this.spacingString = val ?? '0px';
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
    this.spacingString = getCssFromSpacingStyle(this.value);
    this.onChangeFn(this.spacingString);
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
    if (!this.value) {
      this.value = {};
    }
    this.value.top ??= {};
    this.value.bottom ??= {};
    this.value.left ??= {};
    this.value.right ??= {};
    this.showPanel = !this.showPanel;
  }
}
