export function isNullOrEmpty(value: any): boolean {
    if (value === undefined || value === null || value === '' || value == 'null' || value == 'undefined') {
        return true;
    } else {
        return false;
    }
}
