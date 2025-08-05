import { SizeUnit } from '../models/SizeUnit';
import { Spacing, SpacingValue } from '../models/Spacing';

export const spacingRegex = /^((\d+(\.\d+)?(px|em|rem|vw|vh|%)?|auto)(\s+)?){1,4}$/;

export function parseSpacing(input: string): Spacing | null {
  const trimmed = input.trim();
  if (!spacingRegex.test(trimmed)) return null;

  const parts = trimmed.split(/\s+/);
  const values: SpacingValue[] = [];

  for (const p of parts) {
    if (p === 'auto') {
      values.push({ size: 'auto', unit: 'auto' });
    } else {
      const match = p.match(/^(\d+(\.\d+)?)(px|em|rem|vw|vh|%)?$/);
      if (!match) return null;
      values.push({
        size: parseFloat(match[1]),
        unit: (match[3] as SizeUnit) || 'px',
      });
    }
  }

  const get = (i: number): SpacingValue => {
    const base = values[i] || values[0];
    return base ? { ...base } : { size: 0, unit: 'px' }; // clone
  };

  switch (values.length) {
    case 1:
      return { top: get(0), right: get(0), bottom: get(0), left: get(0) };
    case 2:
      return { top: get(0), right: get(1), bottom: get(0), left: get(1) };
    case 3:
      return { top: get(0), right: get(1), bottom: get(2), left: get(1) };
    case 4:
      return { top: get(0), right: get(1), bottom: get(2), left: get(3) };
    default:
      return null;
  }
}

/*
        parseSpacing("5px auto")
        // ✅ { top: 5px, right: auto, bottom: 5px, left: auto }

        parseSpacing("10px 15px 20px auto")
        // ✅ { top: 10px, right: 15px, bottom: 20px, left: auto }

        parseSpacing("auto")
        // ✅ همه جهات auto

        parseSpacing("15px")
        // ✅ همه جهات 15px
*/
/*-------------------------------------------------------------------------------------------------*/

export function getCssFromSpacingStyle(p?: Spacing | null): string {
  if (!p) return '';

  const toStr = (val?: SpacingValue): string => {
    if (!val) return '0';
    if (val.size === 'auto') return 'auto';
    return `${val.size}${val.unit}`;
  };

  const t = toStr(p.top);
  const r = toStr(p.right);
  const b = toStr(p.bottom);
  const l = toStr(p.left);

  if (t === r && t === b && t === l) {
    return t;
  } else if (t === b && r === l) {
    return `${t} ${r}`;
  } else if (r === l) {
    return `${t} ${r} ${b}`;
  } else {
    return `${t} ${r} ${b} ${l}`;
  }
}
