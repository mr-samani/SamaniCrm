
import { CommonModule } from '@angular/common'
import { Component, Injector, Input, OnInit } from '@angular/core';
import { BaseComponent } from '@app/base-components';

@Component({
  selector: 'block-product-category',
  templateUrl: './product-category.component.html',
  styleUrls: ['./product-category.component.scss'],
  standalone: true,
  imports: [CommonModule],
}) 
export class ProductCategoryComponent extends BaseComponent implements OnInit {
  @Input() data!: { title: string; categories: string[] };

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}
}
