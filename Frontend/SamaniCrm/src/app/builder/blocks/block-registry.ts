import { BlockHeroBannerComponent } from './banner/hero-banner.component';
import { BlockProductCategoryComponent } from './product-category/product-category.component';
import { Type } from '@angular/core';
import { BlockRowComponent } from './row/row.component';
import { BlockGeneralHtmlTagsComponent } from './general-html-tags/general-html-tags.component';
import { GeneralTagNames } from './general-html-tags/GeneralTagNames';
import { BlockImgComponent } from './img/img.component';
import { generateSequentialGuid } from '@shared/helper/guid';

export enum BlockTypeEnum {
  GeneralHtmlTag = 0,
  ProductCategory,
  HeroBanner,
  Row,
  Img,
}

export declare type BlockCategory = 'General' | 'Container' | 'Banner' | 'Product' | 'Other';

export class FormTools {
  category!: BlockCategory;
  items: BlockDefinition[] = [];
}

export const BLOCK_REGISTRY: BlockDefinition[] = [
  {
    category: 'Container',
    type: BlockTypeEnum.Row,
    component: BlockRowComponent,
    canChild: true,
    icon: 'fa fa-grate',
  },
  {
    category: 'General',
    type: BlockTypeEnum.GeneralHtmlTag,
    component: BlockGeneralHtmlTagsComponent,
    canChild: true,
  },
  {
    category: 'General',
    type: BlockTypeEnum.Img,
    component: BlockImgComponent,
    canChild: true,
    icon: 'fa fa-image',
  },
  {
    category: 'Product',
    type: BlockTypeEnum.ProductCategory,
    component: BlockProductCategoryComponent,
    icon: 'fa fa-boxes-packing',
    canChild: false,
    tagName: 'section',
    name: 'Product Category',
    data: {
      style: {
        display: 'flex',
        justifyContent: 'center',
        gap: '10px',
        padding: '15px',
        flexWrap: 'wrap',
      },
    },
  },
  {
    category: 'Banner',
    type: BlockTypeEnum.HeroBanner,
    component: BlockHeroBannerComponent,
    icon: 'fa fa-image-landscape',
  },
];

export class BlockDefinition {
  id?: string;
  tagName?: GeneralTagNames;
  name?: string;
  description?: string;
  /** forn icon (fmont awsome) */
  icon?: string;
  /** svg image for toolbox icon */
  image?: string;
  canDelete?: boolean;
  isCustomBlock?: boolean;
  category?: BlockCategory;
  hidden?: boolean;
  type: BlockTypeEnum = BlockTypeEnum.Row;
  component?: Type<any>;
  data?: BlockData;
  attributes?: BlockAttribute;
  children?: BlockDefinition[] = [];
  /** المنت هایی که می توانند فرزند داشته باشند */
  canChild?: boolean;
  rowNumber?: number;
  itemTemplate?: BlockDefinition;
  dynamicDataCacheKey?: string;
  parent?: BlockDefinition;
  constructor(data?: BlockDefinition, parent?: BlockDefinition) {
    if (data) {
      for (let property in data) {
        if (data.hasOwnProperty(property)) (this as any)[property] = (data as any)[property];
      }
      this.data = new BlockData(data.data);
      if (!data.id) {
        this.id = generateSequentialGuid();
      }
      this.parent = parent;

      if (data.itemTemplate) {
        this.itemTemplate = new BlockDefinition(data.itemTemplate, this);
      }
      this.children = (data.children ?? []).map((child) => (child = new BlockDefinition(child, this)));
    }
  }
}
export class BlockData {
  text?: string;
  style?: Partial<CSSStyleDeclaration>;
  css?: string = '';

  constructor(data?: BlockData | any) {
    if (data) {
      for (let property in data) {
        if (data.hasOwnProperty(property)) (this as any)[property] = (data as any)[property];
      }
    }
    this.css ??= '';
    this.style ??= {};
  }
}

export class BlockAttribute {
  url?: BlockAttributeDetails = {
    type: 'image',
    value: '',
  };
}
export class BlockAttributeDetails {
  type?: 'number' | 'string' | 'image' = 'string';
  value?: any;
}
