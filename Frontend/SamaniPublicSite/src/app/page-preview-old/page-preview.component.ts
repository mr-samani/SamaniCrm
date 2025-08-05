import {
  AfterViewInit,
  Component,
  ElementRef,
  Inject,
  Injector,
  PLATFORM_ID,
  signal,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { DomSanitizer } from '@angular/platform-browser';
import { BaseComponent } from '@app/base-components';
import { PageDto, PagesServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs';
import { componentCss } from './componentCss';
import { SharedModule } from '@shared/shared.module';
import * as DOMPurify from 'dompurify';
import { SsrRendererComponent } from './ssr-renderer.component';
@Component({
  selector: 'app-page-preview',
  templateUrl: './page-preview.component.html',
  styleUrls: ['./page-preview.component.scss'],
  imports: [SharedModule, CommonModule, SsrRendererComponent],
  encapsulation: ViewEncapsulation.None, // 👈 مهم برای SSR
  providers: [PagesServiceProxy],
})
export class PagePreviewOldComponent extends BaseComponent implements AfterViewInit {
  id: string;
  culture: string;
  pageInfo = new PageDto();
  loading = signal(false);


  page:any={};


  
  @ViewChild('container', { static: false }) container!: ElementRef;

  constructor(
    injector: Injector,
    private pageService: PagesServiceProxy,
    private sanitizer: DomSanitizer,
    @Inject(PLATFORM_ID) private platformId: any
  ) {
    super(injector);
    this.culture = this.route.snapshot.params['culture'];
    this.id = this.route.snapshot.params['id'];
  }

  ngAfterViewInit() {
    this.loading.set(true);
    this.pageService
      .getPageInfo(this.id, this.culture)
      .pipe(
        finalize(() => {
          this.loading.set(false);
          // this.cd.detectChanges();
        })
      )
      .subscribe((result) => {
        this.pageInfo = result.data ?? new PageDto();
        const containerEl = this.container.nativeElement as HTMLElement;

        const styles = this.pageInfo.styles ?? '';
        const html = this.pageInfo.html ?? '';

        containerEl.innerHTML = `
          <style>${componentCss + '\n' + styles}</style>
          ${DOMPurify.default.sanitize(html)}
        `;

        // فقط اسکریپت در مرورگر اجرا شود
        if (isPlatformBrowser(this.platformId) && this.pageInfo.scripts) {
          const patchedScript = this.pageInfo.scripts;
          const blob = new Blob([patchedScript], { type: 'text/javascript' });
          const script = document.createElement('script');
          script.src = URL.createObjectURL(blob);
          document.body.appendChild(script);
        }

        this.cd.detectChanges();
      });
  }
}
