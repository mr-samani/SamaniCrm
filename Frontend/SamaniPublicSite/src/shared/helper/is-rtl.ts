export function isRtl(lang: string) {
  return ['fa', 'fa-ir', 'ar'].indexOf(lang.toLowerCase()) > -1;
}
