import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  forwardRef,
  HostListener,
  Injector,
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
  /**
   * The maximum range of the box shadow.
   * @default 25
   */
  @Input() maxRange = 25;

  padRect?: DOMRect;
  flashlightRect?: DOMRect;
  x = 0;
  y = 0;

  isDragging = false;
  selectedValue = '';
  list: string[] = [];

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

    const minX = flashlightRec.width / 2;
    const maxX = padRec.width - flashlightRec.width / 2;
    const minY = flashlightRec.height / 2;
    const maxY = padRec.height - flashlightRec.height / 2;

    const clampedX = Math.max(minX, Math.min(position.x, maxX));
    const clampedY = Math.max(minY, Math.min(position.y, maxY));

    // فقط یکبار از وسط اصلاح کن
    this.x = clampedX - flashlightRec.width / 2;
    this.y = clampedY - flashlightRec.height / 2;

    // فاصله flashlight از مرکز
    const dx = this.x - this.center.x;
    const dy = this.y - this.center.y;

    // مقیاس تبدیل به -50 تا +50 (یا هرچقدر بخوای)
    const halfRangeX = (padRec.width - flashlightRec.width) / 2;
    const halfRangeY = (padRec.height - flashlightRec.height) / 2;

    let valueX = (dx / halfRangeX) * this.maxRange; // -50 تا +50
    let valueY = (dy / halfRangeY) * this.maxRange;

    // رُند کردن
    valueX = Math.round(valueX);
    valueY = Math.round(valueY);

    // محدود کردن به -50 تا +50 یا هر مقدار دلخواه
    valueX = Math.min(Math.max(valueX, -this.maxRange), this.maxRange);
    valueY = Math.min(Math.max(valueY, -this.maxRange), this.maxRange);

    // Calculate angle from center to mouse
    //const angle = (Math.atan2(position.y - this.center.y, position.x - this.center.x) * 180) / Math.PI + 90;
    const angle = (Math.atan2(this.y - this.center.y, this.x - this.center.x) * 180) / Math.PI + 90;
    console.log(this.y - this.center.y, this.x - this.center.x, angle);
    // Position flashlight on the edge of the container
    const flashlightX = this.center.x + this.radius * Math.cos(angle);
    const flashlightY = this.center.y + this.radius * Math.sin(angle);

    // Update flashlight position and rotation
    if (this.flashlight.nativeElement) {
      this.flashlight.nativeElement.style.transform = ` rotate(${angle * -1}deg)`;
    }

    // Update box-shadow to simulate light
    const shadowX = -Math.cos(angle) * 20; // Offset shadow in opposite direction
    const shadowY = -Math.sin(angle) * 20;

    // if (!this.value || this.value.x !== valueX || this.value.y !== valueY) {
    //   this.value = newValue;
    //   this.onChangeData();
    // }
  }

  private updateContainerDimensions() {
    this.padRect = this.pad.nativeElement.getBoundingClientRect();
    this.flashlightRect = this.flashlight.nativeElement.getBoundingClientRect();
    this.center = { x: this.padRect.width / 2, y: this.padRect.height / 2 };
    this.x = this.center.x - this.flashlightRect!.width / 2;
    this.y = this.center.y - this.flashlightRect!.height / 2;
    this.radius = Math.min(this.padRect.width, this.padRect.height) / 2 - 20; // Keep flashlight inside bounds

    this.cd.detectChanges();
  }
  writeValue(val: string): void {
    if (!val) {
      this.shadows = [];
      this.onChange('');
      this.change.emit('');
      return;
    }

    // Parse box-shadow string
    this.shadows = parseBoxShadow(val);
    this.result = formatBoxShadowToCSS(this.shadows);
    if (val != this.result) {
      this.update();
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  // Update shadow values (e.g., from UI inputs)
  updateShadow(index: number, updatedShadow: Partial<BoxShadow>) {
    if (index >= this.shadows.length) return;

    this.shadows[index] = {
      ...this.shadows[index],
      ...updatedShadow,
      color: validateColor(updatedShadow.color || this.shadows[index].color),
    };

    this.update();
  }

  // Add a new shadow
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

  // Remove a shadow
  removeShadow(index: number) {
    this.shadows.splice(index, 1);
    this.update();
  }

  private update() {
    this.result = formatBoxShadowToCSS(this.shadows);
    this.onChange(this.result);
    this.change.emit(this.result);
  }
}
