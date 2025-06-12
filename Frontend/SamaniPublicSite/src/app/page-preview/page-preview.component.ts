import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, Injector, signal, ViewChild, ViewEncapsulation } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { BaseComponent } from '@app/base-components';
import { PageDto, PagesServiceProxy } from '@shared/service-proxies';
import { SharedModule } from '@shared/shared.module';
import { finalize } from 'rxjs';
import DOMPurify from 'DOMPurify';
import { componentCss } from './componentCss';
@Component({
  selector: 'app-page-preview',
  templateUrl: './page-preview.component.html',
  styleUrls: ['./page-preview.component.scss'],
  standalone: true,
  imports: [CommonModule, SharedModule],
  providers: [PagesServiceProxy],
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class PagePreviewComponent extends BaseComponent implements AfterViewInit {
  id: string;
  culture: string;

  loading = signal(true);
  pageInfo = new PageDto();

  @ViewChild('container', { static: false }) container!: ElementRef;
  constructor(injector: Injector, private pageService: PagesServiceProxy, private sanitizer: DomSanitizer) {
    super(injector);
    this.culture = this.route.snapshot.params['culture'];
    this.id = this.route.snapshot.params['id'];
  }

  ngAfterViewInit() {
    this.getData();
  }

  getData() {
    this.loading.set(true);
    this.pageService
      .getPageInfo(this.id, this.culture)
      .pipe(
        finalize(() => {
          this.loading.set(false);
          this.cd.detectChanges();
        })
      )
      .subscribe(async (result) => {
        this.pageInfo = result.data ?? new PageDto();
        if (this.pageInfo) {
          const containerEl = this.container.nativeElement as HTMLElement;

          const shadowRoot = containerEl.shadowRoot ?? containerEl.attachShadow({ mode: 'open' });

          // پاکسازی قبلی
          shadowRoot.innerHTML = '';

          // Inject Styles
          const style = document.createElement('style');
          const styles = this.pageInfo.styles ?? '';
          style.textContent = `${componentCss}\n${styles}`;

          // Inject HTML
          const htmlWrapper = document.createElement('div');
          htmlWrapper.innerHTML = DOMPurify.sanitize(this.pageInfo.html ?? '');

          // Append all
          shadowRoot.appendChild(style);
          shadowRoot.appendChild(htmlWrapper);

          // Inject Script
          // ✅ Inject script using a Blob (to actually execute it)
          if (this.pageInfo.scripts) {
            const originalScript = this.pageInfo.scripts ?? '';

            // Expose shadowRoot to the script
            (window as any).__gjsCurrentShadowRoot = shadowRoot;
            // console.log(this.pageInfo.scripts);
            // دستکاری ساده برای جایگزینی `document.querySelectorAll` با `window.__gjsCurrentShadowRoot.querySelectorAll`
            const patchedScript = originalScript.replace(
              /document\.querySelectorAll/g,
              'window.__gjsCurrentShadowRoot.querySelectorAll'
            );

            // حالا inject کن
            const blob = new Blob([patchedScript], { type: 'text/javascript' });
            const blobUrl = URL.createObjectURL(blob);
            const script = document.createElement('script');
            script.src = blobUrl;
            shadowRoot.appendChild(script);
          }

          this.cd.detectChanges();
        }
      });
  }
}
