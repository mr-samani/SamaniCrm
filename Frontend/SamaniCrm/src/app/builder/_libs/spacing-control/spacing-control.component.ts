import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  forwardRef,
  HostListener,
  Injector,
  OnInit,
  Output,
} from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { getPointerPosition, IPosition } from './getPointerPosition';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
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
export class SpacingControlComponent extends AppComponentBase implements OnInit {
  @Output() change = new EventEmitter<Partial<CSSStyleDeclaration> | undefined>();
  spacing: ISpacingModel = {
    margin: {
      top: 0,
      right: 0,
      bottom: 0,
      left: 0,
    },
    padding: {
      top: 0,
      right: 0,
      bottom: 0,
      left: 0,
    },
  };

  startDragging = false;

  previousXY: IPosition = { x: 0, y: 0 };
  dragItem: keyof ISpacingModel = 'margin';
  pos: keyof IPosValue = 'top';
  previousValue = 0;

  style?: Partial<CSSStyleDeclaration>;
  onChange = (_: Partial<CSSStyleDeclaration> | undefined) => {};
  onTouched = () => {};
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}
  writeValue(val: Partial<CSSStyleDeclaration> | undefined): void {
    this.style = val;
    let padding = val?.padding ?? {};
    let margin = val?.margin ?? {};
    this.spacing = {
      padding,
      margin,
    };
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  update() {
    if (!this.style) return;
    this.style.padding = this.spacing.padding + '';
    this.style.margin = this.spacing.margin + '';
    this.onChange(this.style);
    this.change.emit(this.style);
  }

  onMouseDown(ev: MouseEvent | TouchEvent, mOp: keyof ISpacingModel, p: keyof IPosValue) {
    // console.log(ev, mOp, p);
    this.startDragging = true;
    this.dragItem = mOp;
    this.pos = p;
    this.previousValue = this.spacing[this.dragItem][this.pos] ?? 0;
    this.previousXY = getPointerPosition(ev);
  }

  @HostListener('document:mouseup', ['$event'])
  @HostListener('document:touchend', ['$event'])
  onMouseUp(ev: MouseEvent | TouchEvent) {
    this.startDragging = false;
  }

  @HostListener('document:mousemove', ['$event'])
  @HostListener('document:touchmove', ['$event'])
  onMouseMove(ev: MouseEvent | TouchEvent) {
    if (!this.startDragging) return;
    let position = getPointerPosition(ev);
    const offsetX = position.x - this.previousXY.x;
    const offsetY = position.y - this.previousXY.y;
    if (this.pos == 'top' || this.pos == 'bottom') {
      this.spacing[this.dragItem][this.pos] = this.previousValue + offsetY;
    } else {
      this.spacing[this.dragItem][this.pos] = this.previousValue + offsetX;
    }
    if (this.dragItem == 'padding' && (this.spacing[this.dragItem][this.pos] ?? 0) < 0) {
      this.spacing[this.dragItem][this.pos] = 0;
    }
  }
}
