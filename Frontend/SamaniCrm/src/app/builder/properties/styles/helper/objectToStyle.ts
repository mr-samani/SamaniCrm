import { camelToKebab } from './camelToKebab';

export function objectToStyle(styles?: Partial<CSSStyleDeclaration>): string {
  if (!styles) return '';
  let css = '';
  Object.entries(styles).forEach(([key, value]) => {
    if (value != '') css += `${camelToKebab(key)}:${value};`;
  });
  return css;
}
