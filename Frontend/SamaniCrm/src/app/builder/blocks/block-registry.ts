import { BlockHeroBannerComponent } from './banner/hero-banner.component';
import { BlockProductCategoryComponent } from './product-category/product-category.component';
import { Type } from '@angular/core';
import { BlockRowComponent } from './row/row.component';

export enum BlockTypeEnum {
  ProductCategory = 1,
  HeroBanner,
  Row,
}

export const BLOCK_REGISTRY: BlockDefinition[] = [
  {
    type: BlockTypeEnum.Row,
    component: BlockRowComponent,
    defaultData: {
      cols: 2,
    },
  },
  {
    type: BlockTypeEnum.ProductCategory,
    component: BlockProductCategoryComponent,
    defaultData: {
      title: 'Welcome to our Store!',
      subtitle: 'Find the best deals here.',
    },
  },
  {
    type: BlockTypeEnum.HeroBanner,
    component: BlockHeroBannerComponent,
    defaultData: {
      title: 'Welcome to our Store!',
      subtitle: 'Find the best deals here.',
    },
  },
];

export interface BlockDefinition {
  type: BlockTypeEnum;
  component?: Type<any>;
  defaultData?: any;
  data?: any;
}
