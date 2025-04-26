import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'fileSize',
  standalone: false,
})
export class FileSizePipe implements PipeTransform {
  transform(size: number | undefined) {
    if (size === null || size === undefined) {
      return '';
    }
    return humanFileSize(size);
  }
}

/**
 * تبدیل حجم بایت به واحد قابل فهم کاربر
 * @param size  بایت
 * @returns KB , MB, GB, TB, PB, EB, ZB YB
 */
export function humanFileSize(size: number) {
  if (size < 1024) return size + ' B';
  let i = Math.floor(Math.log(size) / Math.log(1024));
  let num = size / Math.pow(1024, i);
  let round = Math.round(num);
  num = round < 10 ? +num.toFixed(2) : round < 100 ? +num.toFixed(1) : round;
  return `${num} ${'KMGTPEZY'[i - 1]}B`;
}
