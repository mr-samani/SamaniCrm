import { SelectPageUrlComponent } from './select-page-url/select-page-url.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';
import { SwitchModule } from '@shared/components/switch/switch.module';
import { MenuComponent } from './menu.component';
import { MenuRoutingModule } from './menu-routing.module';
import { TreeMenuComponent } from './tree-menu/tree-menu.component';
import { CreateOrEditMenuComponent } from './create-or-edit/create-or-edit.component';
import { MenuServiceProxy, PagesServiceProxy } from '@shared/service-proxies';
import { SharedModule } from '@shared/shared.module';
import { TableViewModule } from '@shared/components/table-view/table-view.module';
import { PaginationComponent } from '@shared/components/pagination/pagination.component';

@NgModule({
  declarations: [MenuComponent, TreeMenuComponent, CreateOrEditMenuComponent, SelectPageUrlComponent],
  imports: [
    CommonModule,
    MenuRoutingModule,
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialCommonModule,
    TabGroupModule,
    SwitchModule,
    SharedModule,
    TableViewModule,
    PaginationComponent,
  ],
  providers: [MenuServiceProxy, PagesServiceProxy],
})
export class MenuModule {}
