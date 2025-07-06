import { BlockHeroBannerComponent } from './banner/hero-banner.component';
import { BlockProductCategoryComponent } from './product-category/product-category.component';
import { Type } from '@angular/core';
import { BlockRowComponent } from './row/row.component';
import { guid } from '@shared/helper/guid';
import { BlockDivComponent } from './div/div.component';

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

export const BLOCK_REGISTRY = new Map<BlockTypeEnum, Type<any>>([
  [BlockTypeEnum.Row, BlockRowComponent],
  [BlockTypeEnum.Div, BlockDivComponent],
  [BlockTypeEnum.ProductCategory, BlockProductCategoryComponent],
  [BlockTypeEnum.HeroBanner, BlockHeroBannerComponent],
]);

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
export class BlockDefinition {
  id: string = guid();
  category?: BlockCategory;
  hidden = false;
  type: BlockTypeEnum = BlockTypeEnum.Row;
  component?: Type<any>;
  data?: any;
  children: BlockDefinition[] = [];
  canChild?: boolean;
  rowNumber!: number;

  constructor(data?: IBlockDefinition) {
    if (data) {
      for (let property in data) {
        if (data.hasOwnProperty(property)) (this as any)[property] = (data as any)[property];
      }
      if (!data.id) {
        data.id = guid();
      }
      if (!data.children) {
        data.children = [];
      } else {
        for (let child of data.children) {
          child = new BlockDefinition(child);
        }
      }
    }
  }
}
