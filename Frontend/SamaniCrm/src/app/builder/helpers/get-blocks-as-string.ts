/**
 * Convert block or blocks to a JSON string representation.
 * به دلیل وجود parent خطای circular structure میدهد هنگام استفاده از JSON.stringify معمولی
 * @param obj block or blocks
 * @returns JSON string representation of the block(s)
 */
export function getBlocksAsString(obj: any) {
  return JSON.stringify(obj, (key, value) => {
    return key === 'parent' ? undefined : value;
  });
}
