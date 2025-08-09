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
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { SpacingFormatter } from './SpacingFormatter';
import { IPosValue } from './IPosValue';
import { ISpacingModel } from './ISpacingModel';
import { parseSpacingValues, validateSpacing } from './validateSpacing';

@Component({
  selector: 'spacing-control',
  templateUrl: './spacing-control.component.html',
  styleUrls: ['./spacing-control.component.scss'],
  standalone: true,
  imports: [CommonModule, FormsModule],
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
  previousValue: number = 0;
  customValueMOP?: keyof ISpacingModel;
  customValueP?: keyof IPosValue;
  customValueName?: string;
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
      margin: parseSpacingValues(val?.margin, defaultSpacing.margin, this.unit),
      padding: parseSpacingValues(val?.padding, defaultSpacing.padding, this.unit),
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
    if (!this.spacing || !this.style) return;

    // Validate and format spacing values
    const validatedMargin = validateSpacing(this.spacing.margin, true);
    const validatedPadding = validateSpacing(this.spacing.padding, false);

    // Update style object with formatted CSS values
    this.style.padding = SpacingFormatter.formatSpacingToCSS(validatedPadding);
    this.style.margin = SpacingFormatter.formatSpacingToCSS(validatedMargin);

    this.onChange(this.style);
    this.change.emit(this.style);
  }

  onMouseDown(ev: MouseEvent | TouchEvent, mOp: keyof ISpacingModel, p: keyof IPosValue) {
    this.startDragging = true;
    this.dragItem = mOp;
    this.pos = p;

    this.previousValue =
      this.spacing[this.dragItem][this.pos] != 'auto' ? parseInt(this.spacing[this.dragItem][this.pos] as any) : 0;
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
    if (
      this.dragItem === 'padding' &&
      typeof this.spacing[this.dragItem][this.pos] == 'number' &&
      +this.spacing[this.dragItem][this.pos]! < 0
    ) {
      this.spacing[this.dragItem][this.pos] = 0;
    }
    this.update(); // Update on drag
  }

  open(mOp: keyof ISpacingModel, p: keyof IPosValue, label: string) {
    this.customValueMOP = mOp;
    this.customValueP = p;
    this.customValueName = label;
  }

  updateCustomValue(val?: number | 'auto') {
    if (!this.customValueMOP || !this.customValueP) return;
    this.spacing[this.customValueMOP][this.customValueP] = val;
  }
}
