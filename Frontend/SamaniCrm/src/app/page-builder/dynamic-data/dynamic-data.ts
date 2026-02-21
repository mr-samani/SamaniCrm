import { DynamicDataStructure, DynamicValueType } from 'ngx-page-builder/core';

export const DYNAMIC_DATA: DynamicDataStructure[] = [
  {
    name: 'ProductCategory',
    displayName: 'Product category',
    type: DynamicValueType.Array,
    values: [
      {
        name: 'Id',
        displayName: 'Id',
        type: DynamicValueType.String,
        value: '',
      },
      {
        name: 'Title',
        displayName: 'Title',
        type: DynamicValueType.String,
        value: '',
      },
      {
        name: 'Description',
        displayName: 'Description',
        type: DynamicValueType.String,
        value: '',
      },
      {
        name: 'Image',
        displayName: 'Image',
        type: DynamicValueType.String,
        value: '',
      },
    ],
  },
];
