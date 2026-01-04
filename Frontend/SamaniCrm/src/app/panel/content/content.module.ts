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
import { SharedModule } from '@shared/shared.module';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';
import { CreateOrEditPageMetaDataDialogComponent } from './create-or-edit-page-meta-data-dialog/create-or-edit-page-meta-data-dialog.component';
import {
  providePageBuilder,
  NgxPageBuilder,
  NGX_PAGE_BUILDER_FILE_PICKER,
  NGX_PAGE_BUILDER_HTML_EDITOR,
} from 'ngx-page-builder';
import { FilePickerService } from './page-builder/file-picker.service';
import { HtmlEditorService } from './page-builder/html-editor.service';
import { PagesServiceProxy } from '@shared/service-proxies/api/pages.service';
@NgModule({
  declarations: [ContentComponent, PagesComponent],
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
  ],
  providers: [
    PagesServiceProxy,
    providePageBuilder({
      enableHistory: true,
      //  storageType: StorageType.None,
      toolbarConfig: {
        showSaveButton: true,
        showConfigButton: false,
        showExportHtmlButton: false,
        showImportHtmlButton: true,
        showOpenButton: false,
        showPreviewButton: true,
        showPrintButton: true,
      },
    }),
  ],
})
export class ContentModule {}
