import { Component, Input } from '@angular/core';
import { BlockDefinition } from '../block-registry';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'block-hero-banner',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="hero" *ngIf="block">
      <h1>{{ block.data?.title }}</h1>
      <p>{{ block.data?.subtitle }}</p>
    </div>
  `,
})
export class BlockHeroBannerComponent {
  @Input() block?: BlockDefinition;
}
