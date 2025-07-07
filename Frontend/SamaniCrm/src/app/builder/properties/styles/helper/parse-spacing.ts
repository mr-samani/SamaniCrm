import { SizeUnit } from '../models/SizeUnit';
import { Spacing } from '../models/Spacing';

// ✅ Regex برای تشخیص مقدارهای margin/padding مشابه مرورگر
export const spacingRegex = /^(\d+(\.\d+)?)(px|em|rem|vw|vh|%)?(\s+(\d+(\.\d+)?)(px|em|rem|vw|vh|%)?){0,3}$/;

export function parseSpacing(input: string): Spacing {
  if (!spacingRegex.test(input.trim()))
    return {
      top: { size: 0, unit: 'px' },
      right: { size: 0, unit: 'px' },
      bottom: { size: 0, unit: 'px' },
      left: { size: 0, unit: 'px' },
    };

  const parts = input.trim().split(/\s+/);
  const values: any[] = parts.map((p) => {
    const match = p.match(/^(\d+(\.\d+)?)(px|em|rem|vw|vh|%)?$/);
    if (!match) return { size: 0, unit: 'px' }; // fallback
    return {
      size: parseFloat(match[1]),
      unit: match[3] || 'px',
    };
  });

  const get = (i: number) => values[i] || values[0];

  if (values.length === 1) {
    return {
      top: get(0),
      right: get(0),
      bottom: get(0),
      left: get(0),
    };
  } else if (values.length === 2) {
    return {
      top: get(0),
      right: get(1),
      bottom: get(0),
      left: get(1),
    };
  } else if (values.length === 3) {
    return {
      top: get(0),
      right: get(1),
      bottom: get(2),
      left: get(1),
    };
  } else {
    return {
      top: get(0),
      right: get(1),
      bottom: get(2),
      left: get(3),
    };
  }
}
