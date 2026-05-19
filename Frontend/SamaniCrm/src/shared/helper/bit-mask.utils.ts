// utils/bitmask.util.ts

export abstract class Bitmask {
    /**
     * دریافت تمام فلگ‌های فعال از هر enum
     */
    static getActiveFlags<T extends Record<string, number>>(mask: number, enumObj: T): number[] {
        const flags: number[] = [];

        for (const key in enumObj) {
            const value = enumObj[key];
            if (typeof value === 'number' && (mask & value) !== 0) {
                flags.push(value);
            }
        }

        return flags;
    }

    /**
     * دریافت فلگ‌ها با اطلاعات کامل (value + key)
     */
    static getActiveFlagInfos<T>(mask: number, enumObj: Object): T[] {
        const infos: T[] = [];
        const allFlag = this.allFlag(enumObj);
        for (const key in enumObj) {
            const value = (enumObj as any)[key];
            if (typeof value === 'number' && (mask & value) !== 0 && value != allFlag) {
                infos.push((enumObj as any)[key]);
            }
        }

        return infos;
    }

    static enumArrayToFlag(enumFlag: number[]): number {
        return enumFlag.reduce((c: number, p: number) => (c = c | p), 0);
    }

    /**
     * بررسی فعال بودن یک فلگ
     */
    static hasFlag(mask: number, flag: number): boolean {
        return (mask & flag) !== 0;
    }

    /**
     * اضافه کردن فلگ به mask
     */
    static addFlag(mask: number, flag: number): number {
        return mask | flag;
    }

    /**
     * حذف فلگ از mask
     */
    static removeFlag(mask: number, flag: number): number {
        return mask & ~flag;
    }

    /**
     * ساخت mask از آرایه فلگ‌ها
     */
    static buildMask(flags: number[]): number {
        return flags.reduce((mask, flag) => mask | flag, 0);
    }

    static allFlag(enumFlag: Object): number {
        let infos: number[] = [];
        for (const key in enumFlag) {
            const value = (enumFlag as any)[key];
            if (typeof value === 'number' && key.toString().toLowerCase() !== 'all') {
                infos.push((enumFlag as any)[key]);
            }
        }
        return this.enumArrayToFlag(infos);
    }
}
