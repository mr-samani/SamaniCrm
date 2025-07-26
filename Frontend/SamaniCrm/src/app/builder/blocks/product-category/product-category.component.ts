import { CommonModule } from '@angular/common';
import { Component, ElementRef, Injector, OnInit } from '@angular/core';
import { BlockBase } from '../block-base';
import { BlockDefinition, BlockTypeEnum } from '../block-registry';
import { DynamicRendererComponent } from '../dynamic-renderer.component';
import { GetProductCategoriesQuery, ProductCategoryDto, ProductServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { DynamicDataCache, IDataStructure } from '@app/builder/dynamic-data.service';

@Component({
  selector: 'block-product-category',
  standalone: true,
  imports: [CommonModule, DynamicRendererComponent, MaterialCommonModule],
  templateUrl: './product-category.component.html',
  styleUrls: ['./product-category.component.scss'],
  host: {
    '[attr.style]': 'block.data.css',
  },
  providers: [ProductServiceProxy],
})
export class BlockProductCategoryComponent extends BlockBase implements OnInit {
  loading = false;
  list: ProductCategoryDto[] = [];

  // default value for repeatable item
  itemTemplate = new BlockDefinition({
    category: 'Container',
    type: 0,
    tagName: 'div',
    name: 'category card',
    data: {
      style: {
        width: '320px',
        minWidth: '320px',
        boxShadow: '0px 0px 10px 0px #00000033',
        borderRadius: '5px',
        backgroundColor: '#ffffff',
      },
    },
    rowNumber: 1,
    children: [
      {
        category: 'Container',
        type: BlockTypeEnum.GeneralHtmlTag,
        tagName: 'div',
        name: 'image-container',
        data: {
          style: {},
        },
        rowNumber: 2,
        children: [],
      },
      {
        category: 'General',
        type: 0,
        tagName: 'span',
        name: 'category name',
        data: {
          text: 'product description',
          style: {},
        },
        rowNumber: 3,
        children: [],
      },
    ],
  });

  cacheKey = '';

  constructor(
    injector: Injector,
    private productService: ProductServiceProxy,
  ) {
    super(injector);
    this.el.nativeElement.style;
  }

  ngOnInit(): void {
    this.itemTemplate = this.block && this.block.itemTemplate ? this.block.itemTemplate : this.itemTemplate;

    this.cacheKey = 'productCategory_' + this.block.id;
    this.getData();
  }

  getData() {
    if (this.loading) return;
    this.loading = true;
    const input = new GetProductCategoriesQuery();
    input.skip = 0;
    input.take = 4;
    this.productService
      .getCategories(input)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((result) => {
        this.list = result.data ?? [];
        this.itemTemplate.dynamicDataCacheKey = this.cacheKey;
        this.block.itemTemplate = this.itemTemplate;
        let s = new ProductCategoryDto();

        let dataStructure: IDataStructure[] = [
          {
            nameSpace: 'productCategory',
            type: 'object',
            children: Object.entries(s).map((m) => {
              return {
                nameSpace: m[0],
                type: 'string',
                children: [],
              };
            }),
          },
        ];

        this.b.ds.setCache<ProductCategoryDto[]>(this.cacheKey, dataStructure, this.list);
      });
  }
}
