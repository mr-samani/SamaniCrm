import { Component, Injector, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BlockBase } from '../block-base';

@Component({
  selector: 'block-hero-banner',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="hero-banner">Banner</div>
  `,
  styles: `
  :host{
    display:block;
  }
  `,
})
export class BlockHeroBannerComponent extends BlockBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
