import { Component,  Input } from '@angular/core';

import { BlockBase } from '../block-base';

@Component({
  selector: 'block-hero-banner',
  standalone: true,
  imports: [],
  template: `
    <div class="hero-banner">Banner</div>
  `,
})
export class BlockHeroBannerComponent extends BlockBase {
  constructor() {
    super();
  }
}
