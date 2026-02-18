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
import { providePageBuilder, NGX_PAGE_BUILDER_EXPORT_PLUGIN_STORE } from 'ngx-page-builder/designer';
import { PagesServiceProxy } from '@shared/service-proxies/api/pages.service';
import { PluginService } from './page-builder/plugin.service';
import { PageBuilderServiceProxy } from '@shared/service-proxies/api/page-builder.service';
import { CUSTOM_BLOCKS } from './page-builder/custom-blocks/CustomBlocks';
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
    PluginService,
    PageBuilderServiceProxy,
    PagesServiceProxy,
    providePageBuilder({
      customSources: CUSTOM_BLOCKS,
      enableHistory: true,
      enableExportAsPlugin: true,
      showPlugins: true,
      publicCss: ['/bootstrap/bootstrap.min.css', '/swiper/swiper-bundle.min.css'],
      publicJs: ['/bootstrap/bootstrap.min.js'],
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
    {
      provide: NGX_PAGE_BUILDER_EXPORT_PLUGIN_STORE,
      useExisting: PluginService,
      deps: [],
    },
  ],
})
export class ContentModule {}
