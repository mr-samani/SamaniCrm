import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { BlockDefinition } from '../block-registry';

@Component({
  selector: 'block-product-category',
  standalone: true,
  imports: [CommonModule],
  template: `
    <section *ngIf="block">
      <h3>{{ block.data.title }}</h3>
      <ul>
        <li *ngFor="let c of block.data.categories">{{ c }}</li>
      </ul>
    </section>
  `,
})
export class BlockProductCategoryComponent {
  @Input() block?: BlockDefinition;
}
