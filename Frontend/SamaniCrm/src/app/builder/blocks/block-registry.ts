import { BlockHeroBannerComponent } from './banner/hero-banner.component';
import { BlockProductCategoryComponent } from './product-category/product-category.component';
import { Type } from '@angular/core';
import { BlockRowComponent } from './row/row.component';
import { generateUniqueId, guid } from '@shared/helper/guid';
import { BlockDivComponent } from './div/div.component';
import { BlockStyle } from '../properties/styles/models/_style';

export enum BlockTypeEnum {
  Div = 1,
  ProductCategory,
  HeroBanner,
  Row,
}

export declare type BlockCategory = 'Container' | 'Banner' | 'Product' | 'Other';

export class FormTools {
  category!: BlockCategory;
  items: IBlockDefinition[] = [];
}

export class IBlockDefinition {
  id?: string = guid();
  category?: BlockCategory;
  hidden?: boolean;
  rowNumber?: number;
  type: BlockTypeEnum = BlockTypeEnum.Row;
  component?: Type<any>;
  data?: any;
  children?: IBlockDefinition[] = [];
  /** المنت هایی که می توانند فرزند داشته باشند */
  canChild?: boolean;
}
export const BLOCK_REGISTRY: IBlockDefinition[] = [
  {
    category: 'Container',
    type: BlockTypeEnum.Row,
    component: BlockRowComponent,
    canChild: true,
  },
  {
    category: 'Container',
    type: BlockTypeEnum.Div,
    component: BlockDivComponent,
    canChild: true,
  },
  {
    category: 'Product',
    type: BlockTypeEnum.ProductCategory,
    component: BlockProductCategoryComponent,
    data: { title: 'Welcome to our Store!', categories: ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'] } as any,
  },
  {
    category: 'Banner',
    type: BlockTypeEnum.HeroBanner,
    component: BlockHeroBannerComponent,
  },
];

export class BlockDefinition {
  id: string = guid();
  category?: BlockCategory;
  hidden = false;
  type: BlockTypeEnum = BlockTypeEnum.Row;
  component?: Type<any>;
  data!: BlockData;
  children: BlockDefinition[] = [];
  canChild?: boolean;
  rowNumber!: number;

  constructor(data?: IBlockDefinition) {
    if (data) {
      for (let property in data) {
        if (data.hasOwnProperty(property)) (this as any)[property] = (data as any)[property];
      }

      this.id = data.id ?? generateUniqueId();
      this.data = new BlockData(data.data);

      this.children = (data.children ?? []).map((child) => new BlockDefinition(child));
    }
  }
}

export class BlockData {
  style: BlockStyle = {};
  css: string = '';
  constructor(data?: BlockData) {
    if (data) {
      for (let property in data) {
        if (data.hasOwnProperty(property)) (this as any)[property] = (data as any)[property];
      }
    }
  }
}
