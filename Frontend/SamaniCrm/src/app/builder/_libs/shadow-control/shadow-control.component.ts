import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  forwardRef,
  HostListener,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { BoxShadow, formatBoxShadowToCSS, parseBoxShadow, validateColor } from './box-shadow-parser';
import { getOffsetPosition } from './get-offset-position';

@Component({
  selector: 'shadow-control',
  templateUrl: './shadow-control.component.html',
  styleUrls: ['./shadow-control.component.scss'],
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ShadowControlComponent),
      multi: true,
    },
  ],
})
export class ShadowControlComponent implements OnInit, AfterViewInit {
  @Input() maxRange = 25;

  padRect?: DOMRect;
  flashlightRect?: DOMRect;
  x = 0;
  y = 0;

  isDragging = false;
  selectedShadow?: BoxShadow;
  selectedIndex = 0;

  @Output() change = new EventEmitter<string>();
  shadows: BoxShadow[] = [];
  onChange = (_: string) => {};
  onTouched = () => {};

  @ViewChild('pad', { static: true }) pad!: ElementRef<HTMLDivElement>;
  @ViewChild('flashlight', { static: true }) flashlight!: ElementRef<SVGSVGElement>;

  private radius: number = 0;
  center: { x: number; y: number } = { x: 0, y: 0 };
  result = '';

  constructor(private cd: ChangeDetectorRef) {}

  ngOnInit() {}
  ngAfterViewInit() {
    this.updateContainerDimensions();
  }

  @HostListener('window:resize')
  onResize() {
    this.updateContainerDimensions();
  }

  onPadClick(ev: MouseEvent | TouchEvent) {
    const position = getOffsetPosition(ev, this.pad.nativeElement);
    this.isDragging = true;
    this.updatePosition(position);
  }

  dragStart(ev: MouseEvent | TouchEvent) {
    ev.stopPropagation();
    ev.preventDefault();
    this.updateContainerDimensions();
    this.isDragging = true;
    const position = getOffsetPosition(ev, this.pad.nativeElement);
    this.updatePosition(position);
  }

  @HostListener('document:mouseup', ['$event'])
  @HostListener('document:touchend', ['$event'])
  onDragEnd(ev: MouseEvent | TouchEvent) {
    this.isDragging = false;
  }

  @HostListener('mousemove', ['$event'])
  @HostListener('touchmove', ['$event'])
  onMouseMove(ev: MouseEvent | TouchEvent) {
    if (!this.isDragging) return;
    const position = getOffsetPosition(ev, this.pad.nativeElement);
    this.updatePosition(position);
  }

  private updatePosition(position: { x: number; y: number }) {
    if (!this.isDragging) return;

    const padRec = this.padRect!;
    const flashlightRec = this.flashlightRect!;

    const centerX = this.center.x;
    const centerY = this.center.y;

    const dx = position.x - centerX;
    const dy = position.y - centerY;

    // زاویه
    const angleRad = Math.atan2(dy, dx);
    const minX = flashlightRec.width / 2;
    const maxX = padRec.width - flashlightRec.width / 2;
    const minY = flashlightRec.height / 2;
    const maxY = padRec.height - flashlightRec.height / 2;
    const clampedX = Math.max(minX, Math.min(position.x, maxX));
    const clampedY = Math.max(minY, Math.min(position.y, maxY));

    // فقط یکبار از وسط اصلاح کن
    this.x = clampedX - flashlightRec.width / 2;
    this.y = clampedY - flashlightRec.height / 2;

    // ✅ ولی مقدار واقعی shadow رو از موقعیت موس (dx, dy) بگیر
    const halfRangeX = (padRec.width - flashlightRec.width) / 2;
    const halfRangeY = (padRec.height - flashlightRec.height) / 2;

    let valueX = (dx / halfRangeX) * this.maxRange;
    let valueY = (dy / halfRangeY) * this.maxRange;

    valueX = Math.round(Math.min(Math.max(valueX, -this.maxRange), this.maxRange));
    valueY = Math.round(Math.min(Math.max(valueY, -this.maxRange), this.maxRange));

    // چرخش چراغ قوه
    const angleDeg = angleRad * (180 / Math.PI);
    if (this.flashlight.nativeElement) {
      this.flashlight.nativeElement.style.transform = `rotate(${angleDeg + 180}deg)`;
    }

    // آپدیت shadow
    if (this.selectedShadow) {
      this.selectedShadow = {
        ...this.selectedShadow,
        xOffset: valueX,
        yOffset: valueY,
      };
      this.updateShadow(this.selectedShadow);
    }

    this.cd.detectChanges();
  }

  private updateContainerDimensions() {
    this.padRect = this.pad.nativeElement.getBoundingClientRect();
    this.flashlightRect = this.flashlight.nativeElement.getBoundingClientRect();
    this.center = { x: this.padRect.width / 2, y: this.padRect.height / 2 };
    this.radius = Math.min(this.padRect.width, this.padRect.height) / 2 - this.flashlightRect.width / 2;
    this.x = 0; // Reset to center
    this.y = 0;
    this.cd.detectChanges();
  }

  writeValue(val: string): void {
    if (!val) {
      this.shadows = [];
      this.result = '';
      this.onChange('');
      this.change.emit('');
      return;
    }

    this.shadows = parseBoxShadow(val);
    this.result = formatBoxShadowToCSS(this.shadows);
    if (val !== this.result) {
      this.update();
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  updateShadow(updatedShadow: Partial<BoxShadow>) {
    if (this.selectedIndex >= this.shadows.length) return;

    this.shadows[this.selectedIndex] = {
      ...this.shadows[this.selectedIndex],
      ...updatedShadow,
      color: validateColor(updatedShadow.color || this.shadows[this.selectedIndex].color),
    };
    this.selectedShadow!.cssValue = formatBoxShadowToCSS([this.selectedShadow!]);
    this.update();
  }

  addShadow() {
    this.shadows.push({
      xOffset: 0,
      yOffset: 0,
      blurRadius: 0,
      spreadRadius: 0,
      color: 'rgba(0, 0, 0, 0.5)',
      unit: 'px',
    });
    this.update();
  }

  removeShadow(index: number) {
    this.shadows.splice(index, 1);
    this.update();
  }

  private update() {
    this.result = formatBoxShadowToCSS(this.shadows);
    this.shadows.forEach((m) => (m.cssValue = formatBoxShadowToCSS([m])));
    this.onChange(this.result);
    this.change.emit(this.result);
    this.cd.detectChanges();
  }
}
