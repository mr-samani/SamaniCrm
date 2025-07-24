import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { BlockBase } from '../block-base';
import { BlockDefinition, BlockTypeEnum } from '../block-registry';
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
  loading = false;
  list: ProductCategoryDto[] = [];

  // default value for container
  container = new BlockDefinition({
    type: BlockTypeEnum.GeneralHtmlTag,
    tagName: 'section',
    name: 'Product Category Container',
    canChild: true,
    data: {
      style: {
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        gap: '10px',
        padding: '15px',
        flexWrap: 'wrap',
      },
    },
  });

  // default value for repeatable item
  private itemTemplate = new BlockDefinition({
    category: 'Container',
    type: 0,
    canChild: true,
    tagName: 'article',
    name: 'category card',
    data: {
      style: {
        width: 320,
        minWidth: 320,
        boxShadow: '0px 0px 10px 0px #00000033',
        borderRadius: '5px',
        backgroundColor: '#ffffff',
      },
    },
    rowNumber: 1,
    children: [
      {
        category: 'Container',
        type: 0,
        canChild: true,
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
        canChild: false,
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

  constructor(
    injector: Injector,
    private productService: ProductServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (!this.block.children || !this.block.children.length) {
      this.block.children = [this.container];
    }
    if (!this.block.itemTemplate) {
      this.block.itemTemplate = this.itemTemplate;
    }
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
        this.container.children = [];
        if (this.list.length == 0) {
          this.container.children = [this.block.itemTemplate!];
        } else {
          for (let item of this.list) {
            this.container.children.push(this.block.itemTemplate!);
          }
        }
      });
  }
}
