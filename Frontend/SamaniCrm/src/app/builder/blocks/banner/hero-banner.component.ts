import { Component, Input } from '@angular/core';

@Component({
  selector: 'block-hero-banner',
  standalone: true,
  template: `
    <div class="hero">
      <h1>{{ data.title }}</h1>
      <p>{{ data.subtitle }}</p>
    </div>
  `,
})
export class BlockHeroBannerComponent {
  @Input() data!: { title: string; subtitle: string };
}
 
 