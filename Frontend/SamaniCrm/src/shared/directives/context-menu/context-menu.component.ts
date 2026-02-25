import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  OnInit,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContextMenuItem } from './context-menu.model';
import { animate } from '@angular/animations';

@Component({
  selector: 'app-context-menu',
  standalone: true,
  imports: [CommonModule],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './context-menu.component.html',
  styleUrls: ['./context-menu.component.scss'],
})
export class ContextMenuComponent implements OnInit {
  @Input('contextMenuTitle') contextMenuTitle?: string;
  items: ContextMenuItem[] = [];
  @Input('setItems') set setItems(val: ContextMenuItem[]) {
    this.items = val ?? [];
    this.chdr.detectChanges();
  }
  @Input() x = 0;
  @Input() y = 0;
  @Output() closed = new EventEmitter<void>();

  private el: HTMLElement;

  constructor(
    private elementRef: ElementRef,
    private chdr: ChangeDetectorRef,
  ) {
    this.el = elementRef.nativeElement;
  }

  ngOnInit(): void {
    this.adjustPosition();
  }

  /** Keep the menu inside the viewport */
  private adjustPosition(): void {
    requestAnimationFrame(() => {
      const rect = this.el.getBoundingClientRect();
      const vw = window.innerWidth;
      const vh = window.innerHeight;

      if (this.x + rect.width > vw) {
        this.x = vw - rect.width - 8;
      }
      if (this.y + rect.height > vh) {
        this.y = vh - rect.height - 8;
      }
    });
  }

  select(item: ContextMenuItem): void {
    if (item.disabled) return;
    item.callback();
    this.closed.emit();
  }

  @HostListener('document:keydown.escape')
  onEscape(): void {
    this.closed.emit();
  }
}
