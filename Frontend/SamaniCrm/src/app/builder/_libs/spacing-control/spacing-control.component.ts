import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  forwardRef,
  HostListener,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { getPointerPosition, IPosition } from './getPointerPosition';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

export interface IPosValue {
  top?: number;
  right?: number;
  bottom?: number;
  left?: number;
}
export interface ISpacingModel {
  padding: IPosValue;
  margin: IPosValue;
}

@Component({
  selector: 'spacing-control',
  templateUrl: './spacing-control.component.html',
  styleUrls: ['./spacing-control.component.scss'],
  standalone: true,
  imports: [CommonModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SpacingControlComponent),
      multi: true,
    },
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SpacingControlComponent implements OnInit, ControlValueAccessor {
  @Output() change = new EventEmitter<Partial<CSSStyleDeclaration> | undefined>();
  spacing: ISpacingModel = {
    margin: { top: 0, right: 0, bottom: 0, left: 0 },
    padding: { top: 0, right: 0, bottom: 0, left: 0 },
  };
  unit: string = 'px'; // Default unit
  startDragging = false;
  previousXY: IPosition = { x: 0, y: 0 };
  dragItem: keyof ISpacingModel = 'margin';
  pos: keyof IPosValue = 'top';
  previousValue = 0;
  style?: Partial<CSSStyleDeclaration>;
  onChange = (_: Partial<CSSStyleDeclaration> | undefined) => {};
  onTouched = () => {};

  constructor() {}

  ngOnInit() {}

  writeValue(val: Partial<CSSStyleDeclaration> | undefined): void {
    this.style = val ?? {};
    const defaultSpacing: ISpacingModel = {
      margin: { top: 0, right: 0, bottom: 0, left: 0 },
      padding: { top: 0, right: 0, bottom: 0, left: 0 },
    };

    // Parse margin and padding from input
    this.spacing = {
      margin: this.parseSpacingValues(val?.margin, defaultSpacing.margin),
      padding: this.parseSpacingValues(val?.padding, defaultSpacing.padding),
    };

    this.onChange(this.style);
    this.change.emit(this.style);
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  update() {
    if (!this.spacing && !this.style) return;

    // Validate and format spacing values
    const validatedMargin = this.validateSpacing(this.spacing.margin, true);
    const validatedPadding = this.validateSpacing(this.spacing.padding, false);

    // Update style object with formatted CSS values
    this.style!.padding = this.formatSpacingToCSS(validatedPadding);
    this.style!.margin = this.formatSpacingToCSS(validatedMargin);

    this.onChange(this.style);
    this.change.emit(this.style);
  }

  // Parse spacing values from CSS string (e.g., "10px 20px 30px 40px")
  private parseSpacingValues(cssValue: string | undefined, defaultValue: IPosValue): IPosValue {
    if (!cssValue) return { ...defaultValue };

    const values = cssValue.trim().split(/\s+/);
    const unitRegex = /(px|rem|em|%)$/;
    let unit = this.unit;

    // Extract numeric values and unit
    const parsed: IPosValue = { ...defaultValue };
    if (values.length === 1) {
      // Single value (e.g., "10px")
      const val = this.parseSingleValue(values[0], unitRegex);
      parsed.top = parsed.right = parsed.bottom = parsed.left = val;
      unit = values[0].match(unitRegex)?.[0] || this.unit;
    } else if (values.length === 2) {
      // Two values (e.g., "10px 20px")
      parsed.top = parsed.bottom = this.parseSingleValue(values[0], unitRegex);
      parsed.right = parsed.left = this.parseSingleValue(values[1], unitRegex);
      unit = values[0].match(unitRegex)?.[0] || this.unit;
    } else if (values.length === 3) {
      // Three values (e.g., "10px 20px 30px")
      parsed.top = this.parseSingleValue(values[0], unitRegex);
      parsed.right = parsed.left = this.parseSingleValue(values[1], unitRegex);
      parsed.bottom = this.parseSingleValue(values[2], unitRegex);
      unit = values[0].match(unitRegex)?.[0] || this.unit;
    } else if (values.length === 4) {
      // Four values (e.g., "10px 20px 30px 40px")
      parsed.top = this.parseSingleValue(values[0], unitRegex);
      parsed.right = this.parseSingleValue(values[1], unitRegex);
      parsed.bottom = this.parseSingleValue(values[2], unitRegex);
      parsed.left = this.parseSingleValue(values[3], unitRegex);
      unit = values[0].match(unitRegex)?.[0] || this.unit;
    }

    this.unit = unit; // Update unit if detected
    return parsed;
  }

  // Parse a single CSS value (e.g., "10px" -> 10)
  private parseSingleValue(value: string, unitRegex: RegExp): number {
    const num = parseFloat(value.replace(unitRegex, ''));
    return isNaN(num) ? 0 : num;
  }

  // Validate spacing values
  private validateSpacing(spacing: IPosValue, allowNegative: boolean): IPosValue {
    const validated: IPosValue = {};
    const keys: (keyof IPosValue)[] = ['top', 'right', 'bottom', 'left'];

    for (const key of keys) {
      const value = spacing[key];
      if (value === undefined || isNaN(value)) {
        validated[key] = 0;
      } else if (!allowNegative && value < 0) {
        validated[key] = 0; // No negative values for padding
      } else {
        validated[key] = Math.round(value * 100) / 100; // Round to 2 decimal places
      }
    }
    return validated;
  }

  // Format spacing to CSS string (e.g., { top: 10, right: 20, bottom: 30, left: 40 } -> "10px 20px 30px 40px")
  private formatSpacingToCSS(spacing: IPosValue): string {
    const { top, right, bottom, left } = spacing;
    if (top === right && right === bottom && bottom === left) {
      return `${top}${this.unit}`; // Single value
    } else if (top === bottom && right === left) {
      return `${top}${this.unit} ${right}${this.unit}`; // Two values
    } else if (right === left) {
      return `${top}${this.unit} ${right}${this.unit} ${bottom}${this.unit}`; // Three values
    }
    return `${top}${this.unit} ${right}${this.unit} ${bottom}${this.unit} ${left}${this.unit}`; // Four values
  }

  onMouseDown(ev: MouseEvent | TouchEvent, mOp: keyof ISpacingModel, p: keyof IPosValue) {
    this.startDragging = true;
    this.dragItem = mOp;
    this.pos = p;
    this.previousValue = this.spacing[this.dragItem][this.pos] ?? 0;
    this.previousXY = getPointerPosition(ev);
    this.update(); // Ensure initial update
  }

  @HostListener('document:mouseup', ['$event'])
  @HostListener('document:touchend', ['$event'])
  onMouseUp(ev: MouseEvent | TouchEvent) {
    this.startDragging = false;
    this.update(); // Ensure final update after drag
  }

  @HostListener('document:mousemove', ['$event'])
  @HostListener('document:touchmove', ['$event'])
  onMouseMove(ev: MouseEvent | TouchEvent) {
    if (!this.startDragging) return;
    let position = getPointerPosition(ev);
    const offsetX = position.x - this.previousXY.x;
    const offsetY = position.y - this.previousXY.y;
    if (this.pos === 'top' || this.pos === 'bottom') {
      this.spacing[this.dragItem][this.pos] = this.previousValue + offsetY;
    } else {
      this.spacing[this.dragItem][this.pos] = this.previousValue + offsetX;
    }
    if (this.dragItem === 'padding' && (this.spacing[this.dragItem][this.pos] ?? 0) < 0) {
      this.spacing[this.dragItem][this.pos] = 0;
    }
    this.update(); // Update on drag
  }
}
