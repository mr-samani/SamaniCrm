import { Injectable } from '@angular/core';
import { BlockDefinition } from './blocks/block-registry';
import { findNearestDynamicDataCacheKey } from './properties/styles/helper/findNearestDynamicDataCacheKey';
export interface IDataStructure {
  nameSpace: string;
  type: 'string' | 'number' | 'bigint' | 'boolean' | 'symbol' | 'undefined' | 'object' | 'function';
  children: IDataStructure[];
}
export class DynamicDataCache<T = any> {
  structure: IDataStructure[] = [];
  // object or array
  data?: T;
}
@Injectable()
export class DynamicDataService {
  private cashedDynamicData: { [key: string]: DynamicDataCache } = {};

  reset() {
    this.cashedDynamicData = {};
  }

  setCache<T>(key: string, structure: IDataStructure[], data: T) {
    this.cashedDynamicData[key] = {
      structure,
      data,
    };
    console.log('set cache', this.cashedDynamicData);
  }
  getCache(key?: string) {
    return key ? this.cashedDynamicData[key] : undefined;
  }

  resolveValue(block: BlockDefinition, value?: string, index?: number) {
    value = typeof value == 'string' ? value.trim() : '';
    if (value.startsWith('{{') && value.endsWith('}}')) {
      const key = value.slice(2, -2);
      const dynamicDataCacheKey = findNearestDynamicDataCacheKey(block);
      const dynamicData = this.getCache(dynamicDataCacheKey);
      if (!dynamicData) {
        return value;
      }
      return this.resolveDynamicBinding(key, dynamicData, index);
    }
    return value;
  }

  private resolveDynamicBinding(key: string, dynamicData?: DynamicDataCache, index?: number): string {
    if (!dynamicData || !key || !dynamicData.data) return '';
    const lastPart: string = key.split('.').pop() ?? '';
    let data = dynamicData.data;
    if (Array.isArray(data) && index !== undefined) {
      data = data[index];
    }
    return data[lastPart] ?? '';
  }

  /*------------------------------*/
  getTreeDynamicDataList(block: BlockDefinition): IDataStructure[] {
    const cacheKey = findNearestDynamicDataCacheKey(block);
    if (!cacheKey) return [];
    const dynamicData = this.getCache(cacheKey);
    if (!dynamicData) return [];
    return dynamicData.structure ?? [];
  }
  private convertToTree(obj: any): IDataStructure[] {
    if (!obj || typeof obj !== 'object') return [];
    if (Array.isArray(obj)) {
      obj = obj[0] || {};
    }
    return Object.entries(obj).map(([key, value]) => {
      if (value !== null && typeof value === 'object' && !Array.isArray(value)) {
        return {
          nameSpace: key,
          type: typeof value,
          children: this.convertToTree(value),
        };
      } else {
        return { nameSpace: key, value, type: typeof value, children: [] };
      }
    });
  }
}
