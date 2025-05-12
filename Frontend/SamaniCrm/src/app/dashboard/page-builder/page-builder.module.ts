import { PageBuilderComponent } from './page-builder/page-builder.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageBuilderRoutingModule } from './page-builder-routing.module';
import { MaterialCommonModule } from '@shared/material/material.common.module';


@NgModule({
  declarations: [PageBuilderComponent],
  imports: [CommonModule, PageBuilderRoutingModule, MaterialCommonModule],
})
export class PageBuilderModule {}
