import { Component, ElementRef, Injector, OnInit } from '@angular/core';
import { BlockBase } from '../block-base';
import { BlockDefinition, BlockTypeEnum } from '../block-registry';
import { DynamicRendererComponent } from '../dynamic-renderer.component';
import { GetProductCategoriesQuery, ProductCategoryDto, ProductServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { DynamicDataCache, IDataStructure } from '@app/builder/services/dynamic-data.service';

@Component({
  selector: 'block-product-category',
  standalone: true,
  imports: [DynamicRendererComponent, MaterialCommonModule],
  templateUrl: './product-category.component.html',
  styleUrls: ['./product-category.component.scss'],
  host: {
    '[attr.style]': 'block.data?.css',
  },
  providers: [ProductServiceProxy],
})
export class BlockProductCategoryComponent extends BlockBase implements OnInit {
  loading = false;
  list: ProductCategoryDto[] = [];

  // default value for repeatable item
  itemTemplate = new BlockDefinition({
    id: '01987aba-8c89-f239-9962-36c6f91e477b',
    tagName: 'div',
    name: 'category card',
    category: 'Container',
    type: 0,
    data: {
      style: {
        width: '320px',
        minWidth: '320px',
        boxShadow: '0px 0px 10px 0px #00000080',
        borderRadius: '5px',
        backgroundColor: '#ffffff',
        minHeight: '280px',
      },
      css: 'width:320px;min-width:320px;box-shadow:0px 0px 10px 0px #00000080;border-radius:5px;background-color:#ffffff;min-height:280px;',
    },
    children: [
      {
        id: '01987aba-8c89-ec30-6c6c-e7b7aac3e1ed',
        tagName: 'div',
        name: 'image-container',
        category: 'Container',
        type: 0,
        data: {
          style: {
            height: '200px',
            backgroundColor: '#f5f5ff',
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            gap: 'nullpx',
          },
          css: 'height:200px;background-color:#f5f5ff;display:flex;justify-content:center;align-items:center;gap:nullpx;',
        },
        children: [
          {
            id: '01987aba-cacf-1a02-0532-4d57c315b06c',
            icon: 'fa fa-image',
            category: 'General',
            type: 4,
            data: {
              text: '{{productCategory.image}}',
              style: {
                maxWidth: '100%',
                maxHeight: '100%',
              },
              css: 'max-width:100%;max-height:100%;',
            },
            attributes: {
              url: {
                type: 'image',
                value: 'images/default-image.png',
              },
            },
            children: [],
            canChild: true,
            dynamicDataCacheKey: 'productCategory_01987aba-8c83-7f4d-514d-ce7827617a65',
          },
        ],
        rowNumber: 2,
      },
      {
        id: '01987aba-e9ae-7899-0996-beb0d4e16b5f',
        tagName: 'div',
        name: 'div',
        icon: 'fa block-div',
        category: 'Container',
        type: 0,
        data: {
          text: '{{productCategory.title}}',
          style: {
            padding: '4px',
          },
          css: 'padding:4px;',
        },
        children: [],
        canChild: true,
      },
      {
        id: '01987aba-8c89-b581-fd5f-a1956a125094',
        tagName: 'span',
        name: 'category name',
        category: 'General',
        type: 0,
        data: {
          text: '{{productCategory.description}}',
          style: {
            padding: '4px',
          },
          css: 'padding:4px;',
        },
        children: [],
        rowNumber: 3,
      },
    ],
    rowNumber: 1,
    dynamicDataCacheKey: 'productCategory_01987aba-8c83-7f4d-514d-ce7827617a65',
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
