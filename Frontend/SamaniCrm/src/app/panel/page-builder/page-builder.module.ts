import { PageBuilderComponent } from './page-builder/page-builder.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageBuilderRoutingModule } from './page-builder-routing.module';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { PagesServiceProxy } from '@shared/service-proxies';
import { SharedModule } from '@shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';
import { CreateOrEditPageMetaDataDialogComponent } from '../content/create-or-edit-page-meta-data-dialog/create-or-edit-page-meta-data-dialog.component';

@NgModule({
  declarations: [PageBuilderComponent],
  imports: [
    CommonModule,
    PageBuilderRoutingModule,
    MaterialCommonModule,
    SharedModule,
    TranslateModule,
    CreateOrEditPageMetaDataDialogComponent,
  ],
  providers: [PagesServiceProxy],
})
export class PageBuilderModule {}
