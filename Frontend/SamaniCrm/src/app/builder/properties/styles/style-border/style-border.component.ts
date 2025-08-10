import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  forwardRef,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { cloneDeep } from 'lodash-es';
import { SizeUnitList } from '../models/SizeUnit';
import { BorderTypeList } from '../models/BorderStyle';

/**
 * فرض می‌کنیم این اینترفیس‌ها مطابق مدل‌های تو هستند.
 * اگر مدل اصلی متفاوت است، فقط تطبیق بده.
 */
export type BorderType =
  | 'none'
  | 'hidden'
  | 'dotted'
  | 'dashed'
  | 'solid'
  | 'double'
  | 'groove'
  | 'ridge'
  | 'inset'
  | 'outset'
  | string;
export interface Border {
  size?: number;
  unit?: string; // px, em, rem, %
  type?: BorderType;
  color?: string;
  inset?: boolean;
}
export interface BorderStyle {
  forAll?: boolean;
  border?: Border | null;
  top?: Border | null;
  right?: Border | null;
  bottom?: Border | null;
  left?: Border | null;
}

/**
 * Component: StyleBorderComponent
 * - writeValue now accepts string (css) or BorderStyle
 * - strong parsing & normalization for border declarations
 */
@Component({
  standalone: false,
  selector: 'style-border',
  templateUrl: './style-border.component.html',
  styleUrls: ['./style-border.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => StyleBorderComponent),
      multi: true,
    },
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StyleBorderComponent implements OnInit, ControlValueAccessor {
  @Input() label = '';
  borderTypeList = BorderTypeList;
  sizeUnitList = SizeUnitList;

  showPopup = false;
  left = 0;
  top = 0;

  value?: BorderStyle;
  borderString = '';
  defaultBorder: Border = {
    color: '#ececec',
    inset: false,
    unit: 'px',
    type: 'solid',
    size: 1,
  };

  onChangeFn: any = () => {};
  onTouchedFn: any = () => {};

  @Output() css = new EventEmitter<string>();

  constructor(
    private el: ElementRef<HTMLElement>,
    private ch: ChangeDetectorRef,
  ) {}

  ngOnInit() {
    this.onChangeAll();
  }

  /**************************
   * ControlValueAccessor
   **************************/
  writeValue(val: BorderStyle | string | null): void {
    if (val == null) {
      this.value = undefined;
      this.borderString = '';
      this.onChangeAll();
      return;
    }

    if (typeof val === 'string') {
      // parse CSS string -> BorderStyle
      const parsed = this.parseCssBorderString(val);
      if (parsed) {
        this.value = parsed;
        this.borderString = this.getCssFromBorderStyle(this.value);
        this.onChangeAll();
      } else {
        // اگر رشته معتبر نبود، مقدار قبلی را نشکن (می‌توان لاگ کرد)
        console.warn('Invalid border string provided to StyleBorderComponent:', val);
      }
    } else {
      // BorderStyle object
      this.value = cloneDeep(val);
      this.borderString = this.getCssFromBorderStyle(this.value);
      this.onChangeAll();
    }
  }

  registerOnChange(fn: any): void {
    this.onChangeFn = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouchedFn = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    // optional
  }

  /**************************
   * UI helpers
   **************************/
  onSelectForAll(forAll: boolean) {
    this.value = !this.value ? { forAll } : this.value;
    this.value.forAll = forAll;
    this.onChangeAll();
  }
  onChangeAll() {
    if (!this.value) return;
    if (this.value.forAll && !this.value.border) {
      this.value.border = cloneDeep(this.defaultBorder);
    } else {
      this.value.top ??= cloneDeep(this.value.border || this.defaultBorder);
      this.value.right ??= cloneDeep(this.value.border || this.defaultBorder);
      this.value.bottom ??= cloneDeep(this.value.border || this.defaultBorder);
      this.value.left ??= cloneDeep(this.value.border || this.defaultBorder);
    }
    this.emitChange();
  }

  emitChange() {
    this.onChangeFn(this.value);
    this.borderString = this.getCssFromBorderStyle(this.value);
    this.css.emit(this.borderString);
  }

  remove(name: string) {
    if (this.value) (this.value as any)[name] = {};
    this.emitChange();
  }

  togglePanel() {
    const rect = this.el.nativeElement.getBoundingClientRect();
    this.top = rect.bottom;
    this.left = rect.left;
    if (!this.value) {
      this.value = {};
    }
    this.onChangeAll();
    this.showPopup = !this.showPopup;
    this.ch.detectChanges();
  }

  stringChange() {
    this.writeValue(this.borderString);
  }

  /**************************
   * Parsing utilities
   **************************/

  /**
   * پارس کردن ورودی CSS که می‌تواند شامل:
   *  - "border: 1px solid #000;"
   *  - "border-top: 2px dashed rgba(0,0,0,0.3);"
   *  - یا چند declarado جدا شده با ;
   *
   * خروجی: BorderStyle یا null اگر چیزی نتوانست پارس شود
   */
  private parseCssBorderString(input: string): BorderStyle | null {
    if (!input || !input.trim()) return null;
    const decls = input
      .split(';')
      .map((d) => d.trim())
      .filter(Boolean);

    // نتیجه موقت
    const result: BorderStyle = { forAll: false, border: null, top: null, right: null, bottom: null, left: null };

    let anyParsed = false;

    for (const decl of decls) {
      // split property: value
      const idx = decl.indexOf(':');
      if (idx === -1) continue;
      const prop = decl.slice(0, idx).trim().toLowerCase();
      const val = decl.slice(idx + 1).trim();

      if (prop === 'border') {
        const parsed = this.parseBorderShorthand(val);
        if (parsed) {
          result.forAll = true;
          result.border = parsed;
          anyParsed = true;
        }
      } else if (prop.startsWith('border-') && prop.split('-').length === 3) {
        const side = prop.split('-')[1]; // top,right,bottom,left
        const parsed = this.parseBorderShorthand(val);
        if (parsed) {
          anyParsed = true;
          switch (side) {
            case 'top':
              result.top = parsed;
              break;
            case 'right':
              result.right = parsed;
              break;
            case 'bottom':
              result.bottom = parsed;
              break;
            case 'left':
              result.left = parsed;
              break;
          }
        }
      } else if (prop === 'border-width' || prop === 'border-style' || prop === 'border-color') {
        // پشتیبانی از شورت‌هند عمومی (می‌تونیم آن را برای هر سمت اعمال کنیم)
        const parsedParts = this.splitWhitespacePreservingQuotes(val);
        // برای سادگی: اگر یک مقدار باشه برای همه، اگر چهار مقدار باشه مطابق CSS تخصیص بده
        // این بخش میتونه پیشرفته‌تر بشه؛ فعلاً حالت پایه را پشتیبانی می‌کنیم
        if (prop === 'border-width') {
          const sizes = this.expandCssTrbl(parsedParts);
          ['top', 'right', 'bottom', 'left'].forEach((side, i) => {
            const s = sizes[i];
            const parsedSize = this.parseSizeUnit(s);
            if (!parsedSize) return;
            const current = (result as any)[side] ?? { ...this.defaultBorder };
            current.size = parsedSize.value;
            current.unit = parsedSize.unit;
            (result as any)[side] = current;
            anyParsed = true;
          });
        } else if (prop === 'border-style') {
          const styles = this.expandCssTrbl(parsedParts);
          ['top', 'right', 'bottom', 'left'].forEach((side, i) => {
            const s = styles[i];
            if (!s) return;
            const current = (result as any)[side] ?? { ...this.defaultBorder };
            current.type = s as BorderType;
            (result as any)[side] = current;
            anyParsed = true;
          });
        } else if (prop === 'border-color') {
          const colors = this.expandCssTrbl(parsedParts);
          ['top', 'right', 'bottom', 'left'].forEach((side, i) => {
            const s = colors[i];
            if (!s) return;
            const current = (result as any)[side] ?? { ...this.defaultBorder };
            current.color = this.normalizeColor(s) || current.color;
            (result as any)[side] = current;
            anyParsed = true;
          });
        }
      } else {
        // نادیده بگیر بقیه propها
      }
    }

    if (!anyParsed) return null;

    // اگر forAll set شده، بقیه را خالی نذار؛ نرمال‌سازی کن
    if (result.forAll && result.border) {
      // nothing: caller will use .forAll flag
    } else {
      // پر کردن از default اگر یکی از سایدها خالی موند
      ['top', 'right', 'bottom', 'left'].forEach((side) => {
        if (!(result as any)[side]) (result as any)[side] = cloneDeep(this.defaultBorder);
      });
    }

    return result;
  }

  /**
   * parseBorderShorthand("1px solid #000 inset") => Border
   * قبول می‌کند order های مختلف را (size, style, color, inset)
   */
  private parseBorderShorthand(value: string): Border | null {
    if (!value || value.trim().length === 0) return null;
    // split respecting rgb(...) and calc(...) etc
    const parts = this.splitWhitespacePreservingQuotes(value);

    const border: Border = { ...cloneDeep(this.defaultBorder) };

    for (const p of parts) {
      // size?
      const sizeParsed = this.parseSizeUnit(p);
      if (sizeParsed) {
        border.size = sizeParsed.value;
        border.unit = sizeParsed.unit;
        continue;
      }

      // style?
      if (this.isBorderStyleToken(p)) {
        border.type = p as BorderType;
        continue;
      }

      // inset/outset keyword
      if (p.toLowerCase() === 'inset') {
        border.inset = true;
        continue;
      }
      if (p.toLowerCase() === 'outset') {
        border.inset = false;
        continue;
      }

      // color?
      const color = this.normalizeColor(p);
      if (color) {
        border.color = color;
        continue;
      }

      // اگر به اینجا رسید یعنی توکن ناشناخته است — نادیده بگیر
    }

    // اگر size و type و color همه نبودن، ممکن است مقدار none یا 0 باشه
    if ((!border.size || border.size === 0) && (!border.type || border.type === 'none')) {
      // اگر مقدار value 'none' یا '0' بود، بازگشت نکن
      const v = value.trim().toLowerCase();
      if (v === 'none' || v === '0') {
        return { type: 'none', size: 0, unit: 'px', color: '' };
      }
    }

    return border;
  }

  /**
   * helper: تشخیص token های style
   */
  private isBorderStyleToken(token: string) {
    const t = token.toLowerCase();
    return ['none', 'hidden', 'dotted', 'dashed', 'solid', 'double', 'groove', 'ridge', 'inset', 'outset'].includes(t);
  }

  /**
   * parseSizeUnit('2px') => { value: 2, unit: 'px' } or null
   */
  private parseSizeUnit(token: string): { value: number; unit: string } | null {
    if (!token) return null;
    // support: 0 (no unit), px, em, rem, %, pt, etc.
    const m = /^(-?\d+(\.\d+)?)([a-z%]+)?$/i.exec(token.trim());
    if (!m) return null;
    const value = parseFloat(m[1]);
    const unit = (m[3] || 'px').toLowerCase();
    return { value, unit };
  }

  /**
   * normalizeColor: خیلی ساده رنگ‌ها را نرمال می‌کند
   * - hex #rgb/#rrggbb/#rrggbbaa
   * - rgb(), rgba(), hsl(), hsla()
   * - نام رنگ (fallback) => برگشت همان رشته (مرجع‌پذیر)
   *
   * برای کامل‌تر کردن می‌توانید mapping نام رنگ‌ها را اضافه کنید.
   */
  private normalizeColor(token: string): string | null {
    if (!token) return null;
    const t = token.trim();
    // hex
    if (/^#([0-9a-f]{3,8})$/i.test(t)) return t;
    // rgb/rgba/hsl/hsla
    if (/^(rgb|rgba|hsl|hsla)\(/i.test(t)) return t;
    // css var
    if (/^var\(/i.test(t)) return t;
    // named color (we'll accept any word as color name)
    if (/^[a-zA-Z]+$/.test(t)) return t;
    return null;
  }

  /**
   * split by whitespace but keep functions like rgb(...) intact
   */
  private splitWhitespacePreservingQuotes(input: string): string[] {
    const parts: string[] = [];
    let cur = '';
    let depth = 0;
    for (let i = 0; i < input.length; i++) {
      const ch = input[i];
      if (ch === '(') {
        depth++;
        cur += ch;
        continue;
      } else if (ch === ')') {
        depth = Math.max(0, depth - 1);
        cur += ch;
        continue;
      } else if (/\s/.test(ch) && depth === 0) {
        if (cur.trim()) {
          parts.push(cur.trim());
          cur = '';
        }
        continue;
      } else {
        cur += ch;
      }
    }
    if (cur.trim()) parts.push(cur.trim());
    return parts;
  }

  /**
   * expand CSS TRBL shorthand list
   * inputParts: ['1px'] or ['1px','2px'] or ['1px','2px','3px','4px']
   * return array of 4 values [top,right,bottom,left]
   */
  private expandCssTrbl(parts: string[]): string[] {
    if (!parts || parts.length === 0) return [];
    if (parts.length === 1) return [parts[0], parts[0], parts[0], parts[0]];
    if (parts.length === 2) return [parts[0], parts[1], parts[0], parts[1]];
    if (parts.length === 3) return [parts[0], parts[1], parts[2], parts[1]];
    return [parts[0], parts[1], parts[2], parts[3]];
  }

  /**************************
   * CSS generator (normalizer)
   **************************/
  public getCssFromBorderStyle(b?: BorderStyle): string {
    if (!b) return '';
    if (b.forAll && b.border?.size !== undefined) {
      const bo = b.border;
      const size = bo.size ?? 0;
      const unit = bo.unit ?? 'px';
      const type = bo.type ?? 'solid';
      const color = bo.color ?? 'black';
      return `border: ${size}${unit} ${type} ${color};`;
    } else {
      const top = this.toCssBorderValue(b.top);
      const right = this.toCssBorderValue(b.right);
      const bottom = this.toCssBorderValue(b.bottom);
      const left = this.toCssBorderValue(b.left);
      const parts: string[] = [];
      if (top) parts.push(`border-top: ${top};`);
      if (right) parts.push(`border-right: ${right};`);
      if (bottom) parts.push(`border-bottom: ${bottom};`);
      if (left) parts.push(`border-left: ${left};`);
      return parts.join(' ');
    }
  }

  private toCssBorderValue(border?: Border | null): string {
    if (!border) return 'none';
    if (border.type === 'none') return 'none';
    const size = border.size ?? 0;
    const unit = border.unit ?? 'px';
    const type = border.type ?? 'solid';
    const color = border.color ?? '';
    return `${size}${unit} ${type} ${color}`.trim();
  }
}
