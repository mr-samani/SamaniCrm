// src/app/blocks/product-category/product-category.component.ts

import { Component, Input } from '@angular/core';

@Component({
  standalone: true,
  selector: 'product-category-block',
  template: `
    <div>
      <h3>{{ title }}</h3>
      <ul>
        @for (category of categories; track category) {
          <li>{{ category }}</li>
        }
      </ul>
    </div>
    `,
  imports: [],
})
export class ProductCategoryBlockComponent {
  @Input() title: string = 'Product Categories';
  @Input() categories: string[] = [];
}
