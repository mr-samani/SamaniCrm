import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Pipe({
  name: 'sanitizer',
  standalone: false,
})  

export class SanitizerPipe implements PipeTransform {
  constructor(private domSanitizer: DomSanitizer) {}
  transform(value: string): unknown {
    return this.domSanitizer.bypassSecurityTrustHtml(value);
  }
}
