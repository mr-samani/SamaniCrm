import { AfterViewInit, Component, ElementRef, forwardRef, Injector, Input, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { getCssFromSpacingStyle, parseSpacing, spacingRegex } from '../helper/parse-spacing';
import { SizeUnit, SizeUnitList } from '../models/SizeUnit';
import { Spacing, SpacingValue } from '../models/Spacing';

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
export class StyleSpacingComponent extends AppComponentBase implements OnInit, ControlValueAccessor, AfterViewInit {
  sizeUnitList = SizeUnitList;
  spacingRegex = spacingRegex;

  value: Spacing | null = null;
  spacingString = '';
  @Input() canUseAuto = false;
  @Input() label = '';

  top = 0;
  left = 0;
  allBorder = true;
  allSpaceing: SpacingValue = {
    size: 0,
    unit: 'px',
  };
  onChangeFn = (val: string) => {};
  onTouchedFn = () => {};

  showPopup = false;

  constructor(
    injector: Injector,
    private el: ElementRef<HTMLElement>,
  ) {
    super(injector);
  }

  ngOnInit() {}

  ngAfterViewInit(): void {
    const rect = this.el.nativeElement.getBoundingClientRect();
    this.top = rect.bottom;
    this.left = rect.left;
  }

  writeValue(val: string | undefined): void {
    this.spacingString = val ?? '0px';
    if (!this.canUseAuto) {
      this.spacingString = this.spacingString.replace(/auto/g, '');
    }
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
    this.showPopup = !this.showPopup;
  }

  onChangeAllBorder() {
    this.value = {};
  }
  changeAllSpacing() {
    this.spacingString = (this.allSpaceing.size ?? 0) + (this.allSpaceing.unit ?? 'px');
    this.stringChange();
  }
}
