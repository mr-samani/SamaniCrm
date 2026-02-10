import { PageViewComponent } from './page-view/page-view.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageViewRoutingModule } from './page-view-routing.module';
import { PagesServiceProxy } from '@shared/service-proxies/api/pages.service';
import { SharedModule } from '@shared/shared.module';
import { NgxPagePreviewComponent } from 'ngx-page-builder/preview';
import { providePagePreview } from 'ngx-page-builder/preview';
import { CUSTOM_BLOCKS } from './custom-blocks/CustomBlocks';
@NgModule({
  declarations: [PageViewComponent],
  imports: [CommonModule, PageViewRoutingModule, SharedModule, NgxPagePreviewComponent],
  providers: [
    PagesServiceProxy,
    providePagePreview({
      customSources: CUSTOM_BLOCKS,
      publicCss: ['/bootstrap/bootstrap.min.css'],
      publicJs: ['/bootstrap/bootstrap.min.js'],
    }),
  ],
})
export class PageViewModule {}
