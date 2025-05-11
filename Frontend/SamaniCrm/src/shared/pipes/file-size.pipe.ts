import { Pipe, PipeTransform } from '@angular/core';
import { humanFileSize } from '@shared/helper/file.helper';

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
