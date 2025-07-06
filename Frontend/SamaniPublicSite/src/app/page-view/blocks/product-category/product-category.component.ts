import { CommonModule } from '@angular/common';
import { Component, Injector } from '@angular/core';
import { BlockBase } from '../block-base';

@Component({
  selector: 'block-product-category',
  standalone: true,
  imports: [CommonModule],
  template: `
    Category
    <section>
      <h3>{{ block.data.title }}</h3>
      <ul>
        <li *ngFor="let c of block.data.categories">{{ c }}</li>
      </ul>
    </section>
  `,
})
export class BlockProductCategoryComponent extends BlockBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
