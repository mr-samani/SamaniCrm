import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BuilderRoutingModule } from './builder-routing.module';
import { BuilderComponent } from './builder.component';
import { TranslateModule } from '@ngx-translate/core';
import { NgxDragDropKitModule } from 'ngx-drag-drop-kit';
import { DynamicRendererComponent } from './blocks/dynamic-renderer.component';
import { FormBuilderService } from './form-builder.service';
import { PropertiesComponent } from './properties/properties.component';
import { LayoutsComponent } from './layouts/layouts.component';
import { ToolbarComponent } from './toolbar/toolbar.component';
import { ToolboxComponent } from './toolbox/toolbox.component';
import { FormBuilderBackendService } from './backend.service';
import { PagesServiceProxy } from '@shared/service-proxies';

@NgModule({
  declarations: [BuilderComponent, PropertiesComponent, LayoutsComponent, ToolbarComponent, ToolboxComponent],
  imports: [CommonModule, NgxDragDropKitModule, DynamicRendererComponent, TranslateModule, BuilderRoutingModule],
  providers: [FormBuilderService, FormBuilderBackendService, PagesServiceProxy],
})
export class BuilderModule {}
