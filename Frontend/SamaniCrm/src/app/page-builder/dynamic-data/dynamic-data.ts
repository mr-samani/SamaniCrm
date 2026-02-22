import { DynamicDataStructure, DynamicValueType } from 'ngx-page-builder/core';

export const DYNAMIC_DATA: DynamicDataStructure[] = [
  {
    id:'1',
    name: 'ProductCategory',
    displayName: 'Product category',
    type: DynamicValueType.Array,
    list: [
      [
        {
          name: 'Id',
          displayName: 'Id',
          type: DynamicValueType.String,
          value: 'SampleId',
        },
        {
          name: 'Title',
          displayName: 'Title',
          type: DynamicValueType.String,
          value: 'Sample Title',
        },
        {
          name: 'Description',
          displayName: 'Description',
          type: DynamicValueType.String,
          value: 'Sample Description',
        },
        {
          name: 'Image',
          displayName: 'Image',
          type: DynamicValueType.String,
          value: 'sampleimg',
        },
      ],
    ],
  },
];
