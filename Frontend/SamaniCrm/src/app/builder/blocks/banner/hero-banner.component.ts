import { Component, Injector, Input } from '@angular/core';
import { BlockDefinition } from '../block-registry';
import { CommonModule } from '@angular/common';
import { BlockBase } from '../block-base';

@Component({
  selector: 'block-hero-banner',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      class="hero-banner"
      *ngIf="block"
      (click)="b.onSelect(block, $event)"
      [class.fb-selected]="b.selectedBlock == block">
      Banner
    </div>
  `,
})
export class BlockHeroBannerComponent extends BlockBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
