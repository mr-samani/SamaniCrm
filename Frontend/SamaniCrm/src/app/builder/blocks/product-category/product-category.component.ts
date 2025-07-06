import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { BlockBase } from '../block-base';

@Component({
  selector: 'block-product-category',
  standalone: true,
  imports: [CommonModule],
  template: `
    Category
    <section [style]="block.data.css">
      <h3>{{ data.title }}</h3>
      <ul>
        <li *ngFor="let c of data.categories">{{ c }}</li>
      </ul>
    </section>
  `,
})
export class BlockProductCategoryComponent extends BlockBase implements OnInit {
  data: any;
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.data = this.block.data;
  }
}
