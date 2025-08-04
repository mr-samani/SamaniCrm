import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Pipe({
  name: 'styleView',
  standalone: true,
})
export class StyleViewPipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) {}
  transform(value: string): SafeHtml {
    if (!value) return '';

    // جدا کردن استایل‌ها با استفاده از سمی‌کالن
    const styles = value.split(';').filter((style) => style.trim() !== '');

    // تبدیل هر استایل به یک خط با رنگ‌بندی
    const formattedStyles = styles
      .map((style) => {
        const [property, val] = style.split(':').map((s) => s.trim());
        if (property && val) {
          return `<span style="color: #7878fc">${property}</span>: <span style="color:#949494">${val}</span>`;
        }
        return '';
      })
      .join(';<br>');

    return this.sanitizer.bypassSecurityTrustHtml(formattedStyles + '<br>');
  }
}
