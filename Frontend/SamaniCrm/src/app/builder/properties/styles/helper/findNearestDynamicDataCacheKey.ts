import { BlockDefinition } from '../../../blocks/block-registry';

export function findNearestDynamicDataCacheKey(block: BlockDefinition): string {
  let current = block;
  while (current) {
    if (current.dynamicDataCacheKey) return current.dynamicDataCacheKey;
    current = current.parent!;
  }
  return '';
}
