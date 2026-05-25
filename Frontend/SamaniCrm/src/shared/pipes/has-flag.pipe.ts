import { Pipe, PipeTransform } from '@angular/core';
import { Bitmask } from '@shared/helper/bit-mask.utils';

@Pipe({
    name: 'HasFlag',
    standalone: false,
})
export class HasFlagPipe implements PipeTransform {
    transform(mask: number, flag: number) {
        return Bitmask.hasFlag(mask, flag);
    }
}
 