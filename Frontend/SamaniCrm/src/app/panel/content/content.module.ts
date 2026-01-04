import { EditPageComponent } from './edit-page/edit-page.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ContentRoutingModule } from './content-routing.module';
import { ContentComponent } from './content.component';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { PagesComponent } from './pages/pages.component';
import { TableViewModule } from '@shared/components/table-view/table-view.module';
import { PaginationComponent } from '@shared/components/pagination/pagination.component';
import { PagesServiceProxy } from '@shared/service-proxies';
import { SharedModule } from '@shared/shared.module';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';
import { CreateOrEditPageMetaDataDialogComponent } from './create-or-edit-page-meta-data-dialog/create-or-edit-page-meta-data-dialog.component';
import { providePageBuilder, NgxPageBuilder } from 'ngx-page-builder';
@NgModule({
  declarations: [ContentComponent, PagesComponent, EditPageComponent],
  imports: [
    CommonModule,
    ContentRoutingModule,
    MaterialCommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    TableViewModule,
    PaginationComponent,
    SharedModule,
    TabGroupModule,
    CreateOrEditPageMetaDataDialogComponent,
    NgxPageBuilder,
  ],
  providers: [
    PagesServiceProxy,
    // providePageBuilder({})
  ],
})
export class ContentModule {}
