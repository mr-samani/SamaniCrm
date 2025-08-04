import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BuilderRoutingModule } from './builder-routing.module';
import { BuilderComponent } from './builder.component';
import { TranslateModule } from '@ngx-translate/core';
import { NgxDragDropKitModule } from 'ngx-drag-drop-kit';
import { DynamicRendererComponent } from './blocks/dynamic-renderer.component';
import { FormBuilderService } from './services/form-builder.service';
import { PropertiesComponent } from './properties/properties.component';
import { LayoutsComponent } from './layouts/layouts.component';
import { ToolbarComponent } from './toolbar/toolbar.component';
import { ToolboxComponent } from './toolbox/toolbox.component';
import { FormBuilderBackendService } from './services/backend.service';
import { PageBuilderServiceProxy, PagesServiceProxy } from '@shared/service-proxies';
import { StyleProperties } from './properties/styles/style.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxInputBoxShadowModule, NgxInputColorModule, NgxInputGradientModule } from 'ngx-input-color';
import { BlockAttributesComponent } from './properties/attributes/attributes.component';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { SelectDynamicDataComponent } from './properties/select-dynamic-data/select-dynamic-data.component';
import { DynamicDataService } from './services/dynamic-data.service';
import { HistoryService } from './services/history.service';
import { StyleViewPipe } from './pipes/style-view.pipe';

@NgModule({
  declarations: [
    BuilderComponent,
    PropertiesComponent,
    BlockAttributesComponent,
    SelectDynamicDataComponent,
    LayoutsComponent,
    ToolbarComponent,
    ToolboxComponent,
  ],
  imports: [
    CommonModule,
    NgxDragDropKitModule,
    DynamicRendererComponent,
    TranslateModule,
    BuilderRoutingModule,
    StyleProperties,
    FormsModule,
    ReactiveFormsModule,
    NgxInputColorModule,
    NgxInputGradientModule,
    NgxInputBoxShadowModule,
    FileManagerModule,
    MaterialCommonModule,
    StyleViewPipe,
  ],
  providers: [
    FormBuilderService,
    FormBuilderBackendService,
    DynamicDataService,
    HistoryService,
    PagesServiceProxy,
    PageBuilderServiceProxy,
  ],
})
export class BuilderModule {}
