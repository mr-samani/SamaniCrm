import { BoxShadowValue } from '../models/BoxShadow';

export const boxShadowRegex =
  /((?:inset\s+)?)(-?\d+(?:\.\d+)?\w+)(?:\s+(-?\d+(?:\.\d+)?\w+))?(?:\s+(-?\d+(?:\.\d+)?\w+))?(?:\s+(-?\d+(?:\.\d+)?\w+))?(?:\s+((?:rgba?|hsla?)\([^)]*\)|#[0-9a-fA-F]{3,8}|\w+))?/g;

// ✅ تبدیل string box-shadow به آرایه‌ای از اشیاء
export function parseBoxShadow(input: string): BoxShadowValue[] {
  const shadows: BoxShadowValue[] = [];
  const pattern = boxShadowRegex;
  let match;
  while ((match = pattern.exec(input)) !== null) {
    const [_, insetStr, ox, oy, blur, spread, color] = match;
    shadows.push({
      inset: insetStr.trim() === 'inset',
      offsetX: ox,
      offsetY: oy ?? '0px',
      blurRadius: blur ?? '0px',
      spreadRadius: spread ?? '0px',
      color: color?.trim(),
    });
  }
  return shadows;
}

// ✅ تبدیل آرایه‌ای از اشیاء به string box-shadow
export function stringifyBoxShadow(shadows: BoxShadowValue[]): string {
  if (!shadows || shadows.length === 0) return '';
  return shadows
    .map((s) => {
      const parts = [];
      if (s.inset) parts.push('inset');
      parts.push(s.offsetX);
      parts.push(s.offsetY);
      if (s.blurRadius) parts.push(s.blurRadius);
      if (s.spreadRadius) parts.push(s.spreadRadius);
      if (s.color) parts.push(s.color);
      return parts.join(' ');
    })
    .join(', ');
}
