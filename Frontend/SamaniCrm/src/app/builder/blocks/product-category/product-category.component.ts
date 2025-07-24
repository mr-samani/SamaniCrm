import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { BlockBase } from '../block-base';
import { BlockDefinition } from '../block-registry';
import { DynamicRendererComponent } from '../dynamic-renderer.component';
import { GetProductCategoriesQuery, ProductCategoryDto, ProductServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs';
import { MaterialCommonModule } from '@shared/material/material.common.module';

@Component({
  selector: 'block-product-category',
  standalone: true,
  imports: [CommonModule, DynamicRendererComponent, MaterialCommonModule],
  templateUrl: './product-category.component.html',
  styleUrls: ['./product-category.component.scss'],
  providers: [ProductServiceProxy],
})
export class BlockProductCategoryComponent extends BlockBase implements OnInit {
  loading = true;
  list: ProductCategoryDto[] = [];

  itemTemplate: BlockDefinition = {
    category: 'Container',
    type: 0,
    canChild: true,
    tagName: 'div',
    name: 'div',
    icon: 'fa block-div',
    data: {
      style: {
        width: 320,
        minWidth: 320,
        boxShadow: '0px 0px 10px 0px #00000033',
        borderRadius: '5px',
        backgroundColor: '#ffffff',
      },
      css: '',
    },
    rowNumber: 1,
    children: [
      {
        category: 'Container',
        type: 0,
        canChild: true,
        tagName: 'div',
        name: 'div',
        icon: 'fa block-div',
        data: {
          style: {},
          css: '',
        },
        rowNumber: 2,
        children: [],
      },
      {
        category: 'General',
        type: 0,
        canChild: false,
        tagName: 'span',
        name: 'span',
        icon: 'fa block-span',
        data: {
          text: 'product description',
          style: {},
          css: '',
        },
        rowNumber: 3,
        children: [],
      },
    ],
  };

  constructor(
    injector: Injector,
    private productService: ProductServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    const input = new GetProductCategoriesQuery();
    input.skip = 0;
    input.take = 10;
    this.productService
      .getCategories(input)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((result) => {
        this.list = result.data ?? [];
      });
  }
}
