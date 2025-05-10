import { AbstractControl } from '@angular/forms';

export function RequireMatch(control: AbstractControl) {
    const selection: any = control.value;
    if (typeof selection === 'string' && control.value) {
        return { incorrect: true };
    }
    return null;
}
