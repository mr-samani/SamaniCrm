import { Injectable } from '@angular/core';
import { BlockDefinition } from './blocks/block-registry';
export interface IDataStructure {
  key: string;
  type: 'string' | 'number' | 'bigint' | 'boolean' | 'symbol' | 'undefined' | 'object' | 'function';
  children: IDataStructure[];
}
export interface IDynamicDataCache {
  baseName: string;
  structure: IDataStructure[];
  // object or array
  data: any;
}
@Injectable()
export class DynamicDataService {
  private cashedDynamicData: {
    [key: string]: IDynamicDataCache;
  } = {};

  reset() {
    this.cashedDynamicData = {};
  }

  setCache<T>(key: string, baseName: string, data: T) {
    let structure: IDataStructure[] = [];
    let s = data || {};
    if (Array.isArray(data)) {
      s = data[0] || {};
    }
    structure = this.convertToTree(s);

    this.cashedDynamicData[key] = {
      baseName: baseName,
      data: data,
      structure: structure,
    };

    console.log(this.cashedDynamicData);
  }
  getCache(key?: string) {
    return key ? this.cashedDynamicData[key] : undefined;
  }

  resolveValue(block: BlockDefinition, value?: string, index?: number) {
    value = typeof value == 'string' ? value.trim() : '';
    if (value.startsWith('{{') && value.endsWith('}}')) {
      const key = value.slice(2, -2);
      const dynamicDataCacheKey = this.findNearestDynamicDataCacheKey(block);
      const dynamicData = this.getCache(dynamicDataCacheKey);
      if (!dynamicData) {
        return value;
      }
      return this.resolveDynamicBinding(key, dynamicData, index);
    }
    return value;
  }

  private resolveDynamicBinding(key: string, dynamicData?: any, index?: number): any {
    if (!dynamicData || !key) return null;

    const parts = key.split('.');

    let current: any = dynamicData;
    if (Array.isArray(current) && index !== undefined) {
      current = current[index];
    }
    for (const part of parts) {
      if (Array.isArray(current) && index !== undefined) {
        current = current[index];
      }
      if (current && typeof current === 'object' && part in current) {
        current = current[part];
      } else {
        return null; // کلید پیدا نشد
      }
    }

    return current;
  }

  /*------------------------------*/
  getTreeDynamicDataList(block: BlockDefinition): IDataStructure[] {
    const cacheKey = this.findNearestDynamicDataCacheKey(block);
    if (!cacheKey) return [];
    const dynamicData = this.getCache(cacheKey);
    if (!dynamicData) return [];
    return [
      {
        key: dynamicData.baseName,
        children: dynamicData.structure,
        type: 'object',
      },
    ];
  }
  private convertToTree(obj: any): IDataStructure[] {
    if (!obj || typeof obj !== 'object') return [];
    if (Array.isArray(obj)) {
      obj = obj[0] || {};
    }
    return Object.entries(obj).map(([key, value]) => {
      if (value !== null && typeof value === 'object' && !Array.isArray(value)) {
        return {
          key,
          type: typeof value,
          children: this.convertToTree(value),
        };
      } else {
        return { key, value, type: typeof value, children: [] };
      }
    });
  }

  private findNearestDynamicDataCacheKey(block: BlockDefinition): string {
    let current = block;
    while (current) {
      if (current.dynamicDataCacheKey) return current.dynamicDataCacheKey;
      current = current.parent!;
    }
    return '';
  }
}
