import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'block-product-category',
  standalone: true,
  imports:[CommonModule],
  template: `
    <section>
      <h3>{{ data.title }}</h3>
      <ul>
        <li *ngFor="let c of data.categories">{{ c }}</li>
      </ul>
    </section>
  `,
})
export class BlockProductCategoryComponent {
  @Input() data!: { title: string; categories: string[] };
}
 