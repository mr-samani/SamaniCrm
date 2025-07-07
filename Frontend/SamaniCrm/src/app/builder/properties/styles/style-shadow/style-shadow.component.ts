import { Component, forwardRef, Injector } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { parseBoxShadow, stringifyBoxShadow } from '../helper/parse-box-shadow';

@Component({
  selector: 'style-shadow',
  templateUrl: './style-shadow.component.html',
  styleUrls: ['./style-shadow.component.scss'],
  standalone: false,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => StyleShadowComponent),
      multi: true,
    },
  ],
})
export class StyleShadowComponent extends AppComponentBase implements ControlValueAccessor {
  boxShadowString = '';
  showPanel = false;
  inset = false;
  offsetX = 0;
  offsetY = 0;
  blurRadius = 0;
  spreadRadius = 0;
  color = '#000000';

  isInvalid = false;

  onChange = (_: string) => {};
  onTouched = () => {};

  constructor(injector: Injector) {
    super(injector);
  }

  writeValue(val: string | undefined): void {
    this.boxShadowString = val ?? '';
    this.tryParse(this.boxShadowString);
  }

  tryParse(value: string) {
    try {
      if (!value) {
        this.isInvalid = false;
        return;
      }
      let parsed = parseBoxShadow(value)[0];
      if (parsed) {
        this.inset = parsed.inset == true;
        this.offsetX = parseInt(parsed.offsetX) || 0;
        this.offsetY = parseInt(parsed.offsetY) || 0;
        this.blurRadius = parseInt(parsed.blurRadius ?? '0') || 0;
        this.spreadRadius = parseInt(parsed.spreadRadius ?? '0') || 0;
        this.color = parsed.color || '#000000';
        this.isInvalid = false;
      } else {
        this.isInvalid = true;
      }
    } catch (error) {
      this.isInvalid = true;
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  onUiChange() {
    this.boxShadowString = stringifyBoxShadow([
      {
        offsetX: this.offsetX + 'px',
        offsetY: this.offsetY + 'px',
        blurRadius: this.blurRadius + 'px',
        spreadRadius: this.spreadRadius + 'px',
        color: this.color,
        inset: this.inset,
      },
    ]);
    this.onChange(this.boxShadowString);
    this.onTouched();
  }

  stringChange() {
    this.tryParse(this.boxShadowString);
    this.onChange(this.boxShadowString);
    this.onTouched();
  }
  togglePanel() {
    this.offsetX ??= 0;
    this.offsetY ??= 0;
    this.blurRadius ??= 0;
    this.spreadRadius ??= 0;
    this.color ??= '#000000';
    this.inset ??= false;
    this.showPanel = !this.showPanel;
  }
}
