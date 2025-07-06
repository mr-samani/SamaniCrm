import { PageViewComponent } from './page-view/page-view.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageViewRoutingModule } from './page-view-routing.module';
import { PagesServiceProxy } from '@shared/service-proxies/api/pages.service';
import { SharedModule } from '@shared/shared.module';

@NgModule({
  declarations: [PageViewComponent],
  imports: [CommonModule, PageViewRoutingModule, SharedModule],
  providers: [PagesServiceProxy],
})
export class PageViewModule {}
