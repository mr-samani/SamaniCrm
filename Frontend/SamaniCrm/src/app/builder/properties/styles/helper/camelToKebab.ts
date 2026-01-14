export function camelToKebab(str: string): string {
  return str.replace(/[A-Z]/g, (m) => '-' + m.toLowerCase());
}
