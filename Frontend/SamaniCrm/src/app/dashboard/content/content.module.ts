import { CreatePageDialogComponent } from './create-page-dialog/create-page-dialog.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ContentRoutingModule } from './content-routing.module';
import { ContentComponent } from './content.component';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { BlogsComponent } from './blogs/blogs.component';
import { PagesComponent } from './pages/pages.component';
import { TableViewComponent } from '@shared/components/table-view/table-view.compoenent';
import { PaginationComponent } from '@shared/components/pagination/pagination.component';
import { PagesServiceProxy } from '@shared/service-proxies';

@NgModule({
  declarations: [ContentComponent, BlogsComponent, PagesComponent, CreatePageDialogComponent],
  imports: [
    CommonModule,
    ContentRoutingModule,
    MaterialCommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    TableViewComponent,
    PaginationComponent,
  ],
  providers: [PagesServiceProxy],
})
export class ContentModule {}
