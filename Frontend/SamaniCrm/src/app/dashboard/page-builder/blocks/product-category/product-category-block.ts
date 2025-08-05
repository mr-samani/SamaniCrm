// src/app/blocks/product-category/product-category.component.ts
import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  standalone: true,
  selector: 'product-category-block',
  template: `
    <div>
      <h3>{{ title }}</h3>
      <ul>
        <li *ngFor="let category of categories">{{ category }}</li>
      </ul>
    </div>
  `,
  imports: [CommonModule],
})
export class ProductCategoryBlockComponent {
  @Input() title: string = 'Product Categories';
  @Input() categories: string[] = [];
}
